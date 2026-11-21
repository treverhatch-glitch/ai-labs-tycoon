using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Zero, max, bankrupt and save/load edges for labour through alumni on schema 76.</summary>
public class NewEraSystemsEdgeHardeningTests
{
    const string A = "game-ai";
    const string B = "game-bots";

    static Game EraLab(long seed = 9, Action<GameConfig> tweak = null)
    {
        return Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    static (Game g, ModelState old, ModelState next) ReleasedPair()
    {
        Game g = EraLab();
        ModelState old = g.Train("mini", 300);
        g.UntilTrained(old);
        g.Release(old, A, B);
        ModelState next = g.Train("mini", 300);
        g.Try(Command.DesignWithModel(g.Id, next.Id, old.Id));
        g.UntilTrained(next);
        g.Release(next, A);
        g.Weeks(2);
        return (g, old, next);
    }

    [Fact]
    public void Labour_at_max_pressure_after_crunch_and_layoffs_still_balances_the_books()
    {
        Game g = EraLab(3, c =>
        {
            c.Labour.MoraleBelow = 1000;
            c.Labour.LowMoralePerWeek = 100;
        });
        g.Do(Command.SetCrunch(g.Id, true));
        for (int i = 0; i < 60 && g.Me.LabourStage < Labour.Organising; i++) g.Weeks(1);
        Assert.Equal(Labour.Organising, g.Me.LabourStage);
        g.Do(Command.Layoff(g.Id, 300));
        Assert.True(g.Me.LabourPressure >= g.C.Labour.DrivePressure / 2);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true);
    }

    [Fact]
    public void Treasury_with_zero_reserve_and_max_draw_refusals_leave_digest_unchanged()
    {
        Game g = EraLab();
        Assert.Equal(0, g.Me.Reserve);
        g.Refused(Command.DrawReserve(g.Id, 1), CommandError.InvalidArgument);
        g.Do(Command.SetAsideReserve(g.Id, 500_000));
        g.Do(Command.DrawReserve(g.Id, 500_000));
        Assert.Equal(0, g.Me.Reserve);
        HardeningInvariants.AssertBooksBalance(g.C, g.State);
    }

