using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ReplicationTests
{
    static Game Lab(int researchers = 4, System.Action<GameConfig> more = null)
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
        }).WithStarterLab(researchers: researchers, engineers: 2, safety: 1, sales: 1, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static void Note(Game g, EventKind kind, int pick, int labId = -2)
    {
        Events.Add(g.State, kind, labId == -2 ? g.Id : labId, 0, 0, "").Pick = pick;
        g.State.Tick++;
        Replication.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void A_paper_the_lab_publishes_comes_up_for_a_decision_about_its_code()
    {
        Game g = Lab();
        g.Do(Command.PublishPaper(g.Id));
        g.Weeks(2);
        Assert.Equal(1, g.Me.ReplPending);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void An_accepted_paper_counts_but_a_rejected_or_withdrawn_one_and_another_labs_do_not_and_there_is_a_limit()
    {
        Game g = Lab();
        Note(g, EventKind.PaperDecided, 1);
        Assert.Equal(1, g.Me.ReplPending);
        Note(g, EventKind.PaperDecided, 0);
        Note(g, EventKind.PaperDecided, 2);
        Note(g, EventKind.PaperPublished, 1, g.Id + 40);
        Assert.Equal(1, g.Me.ReplPending);
        for (int i = 0; i < 6; i++) Note(g, EventKind.PaperPublished, 1);
        Assert.Equal(g.C.Replication.PendingMax, g.Me.ReplPending);
    }

    [Fact]
    public void Releasing_needs_a_waiting_paper_a_sane_clean_up_and_the_points_and_one_at_a_time()
    {
        Game g = Lab();
        g.Refused(Command.ReleaseCode(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.WithholdCode(g.Id), CommandError.InvalidArgument);
        g.Me.ReplPending = 2;
        g.Refused(Command.ReleaseCode(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.ReleaseCode(g.Id, g.C.Replication.MaxEffort + 1), CommandError.InvalidArgument);
        g.Me.ResearchPoints = 1;
        g.Refused(Command.ReleaseCode(g.Id, 2), CommandError.MissingPrerequisite);
        g.Me.ResearchPoints = 1_000_000;
        g.Do(Command.ReleaseCode(g.Id, 2));
        g.Refused(Command.ReleaseCode(g.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.WithholdCode(g.Id));
        Game off = Lab(4, c => c.Replication.Enabled = 0);
        off.Me.ReplPending = 1;
        off.Refused(Command.ReleaseCode(off.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Releasing_pays_for_the_clean_up_and_sets_how_reproducible_the_paper_is()
    {
        Game g = Lab(4);
        g.Me.ReplPending = 1;
        long points = g.Me.ResearchPoints;
        g.Do(Command.ReleaseCode(g.Id, 2));
        Assert.Equal(points - 2 * g.C.Replication.EffortPoints, g.Me.ResearchPoints);
        int rigor = g.C.Replication.RigorBase + 2 * g.C.Replication.RigorStep + 4 * g.C.Replication.RigorPerWriter;
        Assert.Equal(rigor, g.Me.ReplRigor);
        Assert.Equal(g.C.Replication.Weeks, g.Me.ReplLeft);
        Assert.Equal(0, g.Me.ReplPending);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.CodeReleased);
        Assert.Equal(rigor, e.SubjectId);
        Assert.Equal(2, e.Pick);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_paper_that_is_reproducible_enough_replicates_and_adds_prestige()
    {
        Game g = Lab(4);
        g.Me.ReplPending = 1;
        g.Do(Command.ReleaseCode(g.Id, 3));
        int rep = g.Me.Reputation;
        Assert.True(g.Me.ReplRigor >= Replication.Bar(g.C, g.Me));
        g.Weeks(g.C.Replication.Weeks);
        Assert.Equal(g.C.Replication.WinPrestige, g.Me.Prestige);
        Assert.Equal(rep + g.C.Replication.WinReputation, g.Me.Reputation);
        Assert.Equal(1, g.Me.ReplWins);
        Assert.Equal(0, g.Me.ReplLeft);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.PaperReplicated).Pick);
    }

    [Fact]
    public void A_paper_that_is_not_reproducible_is_a_scandal()
    {
        Game g = Lab(1);
        g.Me.Prestige = 600;
        g.Me.ReplPending = 1;
        g.Do(Command.ReleaseCode(g.Id, 0));
        Assert.True(g.Me.ReplRigor < Replication.Bar(g.C, g.Me));
        int rep = g.Me.Reputation;
        int before = Prestige.Level(g.C, g.Me);
        g.Me.Prestige = before;
        for (int i = 0; i < g.C.Replication.Weeks; i++) Replication.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.ReplFails);
        Assert.Equal(before - g.C.Replication.LossPrestige, g.Me.Prestige);
        Assert.Equal(rep - PressSystem.Damp(g.C, g.Me, g.C.Replication.LossReputation), g.Me.Reputation);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.PaperReplicated).Pick);
        Assert.Equal(-g.C.Replication.LossPrestige, g.State.Events.Last(e => e.Kind == EventKind.PaperReplicated).Amount);
    }

    [Fact]
    public void A_famous_lab_is_checked_harder()
    {
        Game g = Lab(4);
        int none = Replication.Bar(g.C, g.Me);
        g.Me.Prestige = g.C.Prestige.Max;
        Assert.Equal(none + g.C.Replication.BarPerPrestige, Replication.Bar(g.C, g.Me));
        g.Me.Prestige = 0;
        Assert.True(Replication.Rigor(g.C, g.Me, 1) >= none);
        g.Me.Prestige = g.C.Prestige.Max;
        Assert.True(Replication.Rigor(g.C, g.Me, 0) < Replication.Bar(g.C, g.Me));
    }

    [Fact]
    public void Keeping_the_code_back_costs_some_prestige_and_never_below_nothing()
    {
        Game g = Lab();
        g.Me.Prestige = 100;
        g.Me.ReplPending = 2;
        g.Do(Command.WithholdCode(g.Id));
        Assert.Equal(100 - g.C.Replication.WithholdPrestige, g.Me.Prestige);
        Assert.Equal(1, g.Me.ReplPending);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.CodeWithheld);
        Assert.Equal(0, e.Pick);
        Assert.Equal(g.C.Replication.WithholdPrestige, e.Amount);
        g.Me.Prestige = 10;
        g.Do(Command.WithholdCode(g.Id));
        Assert.Equal(0, g.Me.Prestige);
        Assert.Equal(10, g.State.Events.Last(x => x.Kind == EventKind.CodeWithheld).Amount);
    }

    [Fact]
    public void A_paper_left_waiting_too_long_is_taken_to_have_been_kept_back()
    {
        Game g = Lab();
        g.Me.Prestige = 300;
        g.Me.ReplPending = 1;
        g.Weeks(g.C.Replication.PendingWeeks);
        Assert.Equal(1, g.Me.ReplPending);
        g.Weeks(1);
        Assert.Equal(0, g.Me.ReplPending);
        Assert.Equal(0, g.Me.ReplWait);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.CodeWithheld).Pick);
        Assert.True(g.Me.Prestige < 300);
    }

    [Fact]
    public void A_live_replication_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Me.ReplPending = 2;
        g.Do(Command.ReleaseCode(g.Id, 1));
        g.Weeks(5);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 98, g.Sim.Save());
        Assert.DoesNotContain("replPending", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).ReplPending);
    }

    [Fact]
    public void A_rival_paper_and_bad_data_are_caught_and_the_config_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.ReplWins = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("waiting on their code"));
        rival.ReplWins = 0;
        g.Me.ReplWait = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("waiting on their code"));
        g.Me.ReplWait = 0;
        g.Me.ReplRigor = 400;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("waiting on their code"));
        g.Me.ReplRigor = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Replication.Enabled = 2;
        c.Replication.Weeks = 0;
        c.Replication.RigorBase = -1;
        c.Replication.WinPrestige = -5;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "weeks", "rigorBase", "winPrestige" })
            Assert.Contains(problems, p => p.StartsWith("replication.") && p.Contains(key));
    }
}
