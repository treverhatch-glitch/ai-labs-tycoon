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

/// <summary>Public post-mortems: a write-up after an incident wins back part of the reputation it cost, a full one also tells the public the real alignment.</summary>
public class PostMortemTests
{
    static Game Lab(int safety = 2, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: safety, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 800;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        return m;
    }

    static void Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        g.C.Safety.IncidentChancePermille = 0;
    }

    static ModelState AfterIncident(Game g)
    {
        ModelState m = Selling(g);
        Incident(g, m);
        Assert.True(m.Incidents > 0);
        return m;
    }

    static List<GameEvent> Written(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.PostMortemPublished).ToList(); }

    // ---- the data

    [Fact]
    public void The_post_mortem_ships_and_the_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(6, c.PostMortem.WindowWeeks);
        Assert.Equal(20000, c.PostMortem.SummaryCost);
        Assert.Equal(50000, c.PostMortem.FullCost);
        Assert.Equal(300, c.PostMortem.SummaryRecoverPermille);
        Assert.Equal(600, c.PostMortem.FullRecoverPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.PostMortem.WindowWeeks = 60;
        c.PostMortem.FullCost = 10;
        c.PostMortem.FullRecoverPermille = 1200;
        c.PostMortem.MinSafetyStaff = -1;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("postMortem.windowWeeks"));
        Assert.Contains(problems, p => p.Contains("postMortem.fullCost"));
        Assert.Contains(problems, p => p.Contains("postMortem.fullRecoverPermille"));
        Assert.Contains(problems, p => p.Contains("postMortem.minSafetyStaff"));
    }

    [Fact]
    public void Nothing_is_written_up_until_the_player_does_it()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Weeks(20);
        Assert.Empty(Written(g));
        Assert.Equal(0, m.PostMortemDone);
        Assert.True(m.IncidentRepLoss > 0);
    }

    [Fact]
    public void An_incident_records_what_it_cost_and_clears_an_old_write_up()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        int before = g.Me.Reputation;
        Incident(g, m);
        Assert.True(m.IncidentRepLoss > 0);
        Assert.True(m.IncidentRepLoss <= before - g.Me.Reputation + 5);
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary));
        Assert.Equal(PostMortem.Summary, m.PostMortemDone);
        Incident(g, m);
        Assert.Equal(0, m.PostMortemDone);
        Assert.True(PostMortem.Open(g.C, g.State, m));
    }

    // ---- the command

    [Fact]
    public void A_summary_costs_cash_wins_back_a_share_of_the_loss_and_logs_it()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        int rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        int expect = (int)MathI.MulPermille(m.IncidentRepLoss, 300);
        Assert.Equal(expect, PostMortem.Recovery(g.C, m, PostMortem.Summary));
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary));
        Assert.Equal(rep + expect, g.Me.Reputation);
        Assert.Equal(cash - 20000, g.Me.Cash);
        Assert.Equal(PostMortem.Summary, m.PostMortemDone);
        GameEvent e = Assert.Single(Written(g));
        Assert.Equal(0, e.Pick);
        Assert.Equal(expect, e.Amount);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Contains("summary", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_full_write_up_wins_back_more_and_closes_the_gap_between_shown_and_real_alignment()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        m.DisplayedAlignment = m.Alignment + 300;
        Assert.True(SafetySystem.AlignmentGap(g.C, m) > 0);
        int rep = g.Me.Reputation;
        int expect = (int)MathI.MulPermille(m.IncidentRepLoss, 600);
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Assert.Equal(rep + expect, g.Me.Reputation);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        Assert.Equal(1, Assert.Single(Written(g)).Pick);
        Assert.Contains("full post-mortem", EventText.Describe(g.C, g.State, Written(g)[0]));
    }

    [Fact]
    public void A_summary_leaves_the_displayed_alignment_alone()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        m.DisplayedAlignment = m.Alignment + 300;
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary));
        Assert.Equal(m.Alignment + 300, m.DisplayedAlignment);
    }

    [Fact]
    public void The_full_write_up_costs_the_appeal_the_gap_was_borrowing()
    {
        Game honest = Lab();
        ModelState a = AfterIncident(honest);
        Game quiet = Lab();
        ModelState b = AfterIncident(quiet);
        a.DisplayedAlignment = a.Alignment + 400;
        b.DisplayedAlignment = b.Alignment + 400;
        SectorConfig sector = honest.C.Market.Sectors[0];
        long before = DisplayQueries.QualityParts(honest.C, sector, a, honest.State.Tick).SandbagBoost;
        Assert.True(before > 0);
        honest.Do(Command.PublishPostMortem(honest.Id, a.Id, PostMortem.Full));
        Assert.Equal(0, DisplayQueries.QualityParts(honest.C, sector, a, honest.State.Tick).SandbagBoost);
        Assert.Equal(before, DisplayQueries.QualityParts(quiet.C, sector, b, quiet.State.Tick).SandbagBoost);
    }

    [Fact]
    public void No_write_up_can_win_back_more_than_the_incident_cost()
    {
        Game g = Lab(tweak: c => { c.PostMortem.SummaryRecoverPermille = 1000; c.PostMortem.FullRecoverPermille = 1000; });
        ModelState m = AfterIncident(g);
        int lost = m.IncidentRepLoss;
        int after = g.Me.Reputation;
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Assert.Equal(after + lost, g.Me.Reputation);
        Assert.True(g.Me.Reputation <= 800);
    }

    [Fact]
    public void Reputation_never_goes_above_the_top()
    {
        Game g = Lab(tweak: c => c.PostMortem.FullRecoverPermille = 1000);
        ModelState m = AfterIncident(g);
        g.Me.Reputation = 999;
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Assert.Equal(1000, g.Me.Reputation);
    }

    [Fact]
    public void Each_incident_gets_one_write_up_and_a_second_one_is_refused_without_change()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary));
        string before = g.Sim.Digest();
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        Incident(g, m);
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Assert.Equal(2, Written(g).Count);
    }

    [Fact]
    public void The_window_closes_after_the_data_weeks()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Weeks(6);
        Assert.True(PostMortem.Open(g.C, g.State, m));
        g.Weeks(1);
        Assert.False(PostMortem.Open(g.C, g.State, m));
        string before = g.Sim.Digest();
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void A_model_with_no_incident_has_nothing_to_write_up()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary), CommandError.InvalidArgument);
        g.Refused(Command.PublishPostMortem(g.Id, 9999, PostMortem.Summary), CommandError.UnknownId);
    }

    [Fact]
    public void The_depth_must_be_a_summary_or_a_full_write_up()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, 3), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_lab_with_no_safety_people_cannot_write_one()
    {
        Game g = Lab(safety: 0);
        ModelState m = AfterIncident(g);
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary), CommandError.NotEnoughStaff);
        Assert.Equal(0, m.PostMortemDone);
    }

    [Fact]
    public void A_lab_that_cannot_pay_is_refused_and_nothing_changes()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Me.Cash = 19999;
        string before = g.Sim.Digest();
        g.Refused(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary), CommandError.NotEnoughCash);
        Assert.Equal(before, g.Sim.Digest());
        g.Me.Cash = 20000;
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Summary));
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void A_rival_lab_cannot_publish_and_a_game_with_no_window_has_none()
    {
        Game g = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        CommandResult r = g.Sim.Apply(Command.PublishPostMortem(rival.Id, 0, 1));
        Assert.False(r.Ok);
        Game off = Lab(tweak: c => c.PostMortem.WindowWeeks = 0);
        ModelState m = AfterIncident(off);
        Assert.False(PostMortem.Open(off.C, off.State, m));
        off.Refused(Command.PublishPostMortem(off.Id, m.Id, PostMortem.Summary), CommandError.InvalidArgument);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_write_up_survives_save_and_load_and_the_journal_replays()
    {
        Game g = Game.Solo(2, c => c.Safety.IncidentChancePermille = 0).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        g.C.Safety.IncidentChancePermille = 1000;
        for (int i = 0; i < 40 && m.Incidents == 0; i++) g.Weeks(1);
        g.C.Safety.IncidentChancePermille = 0;
        if (m.Incidents > 0 && PostMortem.Open(g.C, g.State, m) && g.Me.Cash >= 50000)
            g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Empty(SaveGame.Problems(back.Config, back.State));
    }

    [Fact]
    public void The_fields_round_trip_through_a_save()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        ModelState again = back.Player.Models.First(x => x.Id == m.Id);
        Assert.Equal(m.IncidentRepLoss, again.IncidentRepLoss);
        Assert.Equal(PostMortem.Full, again.PostMortemDone);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_write_ups()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        string old = SaveSchemaPretend.At(g.C, 57, g.Sim.Save());
        Assert.DoesNotContain("incidentRepLoss", old);
        Assert.DoesNotContain("postMortemDone", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs.SelectMany(l => l.Models), x => { Assert.Equal(0, x.IncidentRepLoss); Assert.Equal(0, x.PostMortemDone); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
        Assert.True(m.Incidents > 0);
    }

    [Fact]
    public void A_save_with_a_record_that_does_not_fit_is_reported()
    {
        Game g = Lab();
        ModelState m = AfterIncident(g);
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("post-mortem record"));
        m.IncidentRepLoss = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("post-mortem record"));
        m.IncidentRepLoss = 5;
        m.PostMortemDone = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("post-mortem record"));
        m.PostMortemDone = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("post-mortem record"));
    }

    // ---- a long game

    [Fact]
    public void A_long_game_with_write_ups_after_incidents_stays_sound()
    {
        Game g = Game.Solo(11, c => { c.Safety.IncidentChancePermille = 400; }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        int published = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            foreach (ModelState m in g.Me.Models.ToList())
                if (PostMortem.Open(g.C, g.State, m) && g.Me.Cash > 100000 && g.Try(Command.PublishPostMortem(g.Id, m.Id, 1 + i % 2)).Ok) published++;
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.True(published > 0);
        Assert.Equal(published, Written(g).Count);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
