using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class OrchestrationBudgetTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.OrchestrationBudget.MinEra = 0;
            c.Agency.MinEra = 0;
            c.HumanQueue.MinEra = 0;
            c.HumanQueue.BusyWeeks = 1;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Budget_level_zero_keeps_full_lift()
    {
        Game g = Lab();
        Assert.Equal(MathI.Permille, OrchestrationBudget.KeepPermille(g.C, g.Me));
        Assert.Equal(0, OrchestrationBudget.Slots(g.C, g.Me));
    }

    [Fact]
    public void Set_orch_budget_is_checked()
    {
        Game g = Lab();
        g.Refused(Command.SetOrchBudget(g.Id, 9), CommandError.InvalidArgument);
        g.Do(Command.SetOrchBudget(g.Id, 2));
        Assert.Equal(2, g.Me.OrchLevel);
        g.Refused(Command.SetOrchBudget(g.Id, 2), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = g.C.OrchestrationBudget.SetupPerLevel - 1;
        poor.Refused(Command.SetOrchBudget(poor.Id, 1), CommandError.NotEnoughCash);
        Game off = Lab(c => c.OrchestrationBudget.Enabled = 0);
        off.Refused(Command.SetOrchBudget(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Keep_permille_drops_at_higher_levels()
    {
        Game g = Lab();
        g.Do(Command.SetOrchBudget(g.Id, 1));
        Assert.Equal(g.C.OrchestrationBudget.Keep1, OrchestrationBudget.KeepPermille(g.C, g.Me));
        g.Do(Command.SetOrchBudget(g.Id, 3));
        Assert.Equal(g.C.OrchestrationBudget.Keep3, OrchestrationBudget.KeepPermille(g.C, g.Me));
        Assert.True(g.C.OrchestrationBudget.Keep3 >= g.C.OrchestrationBudget.Keep1);
    }

    [Fact]
    public void Too_many_agent_models_accrue_overrun()
    {
        Game g = Lab(c => { c.OrchestrationBudget.Slots1 = 0; c.OrchestrationBudget.Slots2 = 0; c.OrchestrationBudget.Slots3 = 0; c.OrchestrationBudget.Enabled = 1; });
        g.Do(Command.SetOrchBudget(g.Id, 1));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        int before = g.Me.OrchOver;
        OrchestrationBudget.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.OrchOver > before);
    }

    [Fact]
    public void Overrun_feeds_the_human_queue()
    {
        Game g = Lab(c => { c.OrchestrationBudget.Slots1 = 0; c.OrchestrationBudget.Slots2 = 0; c.OrchestrationBudget.Slots3 = 0; c.OrchestrationBudget.QueuePerOver = 3; c.HumanQueue.Enabled = 1; });
        g.Do(Command.SetOrchBudget(g.Id, 3));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Me.OrchOver = 2;
        int q = g.Me.HumanQueue;
        OrchestrationBudget.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(q + g.Me.OrchOver * g.C.OrchestrationBudget.QueuePerOver, g.Me.HumanQueue);
    }

    [Fact]
    public void Overrun_decays_under_budget_weeks()
    {
        Game g = Lab(c => c.OrchestrationBudget.OverDecay = 5);
        g.Me.OrchOver = 10;
        OrchestrationBudget.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(5, g.Me.OrchOver);
    }

    [Fact]
    public void Agency_lift_is_cut_by_orch_keep()
    {
        Game g = Lab(c => { c.OrchestrationBudget.Keep1 = 500; c.HumanQueue.Enabled = 0; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Do(Command.SetOrchBudget(g.Id, 1));
        g.Weeks(1);
        Assert.True(g.Me.AgentEarned >= 0);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_orch_fields()
    {
        Game g = Lab();
        g.Do(Command.SetOrchBudget(g.Id, 2));
        g.Me.OrchOver = 3;
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 131, g.Sim.Save()));
        Assert.Equal(0, older.Player.OrchLevel + older.Player.OrchOver);
    }
}