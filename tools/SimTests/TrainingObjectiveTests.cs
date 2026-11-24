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

/// <summary>What a training run is for: correct, engaging, obedient or careful. The choice is made while the model trains and lands when it finishes.</summary>
public class TrainingObjectiveTests
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
            c.Shocks.Enabled = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    /// <summary>A model trained with this objective, and the same run in an identical game with no objective chosen.</summary>
    static (ModelState chosen, ModelState plain, Game g, Game twin) Pair(string objective, Action<GameConfig> tweak = null, int share = 600)
    {
        Game g = Lab(tweak);
        Game twin = Lab(tweak);
        ModelState m = g.Train("mini", share);
        ModelState t = twin.Train("mini", share);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, objective));
        g.UntilTrained(m);
        twin.UntilTrained(t);
        return (m, t, g, twin);
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
    public void The_four_objectives_are_in_the_data_and_the_default_changes_nothing()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "correct", "engaging", "obedient", "careful" }, c.Objectives.Objectives.Select(o => o.IdValue).ToArray());
        ObjectiveConfig d = c.Objectives.Objective(c.Objectives.DefaultId);
        Assert.Equal(0, d.AlignmentPoints + d.CapabilityPermille + d.AppealPermille + d.CulturePoints);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Engaging_and_obedient_trade_control_for_something_and_careful_trades_the_other_way()
    {
        GameConfig c = TestData.Config();
        ObjectiveConfig engaging = c.Objectives.Objective("engaging"), obedient = c.Objectives.Objective("obedient"), careful = c.Objectives.Objective("careful");
        Assert.True(engaging.AlignmentPoints < 0 && engaging.AppealPermille > 0);
        Assert.True(obedient.AlignmentPoints < 0 && obedient.CapabilityPermille > 0);
        Assert.True(careful.AlignmentPoints > 0 && careful.CapabilityPermille < 0 && careful.AppealPermille < 0);
    }

    [Fact]
    public void The_validator_catches_bad_objective_data()
    {
        AssertProblem(Problems(c => c.Objectives.DefaultId = "nope"), "objectives.defaultId");
        AssertProblem(Problems(c => c.Objectives.Objective("correct").AlignmentPoints = 10), "must change nothing");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].IdValue = "correct"), "objectives");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].Text = ""), "has no text");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].AlignmentPoints = 1001), "alignmentPoints");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].CapabilityPermille = -1000), "capabilityPermille");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].AppealPermille = -1000), "appealPermille");
        AssertProblem(Problems(c => c.Objectives.Objectives[1].CulturePoints = 1001), "culturePoints");
    }

    // ---- what they do

    [Fact]
    public void Choosing_the_default_changes_nothing()
    {
        var (m, t, g, twin) = Pair("correct");
        Assert.Equal("", m.Objective);
        Assert.Equal(t.Capability, m.Capability);
        Assert.Equal(t.Alignment, m.Alignment);
        Assert.Equal(twin.Sim.Digest(), g.Sim.Digest());
    }

    [Fact]
    public void An_engaging_run_costs_alignment_and_makes_the_model_sell_better()
    {
        var (m, t, g, twin) = Pair("engaging");
        Assert.Equal("engaging", m.Objective);
        Assert.Equal(t.Capability, m.Capability);
        Assert.Equal(t.Alignment - 80, m.Alignment);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        SectorConfig sector = g.C.Sector(Sector);
        AppealBreakdown(g, m, out var chosen);
        AppealBreakdown(twin, t, out var plain);
        Assert.True(chosen.ObjectiveEffect > 0);
        Assert.Equal(0, plain.ObjectiveEffect);
        Assert.Equal(chosen.Total, DisplayQueries.Appeal(g.C, g.State, sector, g.State.Sector(Sector), g.Me, m));
    }

    static void AppealBreakdown(Game g, ModelState m, out DisplayQueries.AppealBreakdown parts)
    {
        parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Sector), g.State.Sector(Sector), g.Me, m);
    }

    [Fact]
    public void The_appeal_effect_is_the_objectives_share_of_everything_before_it()
    {
        var (m, _, g, _) = Pair("engaging");
        AppealBreakdown(g, m, out var parts);
        long before = parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.NamingConfusion + parts.ChampionBoost;
        Assert.Equal(MathI.MulPermille(before, 150), parts.ObjectiveEffect);
    }

    [Fact]
    public void An_obedient_run_adds_capability_and_costs_alignment()
    {
        var (m, t, _, _) = Pair("obedient");
        Assert.Equal(t.Capability + MathI.MulPermille(t.Capability, 60), m.Capability);
        Assert.Equal(t.Alignment - 60, m.Alignment);
        Assert.Equal(m.Capability, m.CapBase + m.CapTech + m.CapCarryover + m.CapLuck);
    }

    [Fact]
    public void A_careful_run_adds_alignment_and_costs_capability_and_appeal()
    {
        var (m, t, g, twin) = Pair("careful", null, 0);

        Assert.Equal(t.Alignment + 100, m.Alignment);
        Assert.Equal(t.Capability - MathI.MulPermille(t.Capability, 70), m.Capability);
        AppealBreakdown(g, m, out var parts);
        Assert.True(parts.ObjectiveEffect < 0);
        Assert.Equal(m.Capability, m.CapBase + m.CapTech + m.CapCarryover + m.CapLuck);
    }

    [Fact]
    public void Alignment_stays_inside_its_range_and_capability_never_drops_below_one()
    {
        var (low, lowTwin, _, _) = Pair("engaging", null, 0);
        Assert.Equal(0, lowTwin.Alignment);
        Assert.Equal(0, low.Alignment);

        var (high, _, _, _) = Pair("careful", c => c.Objectives.Objective("careful").AlignmentPoints = 1000, 600);
        Assert.Equal(1000, high.Alignment);

        var (weak, _, _, _) = Pair("careful", c => c.Objectives.Objective("careful").CapabilityPermille = -999);
        Assert.True(weak.Capability >= 1);
    }

    [Fact]
    public void The_objective_moves_safety_culture_when_the_run_finishes_not_before()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = g.Train("mini", 600);
        twin.Train("mini", 600);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "engaging"));
        g.Weeks(1);
        twin.Weeks(1);
        Assert.Equal(twin.Me.CultureShift, g.Me.CultureShift);
        g.UntilTrained(m);
        Game other = Lab();
        ModelState om = other.Train("mini", 600);
        other.UntilTrained(om);
        Assert.Equal(other.Me.CultureShift - 10, g.Me.CultureShift);
    }

    // ---- choosing

    [Fact]
    public void The_last_choice_wins_and_each_change_is_logged_once()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "correct"));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ObjectiveSet);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "engaging"));
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "engaging"));
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.ObjectiveSet));
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "careful"));
        Assert.Equal("careful", m.Objective);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "correct"));
        Assert.Equal("", m.Objective);
        Assert.Equal(3, g.State.Events.Count(e => e.Kind == EventKind.ObjectiveSet));
        g.UntilTrained(m);
        Game twin = Lab();
        ModelState t = twin.Train("mini", 600);
        twin.UntilTrained(t);
        Assert.Equal(t.Alignment, m.Alignment);
    }

    [Fact]
    public void A_wrong_model_a_wrong_objective_or_a_finished_model_is_refused_and_changes_nothing()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Refused(Command.SetTrainingObjective(g.Id, 99999, "engaging"), CommandError.UnknownId);
        g.Refused(Command.SetTrainingObjective(g.Id, m.Id, "nope"), CommandError.UnknownId);
        g.Refused(Command.SetTrainingObjective(g.Id, m.Id, ""), CommandError.UnknownId);
        g.UntilTrained(m);
        g.Refused(Command.SetTrainingObjective(g.Id, m.Id, "engaging"), CommandError.WrongModelStatus);
        Assert.Equal("", m.Objective);
    }

    [Fact]
    public void The_log_and_the_chat_tell_it()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "engaging"));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ObjectiveSet);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Contains("Be engaging", EventText.Describe(g.C, g.State, e));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me), x => x.Kind == EventKind.ObjectiveSet);
    }

    [Fact]
    public void Rivals_and_the_autopilot_never_choose()
    {
        Game g = Game.New(1);
        g.Weeks(200);
        Assert.All(g.State.Labs.SelectMany(l => l.Models), m => Assert.Equal("", m.Objective));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ObjectiveSet);
    }

    // ---- saving

    [Fact]
    public void A_choice_made_mid_run_survives_a_save()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetTrainingObjective(g.Id, m.Id, "obedient"));
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal("obedient", again.Player.Model(m.Id).Objective);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(60);
        again.StepMany(60);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_every_model_trained_to_be_correct()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        string old = SaveSchemaPretend.At(g.C, 31, g.Sim.Save());
        Assert.DoesNotContain("\"objective\"", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs.SelectMany(l => l.Models), x => Assert.Equal("", x.Objective));
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void SaveGame_catches_an_unknown_objective()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        m.Objective = "nope";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown objective"));
    }
}
