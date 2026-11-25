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

/// <summary>Changing what the lab is good at, once per era: money, people, and a year of confused customers.</summary>
public class PivotTests
{
    const string Sector = "web-ads";

    static Game Lab(Action<GameConfig> tweak = null, string style = "sniper", string era = "agents")
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = era, StyleId = style, RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab(researchers: 4, engineers: 1, sales: 1);
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

    static DisplayQueries.AppealBreakdown Appeal(Game g, ModelState m)
    {
        return DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Sector), g.State.Sector(Sector), g.Me, m);
    }

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        g.Release(m, Sector);
        return m;
    }

    // ---- the data

    [Fact]
    public void The_shipped_numbers_validate_and_a_pivot_really_costs()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Pivot.CostWeeks > 0 && c.Pivot.StaffLossPermille > 0 && c.Pivot.ConfusionAppealPermille > 0);
        Assert.Equal(52, c.Pivot.ConfusionWeeks);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_pivot_data()
    {
        AssertProblem(Problems(c => c.Pivot.CostWeeks = 0), "pivot.costWeeks");
        AssertProblem(Problems(c => c.Pivot.MinWeekCost = -1), "pivot.minWeekCost");
        AssertProblem(Problems(c => c.Pivot.StaffLossPermille = 1001), "pivot.staffLossPermille");
        AssertProblem(Problems(c => c.Pivot.ConfusionWeeks = 0), "pivot.confusionWeeks");
        AssertProblem(Problems(c => c.Pivot.ConfusionAppealPermille = 1000), "pivot.confusionAppealPermille");
        AssertProblem(Problems(c => c.Pivot = null), "pivot is missing");
    }

    // ---- a pivot

    [Fact]
    public void A_pivot_changes_the_style_costs_money_and_loses_the_unhappiest_staff()
    {
        Game g = Lab(c => c.Pivot.StaffLossPermille = 340);
        g.Me.Staff[3].Morale = 10;
        g.Me.Staff[5].Morale = 20;
        int staff = g.Me.Staff.Count;
        int unhappy1 = g.Me.Staff[3].Id, unhappy2 = g.Me.Staff[5].Id;
        long cash = g.Me.Cash;
        long cost = Pivot.Cost(g.C, g.State, g.Me);
        int leavers = Pivot.Leavers(g.C, g.Me);
        Assert.True(leavers >= 2);
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        Assert.Equal("shotgun", g.Me.StyleId);
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(staff - leavers, g.Me.Staff.Count);
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == unhappy1 || p.Id == unhappy2);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.StylePivoted);
        Assert.Equal(leavers, e.SubjectId);
        Assert.Equal(cost, e.Amount);
        Assert.Equal("sniper|shotgun", e.Ref);
        Assert.Contains("pivoted", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_share_who_leave_is_rounded_down_with_at_least_one()
    {
        Game g = Lab();
        int n = g.Me.Staff.Count;
        Assert.Equal((int)MathI.Max(1, MathI.MulPermille(n, g.C.Pivot.StaffLossPermille)), Pivot.Leavers(g.C, g.Me));
        Game small = Lab(c => c.Pivot.StaffLossPermille = 1);
        Assert.Equal(1, Pivot.Leavers(small.C, small.Me));
        Game none = Lab(c => c.Pivot.StaffLossPermille = 0);
        Assert.Equal(1, Pivot.Leavers(none.C, none.Me));
        none.Me.Staff.Clear();
        Assert.Equal(0, Pivot.Leavers(none.C, none.Me));
    }

    [Fact]
    public void A_pivot_with_no_staff_still_goes_through()
    {
        Game g = Lab();
        g.Me.Staff.Clear();
        Assert.True(g.Do(Command.PivotStyle(g.Id, "wildcard")).Ok);
        Assert.Equal("wildcard", g.Me.StyleId);
    }

    [Fact]
    public void A_lab_with_no_style_can_pick_one()
    {
        Game g = Lab(null, null);
        Assert.Equal("", g.Me.StyleId);
        g.Do(Command.PivotStyle(g.Id, "sniper"));
        Assert.Equal("sniper", g.Me.StyleId);
        Assert.StartsWith("|", g.State.Events.Last(x => x.Kind == EventKind.StylePivoted).Ref);
    }

    [Fact]
    public void The_new_style_takes_over_the_focus_sectors_at_once()
    {
        Game g = Lab(null, "wildcard");
        int before = Founding.AppealPermille(g.C, g.Me, "cybersecurity");
        g.Do(Command.PivotStyle(g.Id, "sniper"));
        int after = Founding.AppealPermille(g.C, g.Me, "cybersecurity");
        Assert.True(before > after);
        Assert.Equal(g.C.Style("sniper").OtherAppealPermille + (Founding.FounderOf(g.C, g.Me) == null ? 0 : Founding.FounderOf(g.C, g.Me).AppealPermille), after);
    }

    // ---- customers

    [Fact]
    public void Customers_are_confused_for_a_year_and_then_it_passes()
    {
        Game g = Lab();
        ModelState m = Released(g);
        DisplayQueries.AppealBreakdown plain = Appeal(g, m);
        Assert.Equal(0, plain.PivotConfusion);
        g.Do(Command.PivotStyle(g.Id, "sniper".Equals(g.Me.StyleId) ? "shotgun" : "sniper"));
        Assert.Equal(g.State.Tick + 52, g.Me.PivotUntilTick);
        Assert.Equal(52, Pivot.ConfusionWeeksLeft(g.State, g.Me));
        DisplayQueries.AppealBreakdown confused = Appeal(g, m);
        Assert.True(confused.PivotConfusion < 0);
        Assert.Equal(confused.Total, confused.Core + confused.ReputationBoost + confused.SalesBoost + confused.RoleBoost + confused.TrustBoost + confused.ChampionBoost
            + confused.NamingConfusion + confused.ObjectiveEffect + confused.LaunchEffect + confused.PivotConfusion);
        g.Weeks(52);
        Assert.Equal(0, Pivot.ConfusionWeeksLeft(g.State, g.Me));
        Assert.Equal(1000, Pivot.AppealPermille(g.C, g.State, g.Me));
        Assert.Equal(0, Appeal(g, m).PivotConfusion);
    }

    [Fact]
    public void The_confusion_takes_the_configured_share_off()
    {
        Game g = Lab(c => { c.Pivot.ConfusionAppealPermille = 250; });
        Released(g);
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        Assert.Equal(750, Pivot.AppealPermille(g.C, g.State, g.Me));
    }

    // ---- once per era

    [Fact]
    public void A_second_pivot_in_the_same_era_is_refused_and_changes_nothing()
    {
        Game g = Lab();
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        Assert.True(Pivot.Spent(g.C, g.State, g.Me));
        g.Refused(Command.PivotStyle(g.Id, "wildcard"), CommandError.PivotSpent);
        g.Refused(Command.PivotStyle(g.Id, "sniper"), CommandError.PivotSpent);
        Assert.Equal("shotgun", g.Me.StyleId);
    }

    [Fact]
    public void The_next_era_gives_another_pivot()
    {
        Game g = Lab();
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        string era = g.Me.PivotEraId;
        int guard = 0;
        while (Pivot.EraId(g.C, g.State) == era && guard++ < 2000) g.Weeks(26);
        Assert.NotEqual(era, Pivot.EraId(g.C, g.State));
        g.Me.Cash = 1_000_000_000_000_000;
        Assert.False(Pivot.Spent(g.C, g.State, g.Me));
        Assert.True(g.Do(Command.PivotStyle(g.Id, "wildcard")).Ok);
        Assert.Equal("wildcard", g.Me.StyleId);
    }

    // ---- refusals

    [Fact]
    public void Bad_orders_are_refused_and_change_nothing()
    {
        Game g = Lab();
        g.Refused(Command.PivotStyle(g.Id, "dreamer"), CommandError.UnknownId);
        g.Refused(Command.PivotStyle(g.Id, ""), CommandError.UnknownId);
        g.Refused(Command.PivotStyle(g.Id, "sniper"), CommandError.InvalidArgument);
        g.Me.Cash = 10;
        g.Refused(Command.PivotStyle(g.Id, "shotgun"), CommandError.NotEnoughCash);
        Assert.Equal("sniper", g.Me.StyleId);
    }

    [Fact]
    public void Only_the_player_pivots()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string> { TestData.Config().Rivals[0].Id } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        string before = g.Sim.Digest();
        Assert.False(g.Sim.Apply(Command.PivotStyle(rival.Id, "sniper")).Ok);
        Assert.Equal(before, g.Sim.Digest());
    }

    // ---- nothing else moves

    [Fact]
    public void A_lab_that_never_pivots_is_exactly_as_it_was()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = Released(g);
        Released(twin);
        g.Weeks(40);
        twin.Weeks(40);
        Assert.Equal(g.Sim.Digest(), twin.Sim.Digest());
        Assert.Equal(0, Appeal(g, m).PivotConfusion);
        Assert.Equal("", g.Me.PivotEraId);
    }

    [Fact]
    public void Two_identical_labs_that_pivot_the_same_way_stay_identical()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = Released(g);
        Released(twin);
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        twin.Do(Command.PivotStyle(twin.Id, "shotgun"));
        g.Weeks(20);
        twin.Weeks(20);
        Assert.Equal(g.Sim.Digest(), twin.Sim.Digest());
        Assert.DoesNotContain(g.State.Events, x => x.Kind == EventKind.StylePivoted && x.LabId != g.Id);
        Assert.True(Appeal(g, m).Total > 0);
    }

    [Fact]
    public void The_chat_has_a_pivot_line()
    {
        Game g = Lab();
        Assert.Contains(g.C.Chat.Lines, l => l.Kind == "StylePivoted");
    }

    // ---- saving

    [Fact]
    public void A_pivot_survives_a_save_and_the_game_carries_on_the_same()
    {
        Game g = Lab();
        Released(g);
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        g.Weeks(6);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.PivotEraId, again.Player.PivotEraId);
        Assert.Equal(g.Me.PivotUntilTick, again.Player.PivotUntilTick);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(60);
        again.StepMany(60);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_nobody_having_pivoted()
    {
        Game g = Lab();
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        string old = SaveSchemaPretend.At(g.C, 37, g.Sim.Save());
        Assert.DoesNotContain("pivotEraId", old);
        Assert.DoesNotContain("pivotUntilTick", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => { Assert.Equal("", l.PivotEraId); Assert.Equal(0, l.PivotUntilTick); });
    }
}
