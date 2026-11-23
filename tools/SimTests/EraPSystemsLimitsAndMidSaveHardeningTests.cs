using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraPSystemsLimitsAndMidSaveHardeningTests
{
    static Game MarginLab(long seed = 140)
    {
        Game g = Game.Solo(seed, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    static void TriggerMarginCall(Game g)
    {
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.MarginCallLeft > 0);
    }

    [Fact]
    public void Satisfy_margin_call_refuses_when_none_outstanding()
    {
        Game g = MarginLab();
        g.Refused(Command.SatisfyMarginCall(g.Id, 0, 1000), CommandError.InvalidArgument);
    }

    [Fact]
    public void Law_shape_refuses_without_target_when_enabled()
    {
        Game g = Game.Solo(141, c =>
        {
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Refused(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Law_target_refuses_unknown_goal()
    {
        Game g = Game.Solo(142).WithStarterLab();
        g.Refused(Command.SetLawTarget(g.Id, "vague"), CommandError.UnknownId);
    }

    [Fact]
    public void Margin_call_left_mid_save_load_matches()
    {
        Game g = MarginLab(143);
        TriggerMarginCall(g);
        int left = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(left, back.Player.MarginCallLeft);
    }

    [Fact]
    public void Margin_satisfy_mid_save_load_clears_call()
    {
        Game g = MarginLab(144);
        TriggerMarginCall(g);
        int pay = (int)MarginCall.CashDue(g.C, g.State, g.Me);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, pay));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(0, back.Player.MarginCallLeft);
    }
}