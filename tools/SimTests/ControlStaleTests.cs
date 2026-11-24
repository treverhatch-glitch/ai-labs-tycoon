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

/// <summary>Control that goes stale: safety techs count for less for every era since the lab last did safety research; firing the listeners costs culture.</summary>
public class ControlStaleTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.RoomPerks.Clear();
            if (tweak != null) tweak(c);
        });
        g.Me.Cash = 1_000_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000_000;
        return g;
    }

    static int TickIn(GameConfig c, string eraId) { return EraSystem.StartTick(c, c.Era(eraId)) + 1; }

    static void Own(GameConfig c, LabState lab, string techId)
    {
        TechConfig tech = c.Tech(techId);
        foreach (string need in tech.Requires) if (!lab.Techs.Contains(need)) Own(c, lab, need);
        if (!lab.Techs.Contains(techId)) lab.Techs.Add(techId);
    }

    static StaffMember Person(Game g, string role)
    {
        var p = new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 };
        g.Me.Staff.Add(p);
        return p;
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

    // ---- the table

    [Fact]
    public void The_table_is_in_the_data_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Control.GraceEras);
        Assert.Equal(100, c.Control.LossPerEraPermille);
        Assert.Equal(500, c.Control.MaxLossPermille);
        Assert.Equal(15, c.Control.FiredListenerPoints);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 100)]
    [InlineData(3, 200)]
    [InlineData(6, 500)]
    [InlineData(7, 500)]
    [InlineData(40, 500)]
    public void The_loss_grows_by_a_tenth_for_every_era_past_the_grace_up_to_the_cap(int staleEras, int loss)
    {
        Assert.Equal(loss, ControlStale.LossFor(TestData.Config(), staleEras));
    }

    // ---- going stale

    [Fact]
    public void A_lab_with_no_safety_techs_has_nothing_to_go_stale()
    {
        Game g = Lab();
        ControlStale.Report r = ControlStale.Of(g.C, g.Me, TickIn(g.C, "upward"));
        Assert.Equal(-1, r.NewestSafetyEraIndex);
        Assert.Equal(0, r.StaleEras);
        Assert.Equal(0, r.LossPermille);
        Assert.Equal(0, r.BonusPermille);
    }

    [Theory]
    [InlineData("dial-up-days", 0)]
    [InlineData("punchline", 0)]
    [InlineData("it-sees", 100)]
    [InlineData("it-writes", 200)]
    [InlineData("chat-boom", 300)]
    [InlineData("upward", 500)]
    public void An_old_safety_tech_counts_for_less_each_era(string eraNow, int loss)
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        ControlStale.Report r = ControlStale.Of(g.C, g.Me, TickIn(g.C, eraNow));
        Assert.Equal(0, r.NewestSafetyEraIndex);
        Assert.Equal(g.C.Eras.ToList().FindIndex(e => e.Id == eraNow), r.StaleEras);
        Assert.Equal(loss, r.LossPermille);
        Assert.Equal(200, r.FreshBonusPermille);
        Assert.Equal(200 - 200 * loss / 1000, r.BonusPermille);
    }

    [Fact]
    public void New_safety_research_puts_every_older_safety_tech_back_to_full_strength()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        int now = TickIn(g.C, "chat-boom");
        Assert.Equal(300, ControlStale.Of(g.C, g.Me, now).LossPermille);
        Own(g.C, g.Me, "adversarial-tests");
        ControlStale.Report r = ControlStale.Of(g.C, g.Me, now);
        Assert.Equal(2, r.NewestSafetyEraIndex);
        Assert.Equal("adversarial-tests", r.NewestSafetyTechId);
        Assert.Equal(2, r.StaleEras);
        Assert.Equal(100, r.LossPermille);
        int again = TickIn(g.C, "it-sees");
        r = ControlStale.Of(g.C, g.Me, again);
        Assert.Equal(0, r.LossPermille);
        Assert.Equal(r.FreshBonusPermille, r.BonusPermille);
    }

    [Fact]
    public void A_tech_that_is_not_about_safety_does_not_refresh_anything()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        int now = TickIn(g.C, "chat-boom");
        TechConfig plain = g.C.Research.Techs.First(t => t.Era == "it-writes" && !LabQueries.IsSafetyTech(t));
        Own(g.C, g.Me, plain.Id);
        Assert.Equal(300, ControlStale.Of(g.C, g.Me, now).LossPermille);
    }

    [Fact]
    public void Rivals_never_go_stale()
    {
        Game g = Game.New(seed: 3);
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        Own(g.C, rival, "regression-tests");
        ControlStale.Report r = ControlStale.Of(g.C, rival, TickIn(g.C, "upward"));
        Assert.Equal(0, r.StaleEras);
        Assert.Equal(0, r.LossPermille);
        Assert.Equal(r.FreshBonusPermille, r.BonusPermille);
        Assert.Equal(200, r.BonusPermille);
    }

    [Fact]
    public void Staleness_is_a_view_of_the_techs_and_the_date_and_changes_nothing()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        string before = g.Sim.Digest();
        ControlStale.Of(g.C, g.Me, TickIn(g.C, "upward"));
        Assert.Equal(before, g.Sim.Digest());
    }

    // ---- what it does to the work

    [Fact]
    public void Stale_techs_slow_the_alignment_work_that_runs_use()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        int fresh = TrainingSystem.AlignmentWorkPermille(g.C, g.Me, TickIn(g.C, "dial-up-days"));
        int stale = TrainingSystem.AlignmentWorkPermille(g.C, g.Me, TickIn(g.C, "chat-boom"));
        Assert.Equal(fresh - 60, stale);
    }

    [Fact]
    public void A_rivals_alignment_work_does_not_move_with_the_date()
    {
        Game g = Game.New(seed: 3);
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        Own(g.C, rival, "regression-tests");
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(g.C, rival, TickIn(g.C, "dial-up-days")),
            TrainingSystem.AlignmentWorkPermille(g.C, rival, TickIn(g.C, "upward")));
    }

    [Fact]
    public void Switching_the_loss_off_in_the_data_gives_back_the_old_numbers()
    {
        Game g = Lab(c => c.Control.LossPerEraPermille = 0);
        g.Me.Techs.Add("regression-tests");
        Assert.Equal(1200, TrainingSystem.AlignmentWorkPermille(g.C, g.Me, TickIn(g.C, "upward")));
    }

    // ---- hearing about it

    [Fact]
    public void Each_era_that_costs_more_is_announced_until_the_cap_and_no_further()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        g.Until(() => g.Tick >= TickIn(g.C, "upward"), 3000, "the Upward era");
        List<GameEvent> stale = g.State.Events.Where(e => e.Kind == EventKind.ControlStale && e.LabId == g.Id).ToList();
        Assert.Equal(new long[] { 100, 200, 300, 400, 500 }, stale.Select(e => e.Amount).ToArray());
        Assert.All(stale, e => Assert.Equal("regression-tests", e.Ref));
    }

    [Fact]
    public void A_lab_with_no_safety_techs_hears_nothing()
    {
        Game g = Lab();
        g.Until(() => g.Tick >= TickIn(g.C, "chat-boom"), 3000, "the Chat Boom era");
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ControlStale);
    }

    [Fact]
    public void Researching_a_new_safety_tech_says_how_much_ground_it_won_back()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        g.Until(() => g.Tick >= TickIn(g.C, "it-writes"), 3000, "the It Writes era");
        Assert.Equal(200, ControlStale.LossNow(g.C, g.Me, g.State.Tick));
        foreach (string need in g.C.Tech("adversarial-tests").Requires) Own(g.C, g.Me, need);
        g.Research("adversarial-tests");
        GameEvent back = g.State.Events.Last(e => e.Kind == EventKind.ControlRefreshed);
        Assert.Equal(g.Id, back.LabId);
        Assert.Equal(200, back.Amount);
        Assert.Equal("adversarial-tests", back.Ref);
        Assert.Equal(0, ControlStale.LossNow(g.C, g.Me, g.State.Tick));
    }

    [Fact]
    public void Research_that_wins_nothing_back_is_not_announced()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        g.Research("playtest-telemetry");
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ControlRefreshed);
    }

    [Fact]
    public void The_staff_and_the_log_put_it_in_plain_words()
    {
        Game g = Lab();
        Person(g, "safety");
        g.Me.Techs.Add("regression-tests");
        g.Until(() => g.Tick >= TickIn(g.C, "it-sees"), 3000, "the It Sees era");
        GameEvent stale = g.State.Events.Last(e => e.Kind == EventKind.ControlStale);
        string text = EventText.Describe(g.C, g.State, stale);
        Assert.Contains("safety techniques are going stale", text);
        Assert.Contains("Regression Test Suites", text);
        Assert.Contains("90%", text);
        ChatMessage said = CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.ControlStale);
        Assert.Contains("Regression Test Suites", said.Text);

        foreach (string need in g.C.Tech("adversarial-tests").Requires) Own(g.C, g.Me, need);
        g.Research("adversarial-tests");
        GameEvent back = g.State.Events.Last(e => e.Kind == EventKind.ControlRefreshed);
        Assert.Contains("won back 10%", EventText.Describe(g.C, g.State, back));
        Assert.Contains("Adversarial Test Sets", CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.ControlRefreshed).Text);
    }

    // ---- letting the listeners go

    [Theory]
    [InlineData("safety", 2, 30)]
    [InlineData("red-teamer", 1, 15)]
    [InlineData("alignment-fellow", 3, 45)]
    [InlineData("researcher", 2, 0)]
    [InlineData("sales", 1, 0)]
    public void Letting_go_of_the_people_who_listen_costs_culture_and_nobody_else_does(string role, int count, int points)
    {
        Game g = Lab();
        for (int i = 0; i < count + 1; i++) Person(g, role);
        int before = SafetyCulture.Level(g.C, g.Me);
        g.Do(Command.Fire(g.Id, role, count));
        Assert.Equal(before - points, SafetyCulture.Level(g.C, g.Me));
    }

    [Fact]
    public void Letting_people_go_cannot_take_culture_below_nothing()
    {
        Game g = Lab();
        g.Me.CultureShift = -480;
        for (int i = 0; i < 6; i++) Person(g, "safety");
        g.Do(Command.Fire(g.Id, "safety", 6));
        Assert.Equal(0, SafetyCulture.Level(g.C, g.Me));
    }

    [Fact]
    public void A_refused_firing_costs_no_culture_and_changes_nothing()
    {
        Game g = Lab();
        Person(g, "safety");
        g.Refused(Command.Fire(g.Id, "safety", 5), CommandError.NotEnoughStaff);
    }

    // ---- saves, determinism, validation

    [Fact]
    public void A_stale_lab_saves_loads_and_plays_on_the_same()
    {
        Game g = Lab();
        g.Me.Techs.Add("regression-tests");
        g.Until(() => g.Tick >= TickIn(g.C, "it-writes"), 3000, "the It Writes era");
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(60);
        again.StepMany(60);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void The_validator_catches_a_broken_table()
    {
        AssertProblem(Problems(c => c.Control.GraceEras = -1), "control.graceEras");
        AssertProblem(Problems(c => c.Control.LossPerEraPermille = 1001), "control.lossPerEraPermille");
        AssertProblem(Problems(c => c.Control.MaxLossPermille = -5), "control.maxLossPermille");
        AssertProblem(Problems(c => c.Control.FiredListenerPoints = 1001), "control.firedListenerPoints");
    }
}
