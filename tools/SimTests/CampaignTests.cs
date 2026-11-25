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

/// <summary>Era goals marked when an era ends, and the run's score.</summary>
public class CampaignTests
{
    const string Sector = "web-ads";

    static Game Lab(Action<GameConfig> tweak = null, string era = "chat-boom")
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = era, RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
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

    static void ToNextEra(Game g)
    {
        string era = Pivot.EraId(g.C, g.State);
        int guard = 0;
        while (Pivot.EraId(g.C, g.State) == era && guard++ < 4000) g.Weeks(1);
        Assert.NotEqual(era, Pivot.EraId(g.C, g.State));
        g.Weeks(1);
    }

    static void OnlyGoals(GameConfig c, string eraId, params (GoalMetric metric, long target)[] goals)
    {
        c.Campaign.Goals.RemoveAll(x => x.EraId == eraId);
        int n = 0;
        foreach ((GoalMetric metric, long target) in goals)
            c.Campaign.Goals.Add(new EraGoalConfig { IdValue = eraId + "-t" + (n++), EraId = eraId, Name = "Goal " + n, Metric = metric, Target = target, Points = 100 });
    }

    // ---- the data

    [Fact]
    public void Every_era_has_goals_and_the_shipped_data_validates()
    {
        GameConfig c = TestData.Config();
        Assert.All(c.Eras, e => Assert.True(c.Campaign.GoalsOf(e.Id).Count >= 2, e.Id + " needs goals"));
        Assert.True(c.Campaign.AllMetBonus > 0);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Revenue_targets_rise_with_the_eras()
    {
        GameConfig c = TestData.Config();
        long last = 0;
        foreach (EraConfig e in c.Eras)
        {
            long target = c.Campaign.GoalsOf(e.Id).First(g => g.Metric == GoalMetric.LifetimeRevenue).Target;
            Assert.True(target > last, e.Id + " should ask for more than the era before");
            last = target;
        }
    }

    [Fact]
    public void The_validator_catches_bad_campaign_data()
    {
        AssertProblem(Problems(c => c.Campaign.AllMetBonus = -1), "campaign.allMetBonus");
        AssertProblem(Problems(c => c.Campaign.Goals[0].EraId = "the-fifties"), "unknown era");
        AssertProblem(Problems(c => c.Campaign.Goals[0].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Campaign.Goals[0].Target = 0), "target under 1");
        AssertProblem(Problems(c => c.Campaign.Goals[0].Points = 0), "under 1 point");
        AssertProblem(Problems(c => c.Campaign.Goals.RemoveAll(g => g.EraId == "agents")), "era agents has no campaign goals");
        AssertProblem(Problems(c => c.Campaign.Goals.Add(new EraGoalConfig { IdValue = c.Campaign.Goals[0].IdValue, EraId = "agents", Name = "x", Target = 1, Points = 1 })), "campaign.goals");
        AssertProblem(Problems(c => { c.Campaign.Goals[0].Metric = GoalMetric.Reputation; c.Campaign.Goals[0].Target = 1001; }), "0 to 1000");
        AssertProblem(Problems(c => c.Campaign = null), "campaign is missing");
    }

    // ---- what each goal measures

    [Fact]
    public void Each_metric_reads_the_players_lab()
    {
        Game g = Lab();
        Assert.Equal(g.Me.Staff.Count, Campaign.Value(g.C, g.Me, GoalMetric.Staff, g.State));
        Assert.Equal(g.Me.Reputation, Campaign.Value(g.C, g.Me, GoalMetric.Reputation, g.State));
        Assert.Equal(g.Me.Techs.Count, Campaign.Value(g.C, g.Me, GoalMetric.Techs, g.State));
        Assert.Equal(g.Me.Ledger.Lifetime.Revenue, Campaign.Value(g.C, g.Me, GoalMetric.LifetimeRevenue, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.ModelsReleased, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.SectorsServed, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.SectorsLed, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.BestAlignment, g.State));
    }

    [Fact]
    public void Releases_sectors_and_alignment_count_from_models_on_sale()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        m.Alignment = 640;
        g.Release(m, Sector);
        Assert.Equal(1, Campaign.Value(g.C, g.Me, GoalMetric.ModelsReleased, g.State));
        Assert.Equal(1, Campaign.Value(g.C, g.Me, GoalMetric.SectorsServed, g.State));
        Assert.Equal(640, Campaign.Value(g.C, g.Me, GoalMetric.BestAlignment, g.State));
        m.Status = ModelStatus.Retired;
        Assert.Equal(1, Campaign.Value(g.C, g.Me, GoalMetric.ModelsReleased, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.SectorsServed, g.State));
        Assert.Equal(0, Campaign.Value(g.C, g.Me, GoalMetric.BestAlignment, g.State));
    }

