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
        if (Whistle.Open(g.Me) && Whistle.Worst(g.C, g.Me) != null) g.Try(Command.AnswerWhistle(g.Id, Whistle.Clean));
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
        if (week == 96 && g.Me.CreedA == 0) g.Try(Command.SetCreed(g.Id, Creed.Profit, Creed.Openness));
        if (week % 140 == 20 && g.Me.Models.Any(m => m.Status == ModelStatus.Released) && g.Me.Reputation >= g.C.Platforms.MinReputation && g.Me.PlatPlat == 0)
            g.Try(Command.SignPlatform(g.Id, 1));
        if (week % 150 == 25 && Devices.EraNow(g.C, g.State) >= g.C.Devices.EraA && LabQueries.BestReleased(g.Me) != null && !Devices.Active(g.Me))
            g.Try(Command.DesignDevice(g.Id, 1, 0));
        if (week % 90 == 20 && Shocks.Pending(g.Me)) g.Try(Command.AnswerShock(g.Id, 2));
        if (week % 100 == 60 && g.Me.ReplPending > 0 && !Replication.Running(g.Me)) g.Try(Command.ReleaseCode(g.Id, 0));
        if (week == 180 && g.Me.BrandOn == 0) g.Try(Command.OpenCareersPage(g.Id));
        if (week % 160 == 12 && g.Me.TbRef == 0) g.Try(Command.BetOnTech(g.Id, "regression-tests"));
        ModelState training = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Training || m.Status == ModelStatus.Trained);
        if (week % 170 == 18 && training != null && g.Me.WlModel == 0) g.Try(Command.OpenWaitlist(g.Id, training.Id, 12));
        if (week % 175 == 22 && g.Me.FtLevel == 0) g.Try(Command.OpenFreeTier(g.Id, 1));
        if (week == 190 && g.Me.LwOn == 0) g.Try(Command.OpenWatchDesk(g.Id));
        if (week == 210 && g.Me.PgTech == 0 && g.Me.ResearchPoints >= 500) g.Try(Command.StartProgramme(g.Id, "beowulf-clusters"));
        if (week % 165 == 5 && g.Me.PgPhase == Programmes.Choosing) g.Try(Command.ChooseApproach(g.Id, 1));
        ModelState run = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Training);
        if (week % 172 == 8 && run != null && RunQueue.Level(run) == 0) g.Try(Command.SetRunPriority(g.Id, run.Id, 2));
        if (week == 220 && g.Me.PodIds.Count == 0 && g.Me.Staff.Count >= 2)
            g.Try(Command.FormPod(g.Id, g.Me.Staff[0].Id, g.Me.Staff[1].Id));
        if (week % 180 == 10 && g.Me.LicRegime == 0) g.Try(Command.ApplyLicence(g.Id, 1));
        ModelState sold = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
        if (week % 190 == 15 && sold != null) g.Try(Command.PledgeSupport(g.Id, sold.Id, 26));
        if (week >= 300)
        {
            ModelState apiReady = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
            if (week % 40 == 5 && apiReady != null && apiReady.ApiState == 0)
                g.Try(Command.OpenApi(g.Id, apiReady.Id));
            if (week % 45 == 2 && g.Me.OrgForm == 0) g.Try(Command.SetOrg(g.Id, 1 + week / 45 % 3));
            if (week % 50 == 8 && g.Me.FabTier == 0)
            {
                g.C.Fabric.EraA = 0;
                g.C.Fabric.EraB = 0;
                g.C.Fabric.EraC = 0;
                g.Try(Command.OrderFabric(g.Id));
            }
            if (week % 80 == 1 && g.Me.DiscOn == 0) g.Try(Command.JoinRegister(g.Id));
            ModelState cert = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released && m.CertStage == 0);
            if (week % 70 == 20 && cert != null && g.Me.CfGranted < g.C.Conformity.MaxActive)
            {
                cert.Alignment = 800;
                cert.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
                g.Try(Command.StartConformity(g.Id, cert.Id, 1));
            }
        }
        if (week >= 320)
        {
            if (week % 25 == 3) g.Try(Command.AcquireData(g.Id, 1 + week / 25 % 3, 30));
            if (week % 28 == 4) g.Try(Command.CleanData(g.Id, 1, 20));
            if (week % 32 == 6 && g.Me.Models.Any(m => m.Status == ModelStatus.Released) && g.Me.UserPolicy == 0)
                g.Try(Command.SetUserData(g.Id, 1));
            if (week == 330 && g.Me.GovForm == 0)
            {
                foreach (FundingRoundConfig r in g.C.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
                g.Try(Command.RaiseFunding(g.Id, "angel"));
                g.Try(Command.SetGovernance(g.Id, 1));
            }
            if (week % 35 == 9 && g.Me.SpotPermille == 0) g.Try(Command.SetSpotShare(g.Id, 200));
        }
        if (week >= 360)
        {
            g.C.PostTraining.MinEra = 0;
            g.C.Agency.MinEra = 0;
            if (g.Me.BoardInvestorSeats == 0) g.Me.BoardInvestorSeats = 1;
            if (week % 30 == 7 && g.Me.AnnVendor == 0 && g.Me.DataClean1 > 0)
                g.Try(Command.ContractAnnotators(g.Id, 1 + week / 30 % 3));
            if (week % 40 == 11 && g.Me.AnnVendor != 0) g.Try(Command.AuditVendor(g.Id));
            ModelState tune = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained && m.PostKind == 0);
            if (week % 45 == 13 && tune != null && g.Me.DataClean4 >= g.C.PostTraining.Units)
                g.Try(Command.PostTrain(g.Id, tune.Id, 1 + week / 45 % 2));
            ModelState owned = g.Me.Models.FirstOrDefault(m => m.Status != ModelStatus.Retired && m.OwnerId == 0);
            StaffMember researcher = g.Me.Staff.FirstOrDefault(s => s.RoleId == "researcher");
            if (week % 50 == 17 && owned != null && researcher != null)
            {
                g.Try(Command.AssignOwner(g.Id, owned.Id, researcher.Id));
                g.Try(Command.WriteDocs(g.Id, owned.Id));
            }
            if (week % 55 == 19 && g.Me.MeetingsOn == 0) g.Try(Command.HoldBoardMeetings(g.Id, 1));
            if (week % 52 == 21 && g.Me.MotionPend != 0) g.Try(Command.AnswerMotion(g.Id, week % 104 == 21 ? 1 : 0));
        }
        if (week >= 400)
        {
            g.C.Drills.MinEra = 0;
            g.C.EvalHarness.MinEra = 0;
            g.C.ToolPolicy.MinEra = 0;
            g.C.HumanQueue.MinEra = 0;
            g.C.ReleaseChecklist.MinEra = 0;
            if (week % 30 == 4 && g.Me.DrillLeft == 0) g.Try(Command.StartDrill(g.Id));
            ModelState trained = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained);
            ModelState released = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
            if (week % 35 == 6 && trained != null && trained.ToolMask == 0)
                g.Try(Command.SetToolPolicy(g.Id, trained.Id, ToolPolicy.Code));
            if (week % 40 == 8 && released != null && released.EvalSuite == 0)
                g.Try(Command.RegisterEval(g.Id, released.Id, 1));
        }
        if (week >= 420)
        {
            g.C.AgentSandbox.MinEra = 0;
            g.C.RunawayPlaybook.MinEra = 0;
            g.C.OrchestrationBudget.MinEra = 0;
            g.C.TrafficRouter.MinEra = 0;
            g.C.PublicMetrics.MinEra = 0;
            ModelState agent = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released && m.AgentLevel > 0);
            if (week % 32 == 5 && agent != null && agent.SandLeft == 0)
                g.Try(Command.StartSandbox(g.Id, agent.Id));
            if (week % 36 == 7 && g.Me.PlayLevel == 0) g.Try(Command.SetPlaybook(g.Id, 2));
            if (week % 38 == 9 && g.Me.OrchLevel == 0) g.Try(Command.SetOrchBudget(g.Id, 1));
            if (week % 42 == 11 && agent != null && agent.RoutePpm == 0)
                g.Try(Command.SetRouteShare(g.Id, agent.Id, MathI.Ppm / 3));
            if (week % 44 == 13 && agent != null && agent.PubOn == 0)
                g.Try(Command.PublishMetrics(g.Id, agent.Id));
        }
        if (week >= 440)
        {
            g.C.SpecialistLines.MinEra = 0;
            g.C.DataSources.MinEra = 0;
            g.C.OpenWeights.MinEra = 0;
            g.C.SectorPricing.MinEra = 0;
            if (week % 34 == 6 && g.Me.SrcWeeks0 == 0) g.Try(Command.AcquireDataSource(g.Id, "licensed-corpus"));
            ModelState eraN = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
            if (week % 36 == 8 && eraN != null && eraN.SpecKind == 0)
                g.Try(Command.BranchSpecialist(g.Id, eraN.Id, "code"));
            if (week % 38 == 10 && eraN != null && eraN.WgtOpen == 0)
                g.Try(Command.SetWeightsPolicy(g.Id, eraN.Id, 1));
            if (week % 40 == 12 && eraN != null)
                g.Try(Command.SetSectorPrice(g.Id, eraN.Sectors.Count > 0 ? eraN.Sectors[0] : "game-ai", 2));
        }
        if (week >= 460)
        {
            g.C.CampaignDeadline.MinEra = 0;
            g.C.LawShaping.MinEra = 0;
            g.C.ScenarioBaggage.Enabled = 1;
            if (week % 46 == 7 && g.Me.CampaignDeadlineTick == 0)
                g.Try(Command.SetCampaignDeadline(g.Id, g.Tick + 60));
            if (week % 48 == 9 && g.Me.BaggagePicked == 0)
                g.Try(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
            if (week % 50 == 11 && g.Me.LawShapePending == 0)
                EraLawHardeningSupport.TryShapeLaw(g, LawShaping.Loose);
        }
        if (week >= 470)
        {
            g.C.MarginCall.Enabled = 1;
            g.C.ScenarioGoals.Enabled = 1;
            if (week % 52 == 7 && string.IsNullOrEmpty(g.Me.ScenarioId) && g.C.Scenarios.Scenarios.Count > 0)
                g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
            if (week % 54 == 9 && g.Me.LawTarget == "")
                EraLawHardeningSupport.TryShapeLaw(g, LawShaping.Loose);
            if (week % 56 == 11 && Loans.Outstanding(g.Me) == 0)
                g.Try(Command.TakeLoan(g.Id, 150));
        }
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
            HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true, eraHBounds: true, eraIBounds: true, eraJBounds: true, eraKBounds: true, eraLBounds: true, eraMBounds: true, eraNBounds: true, eraOBounds: true, eraPBounds: true);
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
            HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true, eraHBounds: true, eraIBounds: true, eraJBounds: true, eraKBounds: true, eraLBounds: true, eraMBounds: true, eraNBounds: true, eraOBounds: true, eraPBounds: true);
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

    public static IEnumerable<object[]> FullStackSeeds()
    {
        for (int seed = 1; seed <= 50; seed++)
            yield return new object[] { seed };
    }

    [Theory]
    [MemberData(nameof(FullStackSeeds))]
    public void Full_stack_soak_on_seed_keeps_invariants(int seed)
    {
        Game g = Soak(seed, TenYearsWeeks);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
            HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true, eraHBounds: true, eraIBounds: true, eraJBounds: true, eraKBounds: true, eraLBounds: true, eraMBounds: true, eraNBounds: true, eraOBounds: true, eraPBounds: true);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
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