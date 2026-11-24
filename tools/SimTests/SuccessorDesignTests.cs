using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Putting a finished model to work on its successor: a cheaper, stronger run, and a successor that is less well understood.</summary>
public class SuccessorDesignTests
{
    const string Sector = "web-ads";

    static Game Lab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Office.ChancePermille = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Safety.FullAlignmentComputePermille *= 3;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static ModelState Finished(Game g, long capability = 1_000_000)
    {
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        m.Capability = (int)capability;
        return m;
    }

    /// <summary>A lab with a finished helper and a run in training, and the same lab without the help.</summary>
    static (Game g, ModelState helper, ModelState run, Game twin, ModelState twinRun) Setup(Action<GameConfig> tweak = null)
    {
        Game g = Lab(tweak);
        Game twin = Lab(tweak);
        ModelState helper = Finished(g);
        Finished(twin);
        ModelState run = g.Train("mini", 300);
        ModelState twinRun = twin.Train("mini", 300);
        return (g, helper, run, twin, twinRun);
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the data

    [Fact]
    public void The_shipped_numbers_are_a_real_trade_and_validate()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Successor.ComputeSavedPermille > 0 && c.Successor.CapabilityPermille > 0);
        Assert.True(c.Successor.AlignmentPoints > 0 && c.Successor.CulturePoints > 0 && c.Successor.ChainPoints > 0);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_successor_data()
    {
        AssertProblem(Problems(c => c.Successor.ComputeSavedPermille = 1000), "computeSavedPermille");
        AssertProblem(Problems(c => c.Successor.ComputeSavedPermille = -1), "computeSavedPermille");
        AssertProblem(Problems(c => c.Successor.CapabilityPermille = -1), "capabilityPermille");
        AssertProblem(Problems(c => c.Successor.AlignmentPoints = 1001), "alignmentPoints");
        AssertProblem(Problems(c => c.Successor.ChainPoints = -1), "chainPoints");
        AssertProblem(Problems(c => c.Successor.MaxChain = 21), "maxChain");
        AssertProblem(Problems(c => c.Successor.CulturePoints = -1), "culturePoints");
        AssertProblem(Problems(c => c.Successor.HelperMinPermille = 5001), "helperMinPermille");
    }

    // ---- what it does

    [Fact]
    public void Putting_a_model_to_work_saves_compute_and_the_run_finishes_sooner()
    {
        var (g, helper, run, twin, twinRun) = Setup();
        long before = run.RequiredCompute;
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        Assert.Equal(before - MathI.MulPermille(before, 150), run.RequiredCompute);
        Assert.Equal(helper.Id, run.DesignerId);
        int weeks = g.UntilTrained(run);
        int twinWeeks = twin.UntilTrained(twinRun);
        Assert.True(weeks < twinWeeks, weeks + " weeks with help against " + twinWeeks + " without");
    }

    [Fact]
    public void The_help_only_saves_what_the_run_still_needs()
    {
        var (g, helper, run, _, _) = Setup();
        g.Weeks(2);
        long remaining = run.RequiredCompute - run.ProgressCompute;
        long required = run.RequiredCompute;
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        Assert.Equal(required - MathI.MulPermille(remaining, 150), run.RequiredCompute);
        Assert.True(run.RequiredCompute > run.ProgressCompute);
    }

    [Fact]
    public void The_successor_is_stronger_and_less_well_understood()
    {
        var (g, helper, run, twin, twinRun) = Setup();
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        g.UntilTrained(run);
        twin.UntilTrained(twinRun);
        Assert.Equal(twinRun.Capability + MathI.MulPermille(twinRun.Capability, 50), run.Capability);
        Assert.Equal(run.Capability, run.CapBase + run.CapTech + run.CapCarryover + run.CapLuck);
        int plain = TrainingSystem.AlignmentFor(g.C, run.RequiredCompute, run.AlignmentCompute);
        Assert.True(plain >= 100 && plain <= 1000, "the test needs room to lose points, got " + plain);
        Assert.Equal(plain - 70, run.Alignment);
        Assert.Equal(run.Alignment, run.DisplayedAlignment);
    }

    [Fact]
    public void Safety_culture_loses_points_when_the_successor_finishes_not_before()
    {
        var (g, helper, run, twin, twinRun) = Setup();
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        g.Weeks(1);
        twin.Weeks(1);
        Assert.Equal(twin.Me.CultureShift, g.Me.CultureShift);
        g.UntilTrained(run);
        twin.UntilTrained(twinRun);
        int behind = g.Tick - twin.Tick;
        if (behind > 0) twin.Weeks(behind); else g.Weeks(-behind);
        Assert.Equal(twin.Tick, g.Tick);
        Assert.Equal(twin.Me.CultureShift - 15, g.Me.CultureShift);
    }