    [Fact]
    public void Crowns_count_the_sectors_the_lab_holds()
    {
        Game g = Lab();
        g.State.Sector(Sector).ChampionLabId = g.Id;
        g.State.Sectors[1].ChampionLabId = g.Id;
        g.State.Sectors[2].ChampionLabId = g.Id + 99;
        Assert.Equal(2, Campaign.Value(g.C, g.Me, GoalMetric.SectorsLed, g.State));
    }

    // ---- marking

    [Fact]
    public void A_mark_adds_the_points_of_the_goals_met_and_the_bonus_for_all_of_them()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1), (GoalMetric.Staff, 2), (GoalMetric.Reputation, 1000)));
        EraScore mark = Campaign.Mark(g.C, g.State, g.Me, "chat-boom");
        Assert.Equal(3, mark.Of);
        Assert.Equal(2, mark.Met);
        Assert.Equal(200, mark.Points);
        g.Me.Reputation = 1000;
        mark = Campaign.Mark(g.C, g.State, g.Me, "chat-boom");
        Assert.Equal(3, mark.Met);
        Assert.Equal(300 + g.C.Campaign.AllMetBonus, mark.Points);
    }

    [Fact]
    public void Progress_lists_each_goal_with_its_value_and_whether_it_is_met()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1), (GoalMetric.Staff, 10000)));
        List<Campaign.GoalProgress> p = Campaign.Progress(g.C, g.State, g.Me, "chat-boom");
        Assert.Equal(2, p.Count);
        Assert.True(p[0].Met);
        Assert.False(p[1].Met);
        Assert.Equal(g.Me.Staff.Count, p[0].Value);
    }

    [Fact]
    public void An_era_with_no_goals_is_marked_as_nothing()
    {
        Game g = Lab();
        EraScore mark = Campaign.Mark(g.C, g.State, g.Me, "no-such-era");
        Assert.Equal(0, mark.Of);
        Assert.Equal(0, mark.Points);
    }

    // ---- the era ends

    [Fact]
    public void When_an_era_ends_the_players_lab_is_marked_once()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1), (GoalMetric.Staff, 100000)));
        ToNextEra(g);
        EraScore score = Campaign.Recorded(g.Me, "chat-boom");
        Assert.NotNull(score);
        Assert.Equal(1, score.Met);
        Assert.Equal(2, score.Of);
        Assert.Equal(100, score.Points);
        Assert.Single(g.Me.EraScores);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.EraScored);
        Assert.Equal(g.Id, e.LabId);
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(100, e.Amount);
        Assert.Equal("chat-boom", e.Ref);
        Assert.Contains("1 goal met", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_mark_is_taken_when_the_era_ends_not_before()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1)));
        g.Weeks(10);
        Assert.Empty(g.Me.EraScores);
        ToNextEra(g);
        Assert.Equal(100 + g.C.Campaign.AllMetBonus, Campaign.Recorded(g.Me, "chat-boom").Points);
    }

    [Fact]
    public void Only_the_player_is_marked()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "chat-boom", RivalIds = new List<string> { TestData.Config().Rivals[0].Id } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        ToNextEra(g);
        Assert.All(g.State.Labs.Where(l => l.Id != g.State.PlayerLabId), l => Assert.Empty(l.EraScores));
        Assert.Single(g.State.Player.EraScores);
    }

    [Fact]
    public void A_game_that_starts_late_has_no_marks_for_the_eras_before_it()
    {
        Game g = Lab(null, "agents");
        Assert.Empty(g.Me.EraScores);
        ToNextEra(g);
        Assert.Single(g.Me.EraScores);
        Assert.Equal("agents", g.Me.EraScores[0].EraId);
    }

    [Fact]
    public void A_bankrupt_lab_is_not_marked()
    {
        Game g = Lab();
        g.Me.Bankrupt = true;
        ToNextEra(g);
        Assert.Empty(g.Me.EraScores);
    }

    [Fact]
    public void Two_eras_make_two_marks_in_order()
    {
        Game g = Lab();
        ToNextEra(g);
        ToNextEra(g);
        Assert.Equal(new[] { "chat-boom", "agents" }, g.Me.EraScores.Select(s => s.EraId).ToArray());
    }

    // ---- the run's score

    [Fact]
    public void The_run_score_is_the_finished_eras_plus_the_current_era_as_it_stands()
    {
        Game g = Lab(c =>
        {
            OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1));
            OnlyGoals(c, "agents", (GoalMetric.Staff, 1), (GoalMetric.Reputation, 1000));
        });
        Assert.Equal(100 + g.C.Campaign.AllMetBonus, Campaign.RunScore(g.C, g.State, g.Me));
        ToNextEra(g);
        int finished = 100 + g.C.Campaign.AllMetBonus;
        Assert.Equal(finished, g.Me.EraScores[0].Points);
        Assert.Equal(finished + 100, Campaign.RunScore(g.C, g.State, g.Me));
        g.Me.Reputation = 1000;
        Assert.Equal(finished + 200 + g.C.Campaign.AllMetBonus, Campaign.RunScore(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_score_is_not_counted_twice_for_an_era_already_marked()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1)));
        ToNextEra(g);
        int total = Campaign.RunScore(g.C, g.State, g.Me);
        Assert.Equal(g.Me.EraScores.Sum(s => s.Points) + Campaign.Mark(g.C, g.State, g.Me, Pivot.EraId(g.C, g.State)).Points, total);
    }

    // ---- nothing else moves

    [Fact]
    public void Scoring_changes_nothing_else_about_the_game()
    {
        Game g = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 1)));
        Game twin = Lab(c => OnlyGoals(c, "chat-boom", (GoalMetric.Staff, 100000)));
        ToNextEra(g);
        ToNextEra(twin);
        Assert.True(g.Me.EraScores[0].Points > twin.Me.EraScores[0].Points);
        Assert.Equal(g.Me.Cash, twin.Me.Cash);
        Assert.Equal(g.Me.Reputation, twin.Me.Reputation);
        Assert.Equal(g.Me.Staff.Count, twin.Me.Staff.Count);
    }

    [Fact]
    public void The_chat_has_an_era_line()
    {
        Game g = Lab();
        Assert.Contains(g.C.Chat.Lines, l => l.Kind == "EraScored");
    }

    // ---- saving

    [Fact]
    public void Marks_survive_a_save_and_the_game_carries_on_the_same()
    {
        Game g = Lab();
        ToNextEra(g);
        g.Weeks(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.EraScores.Count, again.Player.EraScores.Count);
        Assert.Equal(g.Me.EraScores[0].Points, again.Player.EraScores[0].Points);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        ToNextEra(g);
        while (Pivot.EraId(again.Config, again.State) == "chat-boom" || again.State.Tick < g.State.Tick) again.Step();
        Assert.Equal(g.Me.EraScores.Count, again.Player.EraScores.Count);
    }

    [Fact]
    public void An_older_save_loads_with_no_era_scored()
    {
        Game g = Lab();
        ToNextEra(g);
        string old = SaveSchemaPretend.At(g.C, 38, g.Sim.Save());
        Assert.DoesNotContain("eraScores", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => Assert.Empty(l.EraScores));
    }
}
