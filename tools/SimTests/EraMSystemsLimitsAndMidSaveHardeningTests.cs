using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraMSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 80, System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.AgentSandbox.MinEra = 0;
            c.RunawayPlaybook.MinEra = 0;
            c.OrchestrationBudget.MinEra = 0;
            c.TrafficRouter.MinEra = 0;
            c.PublicMetrics.MinEra = 0;
            c.Agency.MinEra = 0;
            c.PlayerStart.Cash = 50_000_000;
            tweak?.Invoke(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 3, sales: 2, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    static ModelState Sold(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void Sandbox_refuses_while_sandbox_already_running()
    {
        Game g = Lab(80);
        ModelState m = Sold(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Refused(Command.StartSandbox(g.Id, m.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Route_share_refuses_out_of_range_ppm()
    {
        Game g = Lab(81);
        ModelState m = Sold(g);
        g.Refused(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm + 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Playbook_refuses_level_above_max()
    {
        Game g = Lab(82, c => c.RunawayPlaybook.MaxLevel = 2);
        g.Refused(Command.SetPlaybook(g.Id, 5), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_lab_with_orchestration_and_playbook_validates_save()
    {
        Game g = Lab(83);
        g.Do(Command.SetPlaybook(g.Id, 1));
        g.Do(Command.SetOrchBudget(g.Id, 2));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        back.StepMany(2);
        Assert.NotNull(back.Player);
    }

    [Fact]
    public void Route_and_playbook_on_same_lab_survive_save_load()
    {
        Game g = Lab(84);
        ModelState m = Sold(g);
        g.Do(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm / 4));
        g.Do(Command.SetPlaybook(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(m.RoutePpm, back.Player.Models[0].RoutePpm);
        Assert.Equal(g.Me.PlayLevel, back.Player.PlayLevel);
    }
}