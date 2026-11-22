using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraLSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 58, System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Drills.MinEra = 0;
            c.EvalHarness.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.Agency.MinEra = 0;
            c.ReleaseChecklist.MinEra = 0;
            c.ReleaseChecklist.Enabled = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000_000;
        g.Me.ResearchPoints = 500_000;
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
    public void Drill_refuses_while_another_drill_runs()
    {
        Game g = Lab(58);
        g.Do(Command.StartDrill(g.Id));
        g.Refused(Command.StartDrill(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Tool_policy_refuses_invalid_mask()
    {
        Game g = Lab(59, c => c.Agency.MinEra = 0);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Refused(Command.SetToolPolicy(g.Id, m.Id, 8), CommandError.InvalidArgument);
    }

    [Fact]
    public void Eval_refuses_invalid_suite_id()
    {
        Game g = Lab(60);
        ModelState m = Sold(g);
        g.Refused(Command.RegisterEval(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.RegisterEval(g.Id, m.Id, 3), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_lab_with_drill_and_eval_validates_save()
    {
        Game g = Lab(61);
        g.Do(Command.StartDrill(g.Id));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        back.StepMany(2);
        Assert.NotNull(back.Player);
    }

    [Fact]
    public void Drill_and_checklist_on_same_lab_survive_save_load()
    {
        Game g = Lab(62, c => c.ReleaseChecklist.Enabled = 1);
        g.Do(Command.StartDrill(g.Id));
        ModelState m = g.Train("mini", 50);
        g.UntilTrained(m);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Me.DrillPrep, back.Player.DrillPrep);
        Assert.Equal(g.Me.Models[0].ChkDone, back.Player.Models[0].ChkDone);
    }
}