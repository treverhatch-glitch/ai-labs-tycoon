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

/// <summary>Skipping some of the testing before a first launch: a better launch window and a likelier incident.</summary>
public class RushedLaunchTests
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
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        m.Capability = 1_000_000;
        return m;
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

    static DisplayQueries.AppealBreakdown Appeal(Game g, ModelState m)
    {
        return DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Sector), g.State.Sector(Sector), g.Me, m);
    }

    // ---- the data

    [Fact]
    public void The_shipped_numbers_validate_and_a_rush_is_a_real_gamble()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Rush.MaxWeeks >= 1 && c.Rush.LaunchAppealPermillePerWeek > 0 && c.Rush.IncidentPermillePerWeek > 0 && c.Rush.CulturePointsPerWeek > 0);
        Assert.True(c.Rush.IncidentWeeks > c.Rush.LaunchWeeks, "the risk should outlast the boost");
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_rush_data()
    {
        AssertProblem(Problems(c => c.Rush.MaxWeeks = 0), "rush.maxWeeks");
        AssertProblem(Problems(c => c.Rush.LaunchWeeks = 0), "rush.launchWeeks");
        AssertProblem(Problems(c => c.Rush.IncidentWeeks = 0), "rush.incidentWeeks");
        AssertProblem(Problems(c => c.Rush.LaunchAppealPermillePerWeek = -1), "launchAppealPermillePerWeek");
        AssertProblem(Problems(c => c.Rush.LaunchAppealPermillePerWeek = 5000), "launchAppealPermillePerWeek");
        AssertProblem(Problems(c => c.Rush.IncidentPermillePerWeek = 500), "incidentPermillePerWeek");
        AssertProblem(Problems(c => c.Rush.CulturePointsPerWeek = 500), "culturePointsPerWeek");
    }

    // ---- a rushed launch

    [Fact]
    public void A_rushed_launch_puts_the_model_on_sale_records_the_weeks_and_costs_culture()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = Trained(g);
        ModelState t = Trained(twin);
        g.Do(Command.RushRelease(g.Id, m.Id, 4, Sector));
        twin.Do(Command.ReleaseModel(twin.Id, t.Id, Sector));
        Assert.Equal(ModelStatus.Released, m.Status);
        Assert.Equal(4, m.RushWeeks);
        Assert.Equal(0, t.RushWeeks);
        Assert.Equal(twin.Me.CultureShift - 12, g.Me.CultureShift);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ModelRushed);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Equal(4, e.Amount);
        Assert.Contains("4 weeks", EventText.Describe(g.C, g.State, e));
        Assert.DoesNotContain(twin.State.Events, x => x.Kind == EventKind.ModelRushed);
    }

    [Fact]
    public void The_launch_window_lifts_appeal_for_a_quarter_and_then_closes()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.RushRelease(g.Id, m.Id, 5, Sector));
        var open = Appeal(g, m);
        long before = open.Total - open.LaunchEffect;
        Assert.Equal(MathI.MulPermille(before, 1000 + 5 * 40) - before, open.LaunchEffect);
        Assert.Equal(13, RushedLaunch.LaunchWeeksLeft(g.C, g.Tick, m));
        g.Weeks(12);
        Assert.True(Appeal(g, m).LaunchEffect > 0);
        g.Weeks(1);
        Assert.Equal(0, Appeal(g, m).LaunchEffect);
        Assert.Equal(0, RushedLaunch.LaunchWeeksLeft(g.C, g.Tick, m));
    }

    [Fact]
    public void A_tested_launch_has_no_window()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Release(m, Sector);
        Assert.Equal(0, Appeal(g, m).LaunchEffect);
        Assert.Equal(0, RushedLaunch.IncidentPermille(g.C, g.Tick, m));
    }

    [Fact]
    public void A_rushed_model_is_likelier_to_have_an_incident_for_half_a_year()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.Alignment = 600;
        g.Do(Command.RushRelease(g.Id, m.Id, 4, Sector));
        int plain = SafetySystem.IncidentChanceWithoutRush(g.C, g.Tick, m);
        Assert.Equal(0, plain);
        Assert.Equal(12, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
        var parts = DisplayQueries.IncidentParts(g.C, g.Tick, m);
        Assert.Equal(12, parts.FromRush);
        Assert.Equal(parts.FromConfig + parts.FromAlignment + parts.FromCorruption + parts.FromRush, parts.Total);
        Assert.Equal(12, SafetySystem.IncidentChancePermille(g.C, m.ReleasedTick + 25, m));
        Assert.Equal(0, SafetySystem.IncidentChancePermille(g.C, m.ReleasedTick + 26, m));
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, m.ReleasedTick + 26, m).FromRush);
    }

    [Fact]
    public void A_rushed_model_really_has_incidents_and_a_tested_one_does_not_when_nothing_else_would()
    {
        Game g = Lab(c => c.Rush.IncidentPermillePerWeek = 100);
        Game twin = Lab(c => c.Rush.IncidentPermillePerWeek = 100);
        ModelState m = Trained(g);
        ModelState t = Trained(twin);
        g.Do(Command.RushRelease(g.Id, m.Id, 6, Sector));
        twin.Do(Command.ReleaseModel(twin.Id, t.Id, Sector));
        Assert.Equal(600, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
        g.Weeks(26);
        twin.Weeks(26);
        Assert.True(m.Incidents > 0);
        Assert.Equal(0, t.Incidents);
    }

    [Fact]
    public void The_rush_cannot_push_the_chance_past_certain()
    {
        Game g = Lab(c => { c.Safety.IncidentChancePermille = 900; c.Rush.IncidentPermillePerWeek = 100; });
        ModelState m = Trained(g);
        m.Alignment = 0;
        g.Do(Command.RushRelease(g.Id, m.Id, 6, Sector));
        Assert.Equal(1000, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
    }

    // ---- refusals

    [Fact]
    public void Wrong_arguments_are_refused_and_change_nothing()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Refused(Command.RushRelease(g.Id, 99999, 3, Sector), CommandError.UnknownId);
        g.Refused(Command.RushRelease(g.Id, m.Id, 0, Sector), CommandError.InvalidArgument);
        g.Refused(Command.RushRelease(g.Id, m.Id, g.C.Rush.MaxWeeks + 1, Sector), CommandError.InvalidArgument);
        g.Refused(Command.RushRelease(g.Id, m.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.RushRelease(g.Id, m.Id, 3, "nope"), CommandError.UnknownId);
        Assert.Equal(ModelStatus.Trained, m.Status);
        Assert.Equal(0, m.RushWeeks);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ModelRushed);
    }

    [Fact]
    public void A_model_still_training_or_already_on_sale_cannot_be_rushed()
    {
        Game g = Lab();
        ModelState training = g.Train("mini", 600);
        g.Refused(Command.RushRelease(g.Id, training.Id, 3, Sector), CommandError.WrongModelStatus);
        g.UntilTrained(training);
        training.Capability = 1_000_000;
        g.Release(training, Sector);
        g.Refused(Command.RushRelease(g.Id, training.Id, 3, Sector), CommandError.WrongModelStatus);
    }

    [Fact]
    public void A_rush_does_not_get_round_the_release_gate_or_the_laws_bar()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.Alignment = 100;
        m.DisplayedAlignment = 100;
        g.Do(Command.SetReleaseGate(g.Id, 900));
        g.Refused(Command.RushRelease(g.Id, m.Id, 3, Sector), CommandError.BelowReleaseGate);
        Assert.Equal(0, m.RushWeeks);
    }

    // ---- who does it

    [Fact]
    public void Rivals_and_the_autopilot_never_rush()
    {
        Game g = Game.New(1);
        g.Autopilot(200);
        Assert.All(g.State.Labs.SelectMany(l => l.Models), m => Assert.Equal(0, m.RushWeeks));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ModelRushed);
    }

    [Fact]
    public void The_chat_has_a_line_for_it()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.RushRelease(g.Id, m.Id, 2, Sector));
        Assert.Contains(m.Name, CompanyChat.Feed(g.C, g.State, g.Me).Last(x => x.Kind == EventKind.ModelRushed).Text);
    }

    // ---- saving

    [Fact]
    public void A_rushed_model_survives_a_save_mid_window()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.RushRelease(g.Id, m.Id, 3, Sector));
        g.Weeks(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(3, again.Player.Model(m.Id).RushWeeks);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_every_launch_tested()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Release(m, Sector);
        string old = SaveSchemaPretend.At(g.C, 35, g.Sim.Save());
        Assert.DoesNotContain("rushWeeks", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.RushWeeks));
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }
}