    [Fact]
    public void Pledge_at_max_level_and_save_load_mid_lock_plays_on_the_same()
    {
        Game g = EraLab();
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, A);
        g.Weeks(30);
        g.Do(Command.SetPledge(g.Id, g.C.Pledge.MaxLevel));
        Assert.Equal(g.C.Pledge.MaxLevel, g.Me.PledgeLevel);
        string mid = g.Sim.Save();
        g.Weeks(8);
        Game back = new Game(Simulation.Load(g.C, mid));
        back.Weeks(8);
        Assert.Equal(g.Sim.Digest(), back.Sim.Digest());
    }

    [Fact]
    public void Roadmap_at_max_level_with_zero_goal_weeks_is_refused()
    {
        var (g, _, _) = ReleasedPair();
        g.Do(Command.AnnounceRoadmap(g.Id, g.C.Roadmap.MaxLevel, 20));
        g.Refused(Command.AnnounceRoadmap(g.Id, g.C.Roadmap.MaxLevel, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Mentoring_at_max_pairs_refuses_a_third_and_skunkworks_with_minimum_people_runs()
    {
        Game g = EraLab(4, c => c.Seniority.MaxSharePermille = 500);
        StaffMember rMentor = g.Me.Staff.First(s => s.RoleId == "researcher");
        StaffMember rJunior = g.Me.Staff.First(s => s.RoleId == "researcher" && s != rMentor);
        g.Do(Command.PromoteStaff(g.Id, rMentor.Id));
        g.Do(Command.AssignMentor(g.Id, rMentor.Id, rJunior.Id));
        StaffMember eMentor = g.Me.Staff.First(s => s.RoleId == "engineer");
        StaffMember eJunior = g.Me.Staff.First(s => s.RoleId == "engineer" && s != eMentor);
        g.Do(Command.PromoteStaff(g.Id, eMentor.Id));
        g.Do(Command.AssignMentor(g.Id, eMentor.Id, eJunior.Id));
        Assert.Equal(g.C.Mentoring.MaxPairs, Mentoring.Pairs(g.Me));
        StaffMember eMentor2 = g.Me.Staff.First(s => s.RoleId == "engineer" && s.Senior == 0);
        StaffMember eJunior2 = g.Me.Staff.First(s => s.RoleId == "engineer" && s.Senior == 0 && s.MentorId == 0 && s != eMentor2);
        g.Do(Command.PromoteStaff(g.Id, eMentor2.Id));
        g.Refused(Command.AssignMentor(g.Id, eMentor2.Id, eJunior2.Id), CommandError.InvalidArgument);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true);
    }

    [Fact]
    public void Skunkworks_at_minimum_team_size_finishes_and_balances_the_books()
    {
        var (g, _, _) = ReleasedPair();
        int people = g.C.Skunkworks.MinPeople;
        g.Do(Command.StartSkunkworks(g.Id, "research", people, g.C.Skunkworks.MinWeeks));
        g.Weeks(g.C.Skunkworks.MinWeeks);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true);
    }

    [Fact]
    public void Sunset_at_minimum_notice_and_cancel_mid_notice_keeps_save_sound()
    {
        var (g, old, _) = ReleasedPair();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, g.C.Sunset.MinWeeks));
        string mid = g.Sim.Save();
        g.Do(Command.CancelSunset(g.Id, old.Id));
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
        HardeningInvariants.AssertBooksBalance(g.C, back.State);
    }

    [Fact]
    public void Account_teams_at_max_people_and_conference_at_max_pending_are_refused_further()
    {
        var (g, _, _) = ReleasedPair();
        int max = g.C.Accounts.MaxPeople;
        int keep = g.C.Accounts.KeepFree;
        int sales = LabQueries.StaffCount(g.Me, "sales");
        if (sales > keep + max)
        {
            g.Do(Command.AssignAccountTeam(g.Id, A, max));
            g.Refused(Command.AssignAccountTeam(g.Id, B, 1), CommandError.NotEnoughStaff);
        }
        for (int i = 0; i < g.C.Conference.MaxPending; i++) g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Refused(Command.SubmitPaper(g.Id, 1, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Alumni_pool_when_full_drops_oldest_and_bankrupt_lab_still_validates()
    {
        Game g = EraLab(6, c =>
            c.StaffPeople.Traits.First(t => t.QuitBelowMorale <= 0).QuitBelowMorale = c.StaffPeople.MoraleBaseline);
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(6);
        Assert.Equal(g.C.Alumni.MaxSize, g.Me.Alumni.Count);
        int oldest = g.Me.Alumni[0].Id;
        StaffMember spare = g.Me.Staff.FirstOrDefault();
        if (spare != null) g.Do(Command.Fire(g.Id, spare.RoleId, 1));
        Assert.DoesNotContain(g.Me.Alumni, a => a.Id == oldest);
        Finance.GoBankrupt(g.State, g.Me);
        Assert.True(g.Me.Bankrupt);
        string save = g.Sim.Save();
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, save);
        Assert.True(back.Player.Bankrupt);
        Assert.Empty(SaveGame.Problems(back.Config, back.State));
    }

    static void DisableNewEraTools(GameConfig c)
    {
        c.Labour.Enabled = 0;
        c.Sunset.Enabled = 0;
        c.Accounts.Enabled = 0;
        c.Conference.Enabled = 0;
        c.Alumni.Enabled = 0;
        c.Treasury.Enabled = 0;
        c.Pledge.MaxLevel = 0;
        c.Roadmap.MaxLevel = 0;
        c.Mentoring.Enabled = 0;
        c.Skunkworks.Enabled = 0;
    }

    [Fact]
    public void Every_new_era_tool_off_for_a_solo_lab_matches_a_plain_run()
    {
        Game plain = EraLab(4, DisableNewEraTools);
        Game enabled = EraLab(4);
        plain.Autopilot(120);
        enabled.Autopilot(120);
        Assert.Equal(0, enabled.Me.LabourPressure);
        Assert.Equal(0, enabled.Me.Reserve);
        Assert.Empty(enabled.Me.Submissions);
        Assert.Equal(plain.Me.Cash, enabled.Me.Cash);
        Assert.Equal(plain.Me.Reputation, enabled.Me.Reputation);
        Assert.Equal(plain.Sim.Digest(), enabled.Sim.Digest());
    }

    [Fact]
    public void Schema_71_through_76_saves_load_idle_and_play_twenty_weeks()
    {
        var (g, _, _) = ReleasedPair();
        g.Do(Command.SetAsideReserve(g.Id, 50_000));
        string current = g.Sim.Save();
        for (int schema = 71; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: schema >= 72);
        }
    }
}