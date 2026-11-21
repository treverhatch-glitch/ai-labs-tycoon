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

/// <summary>Mending a glitching model: a fine-tune ends the glitch and adds a little alignment; a rollback puts the older version back on sale.</summary>
public class ModelRepairTests
{
    const string Sector = "web-ads";

    static Game Lab(Action<GameConfig> tweak = null, bool plain = true)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            if (plain) c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static ModelState Ship(Game g, params string[] sectors)
    {
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        m.Capability = 1_000_000;
        g.Release(m, sectors.Length == 0 ? new[] { Sector } : sectors);
        return m;
    }

    /// <summary>Two versions of the mini: the first retired, the second on sale and glitching after an incident today.</summary>
    static (Game g, ModelState older, ModelState newer) Glitching(Action<GameConfig> tweak = null, bool plain = true)
    {
        Game g = Lab(tweak, plain);
        ModelState older = Ship(g);
        ModelState newer = Ship(g);
        Assert.Equal(ModelStatus.Retired, older.Status);
        newer.Incidents = 1;
        newer.LastIncidentTick = g.State.Tick;
        return (g, older, newer);
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

    // ---- the table and the glitch

    [Fact]
    public void The_table_is_in_the_data_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Repair.FineTuneRevenueWeeks);
        Assert.Equal(1000, c.Repair.FineTuneMinCost);
        Assert.Equal(40, c.Repair.FineTuneAlignment);
        Assert.Equal(20, c.Repair.RollbackReputationLoss);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_model_is_glitching_for_the_glitch_weeks_after_an_incident_and_not_before_one()
    {
        var (g, older, newer) = Glitching();
        Assert.Equal(6, ModelRepair.GlitchWeeks(g.C, newer));
        Assert.Equal(6, ModelRepair.GlitchWeeksLeft(g.C, newer, g.State.Tick));
        Assert.Equal(1, ModelRepair.GlitchWeeksLeft(g.C, newer, g.State.Tick + 5));
        Assert.Equal(0, ModelRepair.GlitchWeeksLeft(g.C, newer, g.State.Tick + 6));
        newer.Incidents = 0;
        Assert.Equal(0, ModelRepair.GlitchWeeksLeft(g.C, newer, g.State.Tick));
        Assert.Equal(0, ModelRepair.GlitchWeeksLeft(g.C, older, g.State.Tick));
    }

    [Fact]
    public void The_glitch_runs_as_long_as_the_sector_with_the_longest_one()
    {
        Game g = Lab(plain: false);
        ModelState m = Ship(g, "web-ads", "autonomous-driving");
        m.Incidents = 1;
        m.LastIncidentTick = g.State.Tick;
        Assert.Equal(g.C.Safety.SectorFlavours.Max(f => f.GlitchWeeks), ModelRepair.GlitchWeeks(g.C, m));
    }

    // ---- fine-tune

