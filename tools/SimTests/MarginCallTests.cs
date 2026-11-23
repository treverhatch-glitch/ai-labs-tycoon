using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class MarginCallTests
{
    static Game Lab()
    {
        Game g = Game.Solo(51, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 2000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 2);
        g.Me.Cash = 500_000_000;
        return g;
    }

    [Fact]
    public void Breach_after_collateral_falls_fires_margin_call()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 200_000));
        int before = g.Me.Fleet.Sum(b => b.Count);
        Fleet.Take(g.Me, g.Me.Fleet[0], before - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.MarginCallLeft > 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MarginCall);
    }

    [Fact]
    public void Pay_clears_margin_call()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 300_000));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, (int)MarginCall.CashDue(g.C, g.State, g.Me)));
        Assert.Equal(0, g.Me.MarginCallLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MarginCallSatisfied);
    }

    [Fact]
    public void Save_at_143_has_no_margin_field()
    {
        Game g = Lab();
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 143, g.Sim.Save()));
        Assert.Equal(0, older.Player.MarginCallLeft);
    }
}