    [Fact]
    public void Every_link_of_the_chain_costs_more_alignment()
    {
        Game g = Lab();
        ModelState first = Finished(g);
        ModelState second = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, second.Id, first.Id));
        g.UntilTrained(second);
        second.Capability = 1_000_000;
        Assert.Equal(0, SuccessorDesign.Links(g.C, g.Me, first));
        Assert.Equal(1, SuccessorDesign.Links(g.C, g.Me, second));
        Assert.Equal(70, SuccessorDesign.AlignmentLoss(g.C, g.Me, first));
        Assert.Equal(110, SuccessorDesign.AlignmentLoss(g.C, g.Me, second));

        ModelState third = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, third.Id, second.Id));
        g.UntilTrained(third);
        int plain = TrainingSystem.AlignmentFor(g.C, third.RequiredCompute, third.AlignmentCompute);
        Assert.Equal(plain - 110, third.Alignment);
    }

    [Fact]
    public void The_chain_stops_adding_at_the_limit()
    {
        Game g = Lab(c => { c.Successor.MaxChain = 1; });
        ModelState a = Finished(g);
        ModelState b = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, b.Id, a.Id));
        g.UntilTrained(b);
        b.Capability = 1_000_000;
        ModelState cc = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, cc.Id, b.Id));
        g.UntilTrained(cc);
        cc.Capability = 1_000_000;
        Assert.Equal(1, SuccessorDesign.Links(g.C, g.Me, cc));
        Assert.Equal(70 + 40, SuccessorDesign.AlignmentLoss(g.C, g.Me, cc));
    }

    [Fact]
    public void A_helper_that_has_been_pruned_still_leaves_the_base_cost()
    {
        var (g, _, run, _, _) = Setup();
        run.DesignerId = 987654;
        g.UntilTrained(run);
        int plain = TrainingSystem.AlignmentFor(g.C, run.RequiredCompute, run.AlignmentCompute);
        Assert.Equal(plain - 70, run.Alignment);
    }

    // ---- the command

    [Fact]
    public void A_wrong_model_a_wrong_status_a_repeat_or_a_weak_helper_is_refused_and_changes_nothing()
    {
        var (g, helper, run, _, _) = Setup();
        g.Refused(Command.DesignWithModel(g.Id, 99999, helper.Id), CommandError.UnknownId);
        g.Refused(Command.DesignWithModel(g.Id, run.Id, 99999), CommandError.UnknownId);
        g.Refused(Command.DesignWithModel(g.Id, run.Id, run.Id), CommandError.WrongModelStatus);
        g.Refused(Command.DesignWithModel(g.Id, helper.Id, helper.Id), CommandError.WrongModelStatus);

        int real = helper.Capability;
        helper.Capability = 1;
        g.Refused(Command.DesignWithModel(g.Id, run.Id, helper.Id), CommandError.BelowSectorBar);
        helper.Capability = real;

        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        g.Refused(Command.DesignWithModel(g.Id, run.Id, helper.Id), CommandError.InvalidArgument);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.SuccessorDesigned));
    }

    [Fact]
    public void The_helper_needs_to_be_up_to_the_job_in_proportion_to_the_run()
    {
        var (g, helper, run, _, _) = Setup();
        long bar = SuccessorDesign.HelperBar(g.C, g.Me, run);
        Assert.True(bar > 0);
        helper.Capability = (int)bar;
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
    }

    [Fact]
    public void A_retired_model_can_still_help()
    {
        var (g, helper, run, _, _) = Setup();
        helper.Status = ModelStatus.Retired;
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        Assert.Equal(helper.Id, run.DesignerId);
    }

    [Fact]
    public void The_log_and_the_chat_tell_it()
    {
        var (g, helper, run, _, _) = Setup();
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.SuccessorDesigned);
        Assert.Equal(run.Id, e.SubjectId);
        string text = EventText.Describe(g.C, g.State, e);
        Assert.Contains(helper.Name, text);
        Assert.Contains(run.Name, text);
        Assert.Contains(run.Name, CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.SuccessorDesigned).Text);
    }

    [Fact]
    public void Rivals_and_the_autopilot_never_do_it()
    {
        Game g = Game.New(1);
        g.Weeks(200);
        Assert.All(g.State.Labs.SelectMany(l => l.Models), m => Assert.Equal(-1, m.DesignerId));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.SuccessorDesigned);
    }

    // ---- saving

    [Fact]
    public void The_helper_survives_a_save()
    {
        var (g, helper, run, _, _) = Setup();
        g.Do(Command.DesignWithModel(g.Id, run.Id, helper.Id));
        g.Weeks(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(helper.Id, again.Player.Model(run.Id).DesignerId);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(40);
        again.StepMany(40);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_every_model_designed_by_people()
    {
        var (g, _, _, _, _) = Setup();
        string old = SaveSchemaPretend.At(g.C, 31, g.Sim.Save());
        Assert.DoesNotContain("designerId", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs.SelectMany(l => l.Models), m => Assert.Equal(-1, m.DesignerId));
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }
}
