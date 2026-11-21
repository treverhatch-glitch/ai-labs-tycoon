using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ShocksTests
{
    static Game Lab(int capability = 800, System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Models.Clear();
        if (capability > 0) g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = capability, Status = ModelStatus.Released, Alignment = 500, Name = "Answer" });
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static int Index(Game g, string id) { return g.C.WorldEvents.FindIndex(e => e.Id == id); }

    static void Strike(Game g, string id) { Shocks.OnWorldEvent(g.C, g.State, Index(g, id)); }

    static int Morale(Game g) { return g.Me.Staff.Min(s => s.Morale); }

    [Fact]
    public void A_shock_lands_with_a_kind_a_window_and_an_event()
    {
        Game g = Lab();
        Strike(g, "talent-war");
        Assert.Equal(Shocks.Squeeze, g.Me.ShockKind);
        Assert.Equal(Index(g, "talent-war") + 1, g.Me.ShockRef);
        Assert.Equal(g.C.Shocks.WindowWeeks, g.Me.ShockLeft);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ShockStruck);
        Assert.Equal("talent-war", e.Ref);
        Assert.Equal(Shocks.Squeeze, e.SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_news_in_the_real_calendar_lands_on_its_date_and_only_big_news_counts()
    {
        Game g = Lab();
        g.State.Tick = EraSystem.EventTick(g.C, g.C.WorldEvents[Index(g, "talent-war")]);
        g.Weeks(1);
        Assert.Equal(Shocks.Squeeze, g.Me.ShockKind);
        Game quiet = Lab();
        quiet.State.Tick = EraSystem.EventTick(quiet.C, quiet.C.WorldEvents[Index(quiet, "dorm-network")]);
        quiet.Weeks(1);
        Assert.Equal(0, quiet.Me.ShockKind);
        Assert.Equal(Shocks.Boom, Shocks.KindOf("hundred-million"));
        Assert.Equal(Shocks.Backlash, Shocks.KindOf("pause-letter"));
        Assert.Equal(0, Shocks.KindOf("bubble-peak"));
    }

    [Fact]
    public void Only_a_waiting_shock_with_a_valid_answer_and_the_cash_can_be_answered()
    {
        Game g = Lab();
        g.Refused(Command.AnswerShock(g.Id, 2), CommandError.InvalidArgument);
        Strike(g, "talent-war");
        g.Refused(Command.AnswerShock(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.AnswerShock(g.Id, 4), CommandError.InvalidArgument);
        g.Me.Cash = 10;
        g.Refused(Command.AnswerShock(g.Id, 1), CommandError.NotEnoughCash);
        g.Do(Command.AnswerShock(g.Id, 2));
        Game off = Lab(800, c => c.Shocks.Enabled = 0);
        Strike(off, "talent-war");
        Assert.Equal(0, off.Me.ShockKind);
        off.Refused(Command.AnswerShock(off.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Riding_a_squeeze_costs_cash_and_morale()
    {
        Game g = Lab();
        Strike(g, "card-shortage");
        long cash = g.Me.Cash;
        int morale = Morale(g);
        g.Do(Command.AnswerShock(g.Id, 2));
        Assert.Equal(cash - g.Me.Staff.Count * (long)g.C.Shocks.HitCash, g.Me.Cash);
        Assert.Equal(morale - g.C.Shocks.HitMorale, Morale(g));
        Assert.Equal(500, g.Me.Reputation);
        Assert.Equal(1, g.Me.ShockRode);
        Assert.Equal(0, g.Me.ShockKind);
    }

    [Fact]
    public void Bracing_for_a_squeeze_pays_now_and_takes_a_fraction()
    {
        Game g = Lab();
        Strike(g, "card-shortage");
        long cash = g.Me.Cash;
        int morale = Morale(g);
        long staff = g.Me.Staff.Count;
        g.Do(Command.AnswerShock(g.Id, 1));
        long hit = MathI.MulPermille(staff * g.C.Shocks.HitCash, g.C.Shocks.BracePermille);
        Assert.Equal(cash - staff * g.C.Shocks.BraceCost - hit, g.Me.Cash);
        Assert.Equal(morale - (int)MathI.MulPermille(g.C.Shocks.HitMorale, g.C.Shocks.BracePermille), Morale(g));
        Assert.Equal(1, g.Me.ShockBraced);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Pick);
    }

    [Fact]
    public void A_backlash_ridden_out_costs_reputation_and_bracing_costs_cash_for_less()
    {
        Game ride = Lab();
        Strike(ride, "pause-letter");
        ride.Do(Command.AnswerShock(ride.Id, 2));
        int loss = PressSystem.Damp(ride.C, ride.Me, ride.C.Shocks.HitReputation);
        Assert.Equal(500 - loss, ride.Me.Reputation);
        Assert.Equal(-loss, ride.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Amount);
        Game brace = Lab();
        Strike(brace, "pause-letter");
        long cash = brace.Me.Cash;
        brace.Do(Command.AnswerShock(brace.Id, 1));
        Assert.True(brace.Me.Reputation > ride.Me.Reputation);
        Assert.True(brace.Me.Cash < cash);
    }

    [Fact]
    public void A_counter_works_with_a_good_enough_model_and_backfires_without()
    {
        Game good = Lab(800);
        Strike(good, "pause-letter");
        good.Do(Command.AnswerShock(good.Id, 3));
        Assert.Equal(500 + good.C.Shocks.CounterGain, good.Me.Reputation);
        Assert.Equal(3, good.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Pick);
        Assert.Equal(1, good.Me.ShockCountered);
        Game weak = Lab(100);
        Strike(weak, "pause-letter");
        weak.Do(Command.AnswerShock(weak.Id, 3));
        int loss = PressSystem.Damp(weak.C, weak.Me, (int)MathI.MulPermille(weak.C.Shocks.HitReputation, weak.C.Shocks.CounterMiss));
        Assert.Equal(500 - loss, weak.Me.Reputation);
        Assert.Equal(4, weak.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Pick);
        Game none = Lab(0);
        Strike(none, "talent-war");
        Assert.False(Shocks.CanCounter(none.C, none.Me));
    }

    [Fact]
    public void A_boom_ridden_gains_investing_gains_more_and_a_counter_is_a_bet()
    {
        Game ride = Lab();
        Strike(ride, "hundred-million");
        ride.Do(Command.AnswerShock(ride.Id, 2));
        Assert.Equal(500 + ride.C.Shocks.BoomReputation, ride.Me.Reputation);
        Game brace = Lab();
        Strike(brace, "hundred-million");
        long cash = brace.Me.Cash;
        brace.Do(Command.AnswerShock(brace.Id, 1));
        Assert.Equal(500 + (int)MathI.MulPermille(brace.C.Shocks.BoomReputation, brace.C.Shocks.BoomBrace), brace.Me.Reputation);
        Assert.Equal(cash - brace.Me.Staff.Count * (long)brace.C.Shocks.BraceCost, brace.Me.Cash);
        Game win = Lab(800);
        Strike(win, "hundred-million");
        win.Do(Command.AnswerShock(win.Id, 3));
        Assert.Equal(500 + (int)MathI.MulPermille(win.C.Shocks.BoomReputation, win.C.Shocks.CounterBoom), win.Me.Reputation);
        Game lose = Lab(100);
        Strike(lose, "hundred-million");
        lose.Do(Command.AnswerShock(lose.Id, 3));
        Assert.True(lose.Me.Reputation < 500);
    }

    [Fact]
    public void An_unanswered_shock_is_ridden_and_a_new_one_overrides_the_old()
    {
        Game g = Lab();
        Strike(g, "pause-letter");
        g.Weeks(g.C.Shocks.WindowWeeks);
        Assert.Equal(0, g.Me.ShockKind);
        Assert.Equal(5, g.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Pick);
        Assert.Equal(1, g.Me.ShockRode);
        Assert.True(g.Me.Reputation < 500);
        Strike(g, "talent-war");
        Strike(g, "hundred-million");
        Assert.Equal(Shocks.Boom, g.Me.ShockKind);
        Assert.Equal(2, g.Me.ShockRode);
        Assert.Equal(5, g.State.Events.Last(e => e.Kind == EventKind.ShockAnswered).Pick);
    }

    [Fact]
    public void A_live_shock_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        Strike(g, "talent-war");
        g.Weeks(3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(10);
        g.Weeks(10);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 97, g.Sim.Save());
        Assert.DoesNotContain("shockKind", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).ShockKind);
    }

    [Fact]
    public void A_rival_shock_and_bad_data_are_caught_and_the_config_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.ShockBraced = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("shock"));
        rival.ShockBraced = 0;
        g.Me.ShockKind = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("shock"));
        g.Me.ShockKind = 0;
        g.Me.ShockRef = 99999;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("shock"));
        g.Me.ShockRef = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Shocks.Enabled = 2;
        c.Shocks.WindowWeeks = 0;
        c.Shocks.HitReputation = 900;
        c.Shocks.BoomBrace = 500;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "windowWeeks", "hitReputation", "boomBrace" })
            Assert.Contains(problems, p => p.StartsWith("shocks.") && p.Contains(key));
    }
}
