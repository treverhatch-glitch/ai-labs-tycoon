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

/// <summary>Scrapping a training run: the spent compute is gone, part of it comes back as research points, the press and the team mind by how far it got.</summary>
public class AbandonTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 600;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        return g;
    }

    static ModelState Run(Game g, int progressPermille)
    {
        ModelState m = g.Train("mini", 0);
        m.ProgressCompute = m.RequiredCompute * progressPermille / 1000;
        return m;
    }

    static List<GameEvent> Scrapped(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.RunScrapped).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Abandon.Enabled);
        Assert.Equal(200, c.Abandon.SalvagePermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Abandon.Enabled = 2;
        c.Abandon.SalvagePermille = 501;
        c.Abandon.ReputationAtFull = -1;
        c.Abandon.MoraleAtFull = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "salvagePermille", "reputationAtFull", "moraleAtFull" })
            Assert.Contains(problems, p => p.Contains("abandon." + key));
    }

    [Fact]
    public void Nothing_is_ever_scrapped_by_itself()
    {
        Game g = Lab();
        ModelState m = Run(g, 100);
        g.Weeks(3);
        Assert.Empty(Scrapped(g));
        Assert.Contains(g.Me.Models, x => x.Id == m.Id);
    }

    [Fact]
    public void The_helpers_report_progress_salvage_and_the_losses()
    {
        Game g = Lab();
        ModelState m = Run(g, 400);
        Assert.Equal(400, Abandon.ProgressPermille(m));
        Assert.Equal(m.ProgressCompute * 200 / 1000, Abandon.Salvage(g.C, m));
        Assert.Equal(8, Abandon.ReputationLoss(g.C, m));
        Assert.Equal(24, Abandon.MoraleLoss(g.C, m));
        m.ProgressCompute = m.RequiredCompute * 3;
        Assert.Equal(1000, Abandon.ProgressPermille(m));
        m.RequiredCompute = 0;
        Assert.Equal(0, Abandon.ProgressPermille(m));
    }

    [Fact]
    public void Scrapping_removes_the_run_keeps_a_share_as_research_and_logs_it()
    {
        Game g = Lab();
        ModelState m = Run(g, 600);
        long points = g.Me.ResearchPoints;
        long keep = Abandon.Salvage(g.C, m);
        Assert.True(keep > 0);
        int runs = g.Me.Models.Count;
        g.Do(Command.ScrapRun(g.Id, m.Id));
        Assert.Equal(runs - 1, g.Me.Models.Count);
        Assert.DoesNotContain(g.Me.Models, x => x.Id == m.Id);
        Assert.Equal(points + keep, g.Me.ResearchPoints);
        GameEvent e = Assert.Single(Scrapped(g));
        Assert.Equal(1, e.Pick);
        Assert.Equal(keep, e.Amount);
        Assert.Equal(m.Name, e.Ref);
        Assert.Contains("well into it", EventText.Describe(g.C, g.State, e));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_press_and_the_team_mind_in_proportion_to_progress()
    {
        Game early = Lab();
        ModelState a = Run(early, 100);
        int repA = early.Me.Reputation;
        early.Do(Command.ScrapRun(early.Id, a.Id));
        Game late = Lab();
        ModelState b = Run(late, 900);
        int repB = late.Me.Reputation;
        late.Do(Command.ScrapRun(late.Id, b.Id));
        int earlyDrop = repA - early.Me.Reputation;
        int lateDrop = repB - late.Me.Reputation;
        Assert.Equal(2, earlyDrop);
        Assert.Equal(18, lateDrop);
        Assert.All(early.Me.Staff, p => Assert.Equal(700 - 6, p.Morale));
        Assert.All(late.Me.Staff, p => Assert.Equal(700 - 54, p.Morale));
        Assert.Equal(0, Assert.Single(Scrapped(early)).Pick);
    }

    [Fact]
    public void Reputation_and_morale_never_go_below_zero()
    {
        Game g = Lab();
        ModelState m = Run(g, 1000);
        g.Me.Reputation = 3;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 10;
        g.Do(Command.ScrapRun(g.Id, m.Id));
        Assert.Equal(0, g.Me.Reputation);
        Assert.All(g.Me.Staff, p => Assert.Equal(0, p.Morale));
    }

    [Fact]
    public void Scrapping_a_run_at_the_start_costs_nothing_and_frees_the_slot()
    {
        Game g = Lab(c => c.Training.MaxConcurrentRuns = 1);
        ModelState m = g.Train("mini", 0);
        g.Refused(Command.StartTraining(g.Id, "mini", 0), CommandError.TooManyRuns);
        int rep = g.Me.Reputation;
        g.Do(Command.ScrapRun(g.Id, m.Id));
        Assert.Equal(rep, g.Me.Reputation);
        Assert.Equal(0, LabQueries.ActiveRuns(g.Me));
        ModelState again = g.Train("mini", 0);
        Assert.Equal(1, again.Version);
    }

    [Fact]
    public void Only_a_run_that_is_still_training_can_be_scrapped()
    {
        Game g = Lab();
        ModelState done = g.Train("mini", 0);
        g.UntilTrained(done);
        string before = g.Sim.Digest();
        g.Refused(Command.ScrapRun(g.Id, done.Id), CommandError.WrongModelStatus);
        g.Do(Command.ReleaseModel(g.Id, done.Id, 0, "game-ai", "game-bots"));
        g.Refused(Command.ScrapRun(g.Id, done.Id), CommandError.WrongModelStatus);
        Assert.NotEqual("", before);
        g.Refused(Command.ScrapRun(g.Id, 9999), CommandError.UnknownId);
        Assert.Contains(g.Me.Models, x => x.Id == done.Id);
    }

    [Fact]
    public void A_refusal_changes_nothing()
    {
        Game g = Lab();
        ModelState done = g.Train("mini", 0);
        g.UntilTrained(done);
        string before = g.Sim.Digest();
        g.Refused(Command.ScrapRun(g.Id, done.Id), CommandError.WrongModelStatus);
        g.Refused(Command.ScrapRun(g.Id, 9999), CommandError.UnknownId);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_it_switched_off_cannot_scrap()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.ScrapRun(rival.Id, 0)).Ok);
        Game off = Lab(c => c.Abandon.Enabled = 0);
        ModelState m = Run(off, 300);
        off.Refused(Command.ScrapRun(off.Id, m.Id), CommandError.InvalidArgument);
        Assert.Contains(off.Me.Models, x => x.Id == m.Id);
    }

    [Fact]
    public void Scrapping_never_pays_better_than_finishing()
    {
        Game g = Lab();
        ModelState m = Run(g, 1000);
        Assert.True(Abandon.Salvage(g.C, m) <= m.ProgressCompute / 2);
    }

    [Fact]
    public void Salvaged_points_can_buy_a_paper()
    {
        Game g = Lab();
        g.Me.ResearchPoints = 0;
        ModelState m = Run(g, 1000);
        g.Do(Command.ScrapRun(g.Id, m.Id));
        Assert.True(g.Me.ResearchPoints > 0);
    }

    [Fact]
    public void A_scrapped_run_leaves_no_trace_in_the_sectors_and_the_game_plays_on()
    {
        Game g = Lab();
        ModelState m = Run(g, 500);
        g.Do(Command.ScrapRun(g.Id, m.Id));
        g.Weeks(40);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.All(g.State.Sectors, s => Assert.All(s.Entries, e => Assert.NotEqual(m.Id, e.ModelId)));
    }

    [Fact]
    public void Save_and_load_after_a_scrap_play_on_the_same()
    {
        Game g = Lab();
        ModelState m = Run(g, 500);
        g.Do(Command.ScrapRun(g.Id, m.Id));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        ModelState m = g.Train("mini", 0);
        g.Weeks(5);
        g.Do(Command.ScrapRun(g.Id, m.Id));
        ModelState n = g.Train("mini", 0);
        g.UntilTrained(n);
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_runs_scrapped_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11).WithStarterLab(researchers: 3, engineers: 4, safety: 2, gpus: 8);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int scrapped = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 37 == 20)
            {
                ModelState run = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Training);
                if (run != null && g.Try(Command.ScrapRun(g.Id, run.Id)).Ok) scrapped++;
            }
        }
        Assert.True(scrapped > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