    [Fact]
    public void A_fine_tune_ends_the_glitch_adds_alignment_and_costs_a_week_of_sales()
    {
        var (g, older, newer) = Glitching();
        g.State.Sector(Sector).Entry(g.Id).LastRevenue = 50_000;
        newer.Alignment = 500;
        newer.DisplayedAlignment = 520;
        long cash = g.Me.Cash;
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, newer, g.State.Tick));
        SectorConfig sector = g.C.Sector(Sector);
        long hurt = DisplayQueries.Quality(g.C, sector, newer, g.State.Tick);

        g.Do(Command.FineTuneModel(g.Id, newer.Id));

        Assert.Equal(cash - 50_000, g.Me.Cash);
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(g.C, g.Me, newer, g.State.Tick));
        Assert.Equal(0, ModelRepair.GlitchWeeksLeft(g.C, newer, g.State.Tick));
        Assert.Equal(540, newer.Alignment);
        Assert.Equal(560, newer.DisplayedAlignment);
        Assert.True(DisplayQueries.Quality(g.C, sector, newer, g.State.Tick) > hurt);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ModelFineTuned);
        Assert.Equal(newer.Id, e.SubjectId);
        Assert.Equal(50_000, e.Amount);
    }

    [Fact]
    public void A_fine_tune_never_costs_less_than_the_floor_and_never_pushes_alignment_past_the_top()
    {
        var (g, older, newer) = Glitching();
        g.State.Sector(Sector).Entry(g.Id).LastRevenue = 10;
        newer.Alignment = 990;
        newer.DisplayedAlignment = 990;
        long cash = g.Me.Cash;
        g.Do(Command.FineTuneModel(g.Id, newer.Id));
        Assert.Equal(cash - 1000, g.Me.Cash);
        Assert.Equal(1000, newer.Alignment);
        Assert.Equal(1000, newer.DisplayedAlignment);
    }

    [Fact]
    public void A_fine_tune_is_refused_when_nothing_glitches_or_the_cash_is_short_and_changes_nothing()
    {
        var (g, older, newer) = Glitching();
        g.Refused(Command.FineTuneModel(g.Id, 99999), CommandError.UnknownId);
        g.Refused(Command.FineTuneModel(g.Id, older.Id), CommandError.InvalidArgument);
        g.State.Sector(Sector).Entry(g.Id).LastRevenue = 50_000;
        g.Me.Cash = 100;
        g.Refused(Command.FineTuneModel(g.Id, newer.Id), CommandError.NotEnoughCash);
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.FineTuneModel(g.Id, newer.Id));
        g.Refused(Command.FineTuneModel(g.Id, newer.Id), CommandError.InvalidArgument);
    }

    // ---- rollback

    [Fact]
    public void A_rollback_puts_the_older_version_back_on_sale_and_withdraws_the_new_one()
    {
        var (g, older, newer) = Glitching();
        SectorEntry entry = g.State.Sector(Sector).Entry(g.Id);
        Assert.Equal(newer.Id, entry.ModelId);
        int rep = g.Me.Reputation;
        long cash = g.Me.Cash;

        g.Do(Command.RollBackModel(g.Id, newer.Id));

        Assert.Equal(older.Id, entry.ModelId);
        Assert.Equal(ModelStatus.Released, older.Status);
        Assert.Contains(Sector, older.Sectors);
        Assert.Equal(ModelStatus.Trained, newer.Status);
        Assert.Empty(newer.Sectors);
        Assert.Equal(rep - 20, g.Me.Reputation);
        Assert.Equal(cash, g.Me.Cash);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ModelRolledBack);
        Assert.Equal(older.Id, e.SubjectId);
        Assert.Equal(newer.Name, e.Ref);
        Assert.Equal(20, e.Amount);
    }

    [Fact]
    public void A_rollback_goes_to_the_newest_older_version_and_every_sector_the_new_model_was_in()
    {
        Game g = Lab();
        ModelState one = Ship(g);
        ModelState two = Ship(g);
        ModelState three = Ship(g, "web-ads", "spam-search");
        three.Incidents = 1;
        three.LastIncidentTick = g.State.Tick;
        Assert.Same(two, ModelRepair.RollbackTarget(g.Me, three));
        g.Do(Command.RollBackModel(g.Id, three.Id));
        Assert.Equal(two.Id, g.State.Sector("web-ads").Entry(g.Id).ModelId);
        Assert.Equal(two.Id, g.State.Sector("spam-search").Entry(g.Id).ModelId);
        Assert.Equal(ModelStatus.Retired, one.Status);
        Assert.Equal(new[] { "web-ads", "spam-search" }.OrderBy(x => x), two.Sectors.OrderBy(x => x));
    }

    [Fact]
    public void A_rollback_is_refused_with_nothing_to_go_back_to_or_no_glitch_and_changes_nothing()
    {
        Game g = Lab();
        ModelState only = Ship(g);
        only.Incidents = 1;
        only.LastIncidentTick = g.State.Tick;
        g.Refused(Command.RollBackModel(g.Id, only.Id), CommandError.InvalidArgument);
        g.Refused(Command.RollBackModel(g.Id, 99999), CommandError.UnknownId);
        var (g2, older, newer) = Glitching();
        newer.Incidents = 0;
        g2.Refused(Command.RollBackModel(g2.Id, newer.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rollback_will_not_bring_back_a_version_the_labs_own_gate_or_the_law_would_stop()
    {
        var (g, older, newer) = Glitching();
        older.DisplayedAlignment = 300;
        g.Me.ReleaseGate = 700;
        g.Refused(Command.RollBackModel(g.Id, newer.Id), CommandError.BelowReleaseGate);
        g.Me.ReleaseGate = 0;
        older.DisplayedAlignment = 0;
        older.Alignment = 0;
        int bar = EraRules.ReleaseBar(g.C, g.State.Tick);
        if (bar > 0) g.Refused(Command.RollBackModel(g.Id, newer.Id), CommandError.BelowEraSafetyBar);
        else Assert.True(EraRules.MeetsReleaseBar(g.C, older, g.State.Tick));
    }

    [Fact]
    public void The_withdrawn_model_can_be_sold_again_and_the_older_one_steps_back_aside()
    {
        var (g, older, newer) = Glitching();
        g.Do(Command.RollBackModel(g.Id, newer.Id));
        g.Do(Command.ReleaseModel(g.Id, newer.Id, Sector));
        Assert.Equal(newer.Id, g.State.Sector(Sector).Entry(g.Id).ModelId);
        Assert.Equal(ModelStatus.Released, newer.Status);
        Assert.Equal(ModelStatus.Retired, older.Status);
    }

    // ---- words and saves

    [Fact]
    public void The_log_and_the_chat_say_what_was_done()
    {
        var (g, older, newer) = Glitching();
        g.State.Sector(Sector).Entry(g.Id).LastRevenue = 5_000;
        g.Do(Command.FineTuneModel(g.Id, newer.Id));
        GameEvent tuned = g.State.Events.Last(e => e.Kind == EventKind.ModelFineTuned);
        Assert.Contains("to fine-tune " + newer.Name, EventText.Describe(g.C, g.State, tuned));
        Assert.Contains(newer.Name, CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.ModelFineTuned).Text);

        newer.Incidents = 1;
        newer.LastIncidentTick = g.State.Tick;
        g.Do(Command.RollBackModel(g.Id, newer.Id));
        GameEvent back = g.State.Events.Last(e => e.Kind == EventKind.ModelRolledBack);
        string text = EventText.Describe(g.C, g.State, back);
        Assert.Contains("pulled " + newer.Name, text);
        Assert.Contains("put " + older.Name + " back on sale", text);
        string said = CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.ModelRolledBack).Text;
        Assert.Contains(older.Name, said);
        Assert.Contains(newer.Name, said);
    }

    [Fact]
    public void A_rolled_back_lab_saves_loads_and_plays_on_the_same()
    {
        var (g, older, newer) = Glitching();
        g.Do(Command.RollBackModel(g.Id, newer.Id));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(40);
        again.StepMany(40);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void The_validator_catches_a_broken_table()
    {
        AssertProblem(Problems(c => c.Repair.FineTuneRevenueWeeks = -1), "repair.fineTuneRevenueWeeks");
        AssertProblem(Problems(c => c.Repair.FineTuneMinCost = -1), "repair.fineTuneMinCost");
        AssertProblem(Problems(c => c.Repair.FineTuneAlignment = 1001), "repair.fineTuneAlignment");
        AssertProblem(Problems(c => c.Repair.RollbackReputationLoss = -1), "repair.rollbackReputationLoss");
    }
}
