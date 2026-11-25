using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class CreedTests
{
    static Game Lab(System.Action<GameConfig> more = null)
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
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static void Act(Game g, params EventKind[] kinds)
    {
        foreach (EventKind k in kinds) Events.Add(g.State, k, g.Id, 0, 0, "");
        g.State.Tick++;
        Creed.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Stating_values_starts_integrity_and_a_lock()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        Assert.Equal(Creed.Safety, g.Me.CreedA);
        Assert.Equal(Creed.Openness, g.Me.CreedB);
        Assert.Equal(g.C.Creed.StartIntegrity, g.Me.CreedIntegrity);
        Assert.Equal(g.C.Creed.FlipLockWeeks, g.Me.CreedLock);
        Assert.Equal(13, g.State.Events.Last(e => e.Kind == EventKind.CreedSet).SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Two_different_values_are_needed_and_the_lock_holds()
    {
        Game g = Lab();
        g.Refused(Command.SetCreed(g.Id, 0, 2), CommandError.InvalidArgument);
        g.Refused(Command.SetCreed(g.Id, 1, 6), CommandError.InvalidArgument);
        g.Refused(Command.SetCreed(g.Id, 3, 3), CommandError.InvalidArgument);
        g.Refused(Command.HoldTownHall(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.OwnUp(g.Id), CommandError.InvalidArgument);
        g.Do(Command.SetCreed(g.Id, 1, 2));
        g.Refused(Command.SetCreed(g.Id, 3, 4), CommandError.InvalidArgument);
        g.Weeks(g.C.Creed.FlipLockWeeks);
        g.Refused(Command.SetCreed(g.Id, 2, 1), CommandError.InvalidArgument);
        g.Do(Command.SetCreed(g.Id, 3, 4));
        Game off = Lab(c => c.Creed.Enabled = 0);
        off.Refused(Command.SetCreed(off.Id, 1, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void An_act_in_keeping_adds_and_an_act_against_costs_more()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Profit));
        Act(g, EventKind.RedTeamRan);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
        Assert.Equal(1, g.Me.CreedKept);
        Act(g, EventKind.FoundationEndowed);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints - g.C.Creed.BrokenPoints, g.Me.CreedIntegrity);
        Assert.Equal(1, g.Me.CreedBroken);
        Assert.True(g.C.Creed.BrokenPoints > g.C.Creed.KeptPoints);
    }

    [Fact]
    public void One_act_can_keep_one_value_and_break_another()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Speed));
        Act(g, EventKind.ModelRushed);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints - g.C.Creed.BrokenPoints, g.Me.CreedIntegrity);
        Assert.Equal(1, g.Me.CreedKept);
        Assert.Equal(1, g.Me.CreedBroken);
    }

    [Fact]
    public void Only_the_labs_own_acts_of_the_week_before_count_and_each_counts_once()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Profit));
        Events.Add(g.State, EventKind.RedTeamRan, g.Id + 77, 0, 0, "");
        g.State.Tick++;
        Creed.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Creed.StartIntegrity, g.Me.CreedIntegrity);
        Events.Add(g.State, EventKind.RedTeamRan, g.Id, 0, 0, "");
        Creed.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Creed.StartIntegrity, g.Me.CreedIntegrity);
        g.State.Tick++;
        Creed.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
        g.State.Tick++;
        Creed.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
    }

    [Fact]
    public void A_creed_that_is_lived_up_to_is_held_up_once()
    {
        Game g = Lab(c => { c.Creed.KeptPoints = 150; c.Creed.StartIntegrity = 700; });
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        int rep = g.Me.Reputation;
        int morale = g.Me.Staff[0].Morale;
        Act(g, EventKind.RedTeamRan);
        Assert.Equal(1, g.Me.CreedBand);
        Assert.Equal(rep + g.C.Creed.HighReputation, g.Me.Reputation);
        Assert.Equal(morale + g.C.Creed.HighMorale, g.Me.Staff[0].Morale);
        Act(g, EventKind.PaperPublished);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.CreedProud);
        Assert.Equal(rep + g.C.Creed.HighReputation, g.Me.Reputation);
    }

    [Fact]
    public void The_band_needs_a_gap_to_leave_and_can_be_crossed_again()
    {
        Game g = Lab(c => { c.Creed.KeptPoints = 150; c.Creed.StartIntegrity = 700; });
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        Act(g, EventKind.RedTeamRan);
        Assert.Equal(1, g.Me.CreedBand);
        Act(g, EventKind.ModelRushed);
        Assert.Equal(1, g.Me.CreedBand);
        Assert.True(g.Me.CreedIntegrity < g.C.Creed.HighBar);
        Act(g, EventKind.ModelRushed, EventKind.ModelRushed);
        Assert.Equal(0, g.Me.CreedBand);
        Act(g, EventKind.RedTeamRan, EventKind.PaperPublished, EventKind.PostMortemPublished);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.CreedProud));
    }

    [Fact]
    public void Breaking_the_values_makes_the_creed_a_joke_once_with_a_hit_to_reputation_and_morale()
    {
        Game g = Lab(c => { c.Creed.BrokenPoints = 150; c.Creed.StartIntegrity = 400; });
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        int rep = g.Me.Reputation;
        int morale = g.Me.Staff[0].Morale;
        Act(g, EventKind.ModelRushed);
        Assert.Equal(2, g.Me.CreedBand);
        int loss = PressSystem.Damp(g.C, g.Me, g.C.Creed.LowReputation);
        Assert.Equal(rep - loss, g.Me.Reputation);
        Assert.Equal(morale - g.C.Creed.LowMorale, g.Me.Staff[0].Morale);
        Assert.Equal(loss, g.State.Events.Last(e => e.Kind == EventKind.CreedHollow).Amount);
        Act(g, EventKind.ModelRushed);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.CreedHollow);
    }

    [Fact]
    public void A_town_hall_costs_cash_lifts_integrity_and_waits_but_not_while_the_creed_is_a_joke()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, 1, 3));
        long cash = g.Me.Cash;
        g.Do(Command.HoldTownHall(g.Id));
        Assert.Equal(cash - g.C.Creed.TownCost, g.Me.Cash);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.TownGain, g.Me.CreedIntegrity);
        g.Refused(Command.HoldTownHall(g.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Creed.TownCooldown);
        g.Do(Command.HoldTownHall(g.Id));
        g.Me.CreedIntegrity = 100;
        g.Me.CreedBand = 2;
        g.Weeks(g.C.Creed.TownCooldown);
        g.Refused(Command.HoldTownHall(g.Id), CommandError.InvalidArgument);
        g.Me.Cash = 0;
        g.Me.CreedBand = 0;
        g.Refused(Command.HoldTownHall(g.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Owning_up_costs_reputation_and_brings_the_creed_back_from_a_joke()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, 1, 3));
        g.Refused(Command.OwnUp(g.Id), CommandError.InvalidArgument);
        g.Me.CreedIntegrity = 100;
        g.Me.CreedBand = 2;
        int rep = g.Me.Reputation;
        g.Do(Command.OwnUp(g.Id));
        Assert.Equal(rep - g.C.Creed.OwnUpReputation, g.Me.Reputation);
        Assert.Equal(g.C.Creed.OwnUpRestore, g.Me.CreedIntegrity);
        Assert.Equal(0, g.Me.CreedBand);
        g.Refused(Command.OwnUp(g.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Changing_the_creed_starts_from_distrust_and_keeps_the_record()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, 1, 3));
        Act(g, EventKind.RedTeamRan);
        g.Weeks(g.C.Creed.FlipLockWeeks);
        g.Do(Command.SetCreed(g.Id, 4, 5));
        Assert.Equal(g.C.Creed.FlipIntegrity, g.Me.CreedIntegrity);
        Assert.True(g.Me.CreedKept >= 1);
        Assert.Equal(g.C.Creed.FlipLockWeeks, g.Me.CreedLock);
    }

    [Fact]
    public void A_live_creed_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, 1, 4));
        g.Do(Command.HoldTownHall(g.Id));
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(25);
        g.Weeks(25);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 96, g.Sim.Save());
        Assert.DoesNotContain("creedA", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).CreedA);
    }

    [Fact]
    public void A_rival_creed_and_bad_data_are_caught_and_the_config_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.CreedA = 1;
        rival.CreedB = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("creed"));
        rival.CreedA = 0;
        rival.CreedB = 0;
        g.Me.CreedA = 2;
        g.Me.CreedB = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("creed"));
        g.Me.CreedA = 0;
        g.Me.CreedB = 0;
        g.Me.CreedBand = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("creed"));
        g.Me.CreedBand = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Creed.Enabled = 2;
        c.Creed.KeptPoints = 2000;
        c.Creed.StartIntegrity = 2000;
        c.Creed.LowBar = 600;
        c.Creed.HighReputation = 900;
        c.Creed.TownGain = 2000;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "keptPoints", "startIntegrity", "lowBar", "highReputation", "townGain" })
            Assert.Contains(problems, p => p.StartsWith("creed.") && p.Contains(key));
    }
}
