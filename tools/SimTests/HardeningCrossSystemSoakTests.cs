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

/// <summary>All major player tools together across many seeds for a decade-plus, with economy and save invariants.</summary>
public class HardeningCrossSystemSoakTests
{
    const int TenYearsWeeks = 10 * 48;
    const int FullSoakWeeks = 520;

    static Game Start(long seed)
    {
        return Game.New(seed, c =>
        {
            c.Marketing.MinSales = 0;
            c.Chiefs.MinStaff = 0;
            c.Board.SafetyStaffPerLevel = 0;
            c.Optimise.EngineersPerLevel = 0;
            c.OnCall.StaffPerLevel = 0;
            c.Sla.MinReputation = 0;
            c.Compliance.StaffPerLevel = 0;
            c.Audit.MinSafetyStaff = 0;
            c.Hack.MinMorale = 0;
            c.Equity.MinStaff = 0;
            c.Whistle.MoraleBelow = 1000;
            c.RedTeam.RoleId = "";
            c.Listing.MinWeeklyRevenue = 1;
            c.Rbf.MinWeeklyRevenue = 1;
        });
    }

    static void Mentor(Game g, string role)
    {
        StaffMember mentor = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 1 && !Mentoring.IsMentor(g.Me, s));
        if (mentor == null)
        {
            StaffMember pick = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 0 && s.MentorId == 0);
            if (pick != null && g.Try(Command.PromoteStaff(g.Id, pick.Id)).Ok) mentor = pick;
        }
        StaffMember junior = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 0 && s.MentorId == 0 && s != mentor);
        if (mentor != null && junior != null) g.Try(Command.AssignMentor(g.Id, mentor.Id, junior.Id));
    }

    static void UseEveryTool(Game g, int week)
    {
        if (week == 8 || week == 120) g.Try(Command.SetEthicsBoard(g.Id, 1));
        if (week == 200) g.Try(Command.SetEthicsBoard(g.Id, 0));
        if (week == 12) g.Try(Command.SetOptimisation(g.Id, 2));
        if (week % 52 == 18 && g.Me.Cash > 300_000)
        {
            SectorState sector = g.State.Sectors.FirstOrDefault(s => s.Entry(g.Id) != null);
            if (sector != null) g.Try(Command.RunCampaign(g.Id, sector.Id, 2));
        }
        if (week % 55 == 22) g.Try(Command.SetOnCall(g.Id, week % 110 == 22 ? 1 : 0));
        if (week % 52 == 8 && g.Me.Cash > 500_000) g.Try(Command.SetResearchFocus(g.Id, new[] { "frontier", "alignment", "efficiency" }[week / 52 % 3]));
        if (week % 40 == 14) g.Try(Command.HoldHackWeek(g.Id));
        if (week == 90 && g.Me.Cash > 400_000) g.Try(Command.GrantStaffEquity(g.Id));
        if (week % 80 == 5) g.Try(Command.SetAsideReserve(g.Id, 80_000));
        if (week % 80 == 40) g.Try(Command.DrawReserve(g.Id, 40_000));
        if (week % 60 == 3) g.Try(Command.SetPledge(g.Id, 1 + week / 60 % 3));
        if (week % 70 == 12) g.Try(Command.AnnounceRoadmap(g.Id, 1 + week / 70 % 2, 20 + week % 25));
        if (week % 35 == 5) Mentor(g, week / 35 % 2 == 0 ? "researcher" : "engineer");
        if (week % 90 == 50)
        {
            StaffMember mentored = g.Me.Staff.FirstOrDefault(s => s.MentorId > 0);
            if (mentored != null) g.Try(Command.EndMentoring(g.Id, mentored.Id));
        }
        if (week % 45 == 20) g.Try(Command.StartSkunkworks(g.Id, new[] { "research", "safety", "engineering" }[week / 45 % 3], 2, 10));
        if (week % 45 == 32) g.Try(Command.WrapUpSkunkworks(g.Id));
        if (week % 95 == 60 && g.Me.Staff.Count >= 10) g.Try(Command.Layoff(g.Id, 120));
        if (week % 30 == 8)
        {
            ModelState released = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
            ModelState trained = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained);
            if (released != null && trained != null)
            {
                g.Try(Command.AnnounceSunset(g.Id, released.Id, 6 + week % 12));
                g.Try(Command.DesignWithModel(g.Id, trained.Id, released.Id));
            }
        }
        if (week % 50 == 15) g.Try(Command.AssignAccountTeam(g.Id, "game-ai", 1 + week % 2));
        if (week % 65 == 20 && g.Me.ResearchPoints >= 500) g.Try(Command.SubmitPaper(g.Id, 1 + week / 65 % 3, 1 + week % 3));
        if (week % 90 == 35) g.Try(Command.PublishPaper(g.Id));
        if (week % 110 == 80 && g.Me.Alumni.Count > 0) g.Try(Command.RehireAlumnus(g.Id, g.Me.Alumni[0].Id));
        if (week % 45 == 10) g.Try(Command.SetCrunch(g.Id, true));
        if (week % 45 == 16) g.Try(Command.SetCrunch(g.Id, false));
        if (Whistle.Open(g.Me)) g.Try(Command.AnswerWhistle(g.Id, Whistle.Clean));
        if (week % 55 == 7) g.Try(Command.StartInternships(g.Id, "safety", 1));
        if (week % 55 == 20) g.Try(Command.EndInternships(g.Id));
        if (week % 48 == 11) g.Try(Command.StaffSupportDesk(g.Id, week % 96 == 11 ? 2 : 1));
        if (week % 70 == 25)
        {
            StaffMember away = g.Me.Staff.FirstOrDefault(s => s.RoleId == "safety" && s.LeaveDue == 0);
            if (away != null) g.Try(Command.SendOnSabbatical(g.Id, away.Id));
        }
        if (week % 85 == 30) g.Try(Command.OpenRequisition(g.Id, "researcher", 1));
        if (week % 100 == 45) g.Try(Command.HostScholars(g.Id, 1));
        if (week % 60 == 7) g.Try(Command.BuildSecurity(g.Id));
        if (week % 70 == 14) g.Try(Command.AdoptPolicy(g.Id, 1 + week / 70 % 2));
        if (week % 75 == 20 && GovContracts.Offered(g.Me)) g.Try(Command.AcceptContract(g.Id));
        if (week % 80 == 25) g.Try(Command.ConvertCharter(g.Id, week % 160 == 25 ? Charter.Capped : Charter.ForProfit));
        foreach (LawCase claim in g.Me.Cases.ToList())
            g.Try(week % 2 == 0 ? Command.SettleCase(g.Id, claim.Id) : Command.RetainFirm(g.Id, claim.Id));
        if (week % 52 == 6 && g.Me.StdDraft > 0) g.Try(Command.TakeStandardsRole(g.Id, Standards.Member));
        if (week % 90 == 18 && g.Me.StdLevel > 0) g.Try(Command.AdoptStandards(g.Id));
        if (week % 50 == 7) g.Try(Command.SetWarranty(g.Id, week % 100 == 7 ? 1 : 0));
        if (week % 80 == 30 && Loans.Outstanding(g.Me) == 0) g.Try(Command.TakeLoan(g.Id, 500));
        if (Covenants.InBreach(g.C, g.State, g.Me)) g.Try(week % 2 == 0 ? Command.WaiveCovenant(g.Id) : Command.CureCovenant(g.Id));
        if (week % 60 == 15) g.Try(Command.SetCapitalPolicy(g.Id, week % 120 == 15 ? CapitalAllowances.Capitalise : CapitalAllowances.Expense));
        if (week % 100 == 22 && !Listing.Listed(g.Me) && g.Me.Reputation >= g.C.Listing.MinReputation && Listing.Revenue(g.Me) >= g.C.Listing.MinWeeklyRevenue)
            g.Try(Command.ListCompany(g.Id));
        if (week % 85 == 33 && Listing.Listed(g.Me) && Listing.Revenue(g.Me) >= g.C.Rbf.MinWeeklyRevenue) g.Try(Command.TakeRevenueAdvance(g.Id, 1, 400));
        if (week % 70 == 40) g.Try(Command.EndowFoundation(g.Id, 500, Foundation.Safety));
        if (week % 95 == 12) g.Try(Command.SaleLeaseback(g.Id, 2));
        if (week % 110 == 55 && g.Me.NoteOwed == 0) g.Try(Command.IssueNote(g.Id, 1, 300));
    }

    static Game Soak(long seed, int weeks)
    {
        Game g = Start(seed);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(48);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseEveryTool(g, week);
            g.Autopilot(1);
            HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true);
            Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        }
        return g;
    }

    [Theory]
    [InlineData(7)]
    [InlineData(13)]
    [InlineData(19)]
    [InlineData(31)]
    [InlineData(47)]
    public void Ten_years_with_every_tool_keeps_invariants(int seed)
    {
        Game g = Soak(seed, TenYearsWeeks);
        Assert.False(double.IsNaN(g.Me.Cash));
        Assert.True(LabQueries.TotalStaff(g.Me) >= 0);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Theory]
    [InlineData(11)]
    [InlineData(23)]
  [InlineData(37)]
    public void Five_hundred_twenty_weeks_with_every_tool_stays_sound(int seed)
    {
        Game g = Soak(seed, FullSoakWeeks);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.InRange(g.State.BubblePermille, 0, MathI.Permille);
        Assert.True(g.State.Events.Count <= g.C.EventLogMax);
    }

    [Fact]
    public void A_mid_soak_save_load_and_journal_replay_match()
    {
        Game g = Soak(29, 200);
        string mid = g.Sim.Save();
        Game loaded = new Game(Simulation.Load(g.C, mid));
        loaded.Autopilot(30);
        Simulation replay = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), replay.Digest());
    }
}