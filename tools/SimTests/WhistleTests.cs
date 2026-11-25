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

/// <summary>The whistleblower: an overstated alignment and unhappy safety staff end in a concern that is answered, bought off or leaked.</summary>
public class WhistleTests
{
    static Game Shipped(Action<GameConfig> tweak = null, int safety = 2, int claim = 950)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Whistle.MoraleBelow = 1000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: safety, sales: 1, gpus: 8);
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, claim, "game-ai"));
        return g;
    }

    static ModelState Model(Game g) { return g.Me.Models.First(m => m.Status == ModelStatus.Released); }

    static Game Raised(Action<GameConfig> tweak = null)
    {
        Game g = Shipped(tweak);
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        return g;
    }

    static int Safety(Game g) { return LabQueries.StaffCount(g.Me, "safety"); }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Whistle.Enabled);
        Assert.Equal("safety", c.Whistle.RoleId);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Whistle.Enabled = 2;
        c.Whistle.RoleId = "nobody";
        c.Whistle.GapPermille = 0;
        c.Whistle.MoraleBelow = 1001;
        c.Whistle.WeeksToConcern = 0;
        c.Whistle.AnswerWeeks = 0;
        c.Whistle.CleanReputation = 501;
        c.Whistle.CleanMorale = 501;
        c.Whistle.SilenceFee = 0;
        c.Whistle.SilenceMorale = 501;
        c.Whistle.LeakReputation = 10;
        c.Whistle.LeakPerSilence = 201;
        c.Whistle.LeakMorale = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "roleId", "gapPermille", "moraleBelow", "weeksToConcern", "answerWeeks", "cleanReputation", "cleanMorale", "silenceFee", "silenceMorale", "leakReputation", "leakPerSilence", "leakMorale" })
            Assert.Contains(problems, p => p.Contains("whistle." + key));
    }

    [Fact]
    public void An_honest_lab_and_a_content_one_never_hear_from_anybody()
    {
        Game honest = Shipped(claim: 0);
        Assert.Equal(0, SafetySystem.AlignmentGap(honest.C, Model(honest)));
        honest.Weeks(40);
        Assert.Equal(0, honest.Me.WhistleWeeks);
        Assert.False(Whistle.Open(honest.Me));

        Game content = Shipped(c => c.Whistle.MoraleBelow = 450);
        Assert.True(content.Me.Staff.Where(p => p.RoleId == "safety").All(p => p.Morale > 450));
        content.Weeks(40);
        Assert.Equal(0, content.Me.WhistleWeeks);
        Assert.False(Whistle.Open(content.Me));

        Game nobody = Shipped(safety: 0);
        nobody.Weeks(40);
        Assert.Equal(0, nobody.Me.WhistleWeeks);
        Assert.Empty(Of(honest, EventKind.WhistleRaised));
    }

    [Fact]
    public void A_gap_and_an_unhappy_person_raise_a_concern_after_the_weeks_and_it_names_the_model_and_a_deadline()
    {
        Game g = Shipped();
        int weeks = g.C.Whistle.WeeksToConcern;
        g.Weeks(weeks - 1);
        Assert.Equal(weeks - 1, g.Me.WhistleWeeks);
        Assert.False(Whistle.Open(g.Me));
        int tick = g.State.Tick;
        g.Weeks(1);
        Assert.True(Whistle.Open(g.Me));
        Assert.Equal(Model(g).Id, g.Me.WhistleModel);
        Assert.Equal(tick + 1 + g.C.Whistle.AnswerWeeks, g.Me.WhistleDue);
        GameEvent e = Of(g, EventKind.WhistleRaised).Single();
        Assert.Equal(Model(g).Id, e.SubjectId);
        Assert.Equal(SafetySystem.AlignmentGap(g.C, Model(g)), e.Amount);
        Assert.Contains("shows", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_good_week_takes_one_back_off_the_count()
    {
        Game g = Shipped();
        g.Weeks(4);
        Assert.Equal(4, g.Me.WhistleWeeks);
        g.C.Whistle.MoraleBelow = 0;
        g.Weeks(2);
        Assert.Equal(2, g.Me.WhistleWeeks);
        g.C.Whistle.MoraleBelow = 1000;
        g.Weeks(1);
        Assert.Equal(3, g.Me.WhistleWeeks);
    }

    [Fact]
    public void Coming_clean_drops_the_shown_alignment_costs_reputation_and_cheers_the_safety_staff()
    {
        Game g = Raised(c => { c.Whistle.CleanMorale = 100; });
        foreach (StaffMember p in g.Me.Staff.Where(p => p.RoleId == "safety")) p.Morale = 400;
        int rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Clean));
        Assert.Equal(Model(g).Alignment, Model(g).DisplayedAlignment);
        Assert.Equal(rep - PressSystem.Damp(g.C, g.Me, 25), g.Me.Reputation);
        Assert.Equal(cash, g.Me.Cash);
        Assert.All(g.Me.Staff.Where(p => p.RoleId == "safety"), p => Assert.Equal(500, p.Morale));
        Assert.False(Whistle.Open(g.Me));
        Assert.Equal(0, g.Me.WhistleWeeks);
        Assert.Equal(0, g.Me.Silenced);
        GameEvent e = Of(g, EventKind.WhistleAnswered).Single();
        Assert.Equal(Whistle.Clean, e.Pick);
        g.Weeks(60);
        Assert.Empty(Of(g, EventKind.WhistleLeaked));
        Assert.Empty(Of(g, EventKind.WhistleRaised).Skip(1));
    }

    [Fact]
    public void Paying_for_silence_costs_more_each_time_and_leaves_the_gap_where_it_was()
    {
        Game g = Raised();
        int gap = SafetySystem.AlignmentGap(g.C, Model(g));
        long cash = g.Me.Cash;
        Assert.Equal(250000, Whistle.SilenceCost(g.C, g.Me));
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Silence));
        Assert.Equal(cash - 250000, g.Me.Cash);
        Assert.Equal(1, g.Me.Silenced);
        Assert.Equal(gap, SafetySystem.AlignmentGap(g.C, Model(g)));
        Assert.False(Whistle.Open(g.Me));
        Assert.Equal(0, g.Me.WhistleWeeks);
        Assert.Equal(500000, Whistle.SilenceCost(g.C, g.Me));
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        long again = g.Me.Cash;
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Silence));
        Assert.Equal(again - 500000, g.Me.Cash);
        Assert.Equal(2, g.Me.Silenced);
        Assert.Equal(60 + 2 * 30, Whistle.LeakReputationLoss(g.C, g.Me));
        Assert.Equal(Whistle.Silence, Of(g, EventKind.WhistleAnswered).Last().Pick);
    }

    [Fact]
    public void An_unanswered_concern_leaks_on_the_deadline_with_a_reputation_trust_and_staff_cost()
    {
        Game g = Raised();
        Game twin = Shipped(c => c.Whistle.Enabled = 0);
        twin.Weeks(g.C.Whistle.WeeksToConcern);
        int due = g.Me.WhistleDue;
        int people = Safety(g);
        g.Weeks(due - g.State.Tick - 1);
        Assert.True(Whistle.Open(g.Me));
        Assert.Empty(Of(g, EventKind.WhistleLeaked));
        twin.Weeks(due - twin.State.Tick - 1);
        int repBefore = g.Me.Reputation;
        g.Weeks(1);
        twin.Weeks(1);
        GameEvent e = Of(g, EventKind.WhistleLeaked).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(people - 1, Safety(g));
        Assert.True(g.Me.Reputation < repBefore);
        Assert.True(e.Amount >= PressSystem.Damp(g.C, g.Me, 60));
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") < TrustSystem.Of(twin.C, twin.Me, "game-ai"));
        Assert.Equal(Model(g).Alignment, Model(g).DisplayedAlignment);
        Assert.False(Whistle.Open(g.Me));
        Assert.Equal(0, g.Me.Silenced);
        Assert.Contains("leaked", EventText.Describe(g.C, g.State, e));
        Assert.Contains("resigned", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Silences_bought_before_a_leak_make_it_worse()
    {
        Game plain = Raised();
        Game bought = Raised();
        bought.Do(Command.AnswerWhistle(bought.Id, Whistle.Silence));
        bought.Weeks(bought.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(bought.Me));
        Assert.Equal(1, bought.Me.Silenced);
        Assert.Equal(60 + 30, Whistle.LeakReputationLoss(bought.C, bought.Me));
        Assert.Equal(60, Whistle.LeakReputationLoss(plain.C, plain.Me));
    }

    [Fact]
    public void The_concern_lapses_quietly_when_the_gap_closes_by_itself()
    {
        Game g = Raised();
        Model(g).DisplayedAlignment = Model(g).Alignment;
        g.Weeks(1);
        Assert.False(Whistle.Open(g.Me));
        Assert.Equal(0, g.Me.WhistleModel);
        g.Weeks(40);
        Assert.Empty(Of(g, EventKind.WhistleLeaked));
        Assert.Empty(Of(g, EventKind.WhistleAnswered));
    }

    [Fact]
    public void Refused_answers_leave_the_game_exactly_as_it_was()
    {
        Game none = Shipped();
        string d = none.Sim.Digest();
        none.Refused(Command.AnswerWhistle(none.Id, Whistle.Clean), CommandError.InvalidArgument);
        Assert.Equal(d, none.Sim.Digest());

        Game g = Raised();
        d = g.Sim.Digest();
        g.Refused(Command.AnswerWhistle(g.Id, 2), CommandError.InvalidArgument);
        g.Refused(Command.AnswerWhistle(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Me.Cash = 1000;
        d = g.Sim.Digest();
        g.Refused(Command.AnswerWhistle(g.Id, Whistle.Silence), CommandError.NotEnoughCash);
        Assert.Equal(d, g.Sim.Digest());
        Assert.True(Whistle.Open(g.Me));

        Game off = Shipped(c => c.Whistle.Enabled = 0);
        off.Weeks(30);
        Assert.False(Whistle.Open(off.Me));
        off.Refused(Command.AnswerWhistle(off.Id, Whistle.Clean), CommandError.InvalidArgument);

        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.AnswerWhistle(rival.Id, 0)).Ok);
    }

    [Fact]
    public void Save_and_load_mid_concern_play_on_the_same()
    {
        Game g = Raised();
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Silence));
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(g.Me.WhistleDue, lab.WhistleDue);
        Assert.Equal(g.Me.WhistleModel, lab.WhistleModel);
        Assert.Equal(1, lab.Silenced);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        for (int i = 0; i < 12; i++) { g.Weeks(1); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.NotEmpty(Of(g, EventKind.WhistleLeaked));
    }

    [Fact]
    public void An_older_save_loads_with_no_concern_open()
    {
        Game g = Raised();
        string old = SaveSchemaPretend.At(g.C, 65, g.Sim.Save());
        foreach (string key in new[] { "whistleWeeks", "whistleModel", "whistleDue", "silenced" }) Assert.DoesNotContain("\"" + key + "\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.WhistleDue); Assert.Equal(0, lab.WhistleWeeks); Assert.Equal(0, lab.Silenced); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_concern()
    {
        Game g = Raised();
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("whistleblower"));
        g.Me.WhistleModel = 9999;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("whistleblower"));
        g.Me.WhistleModel = Model(g).Id;
        g.Me.Silenced = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("whistleblower"));
        g.Me.Silenced = 0;
        g.Me.WhistleDue = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("whistleblower"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).Silenced = 1;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("whistleblower"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Shipped();
        g.Weeks(g.C.Whistle.WeeksToConcern);
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Silence));
        g.Weeks(g.C.Whistle.WeeksToConcern + g.C.Whistle.AnswerWeeks + 2);
        Assert.NotEmpty(Of(g, EventKind.WhistleLeaked));
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_of_buying_silence_and_leaking_stays_sound_and_the_books_balance()
    {
        Game g = Shipped(c => c.Whistle.WeeksToConcern = 4);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            if (Whistle.Open(g.Me) && g.Me.Silenced < 2) g.Try(Command.AnswerWhistle(g.Id, Whistle.Silence));
            if (Safety(g) == 0) g.Hire("safety", 1);
        }
        Assert.NotEmpty(Of(g, EventKind.WhistleLeaked));
        Assert.NotEmpty(Of(g, EventKind.WhistleAnswered));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
