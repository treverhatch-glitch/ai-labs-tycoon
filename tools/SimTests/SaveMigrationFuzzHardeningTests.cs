using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Every save schema from 1 through the current one loads, plays on, and keeps the books balanced.</summary>
public class SaveMigrationFuzzHardeningTests
{
    const int PlayWeeks = 24;

    static Game RichSave()
    {
        Game g = Game.Solo(42, c =>
        {
            foreach (var t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 50_000_000;
        }).WithStarterLab(researchers: 2, engineers: 2, safety: 2, sales: 2, gpus: 8);
        g.Autopilot(80);
        ModelState m = g.Me.Models.Find(x => x.Status == ModelStatus.Trained);
        if (m != null) g.Try(Command.ReleaseModel(g.Id, m.Id, 1000, "game-ai"));
        g.Try(Command.SetAsideReserve(g.Id, 100_000));
        g.Try(Command.SetPledge(g.Id, 1));
        g.Try(Command.AnnounceRoadmap(g.Id, 1, 30));
        g.Try(Command.AssignAccountTeam(g.Id, "game-ai", 1));
        g.Try(Command.SubmitPaper(g.Id, 1, 2));
        g.Try(Command.StartInternships(g.Id, "safety", 1));
        g.Try(Command.StaffSupportDesk(g.Id, 1));
        g.Weeks(4);
        return g;
    }

    static void TouchEraC(Game g)
    {
        g.Try(Command.AdoptPolicy(g.Id, 1));
        g.Try(Command.BuildSecurity(g.Id));
        g.Me.Exposure = 0;
        g.Me.ContractNext = g.State.Tick + g.C.GovContracts.OfferEveryWeeks;
        Litigation.File(g.C, g.State, g.Me, Litigation.Incident, 50_000, 500, "hardening");
        g.Weeks(2);
    }

    static void TouchEraD(Game g)
    {
        g.Try(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        g.Try(Command.SetWarranty(g.Id, 1));
        if (Loans.Outstanding(g.Me) == 0) g.Try(Command.TakeLoan(g.Id, 200));
        g.Weeks(2);
    }

    static bool EraBounds(int schema) => schema >= 72;
    static bool EraBBounds(int schema) => schema >= 77;
    static bool EraCBounds(int schema) => schema >= 81;
    static bool EraDBounds(int schema) => schema >= 85;
    static bool EraEBounds(int schema) => schema >= 90;
    static bool EraFBounds(int schema) => schema >= 95;
    static bool EraGBounds(int schema) => schema >= 100;
    static bool EraHBounds(int schema) => schema >= 105;
    static bool EraIBounds(int schema) => schema >= 110;
    static bool EraJBounds(int schema) => schema >= 115;
    static bool EraKBounds(int schema) => schema >= 120;
    static bool EraLBounds(int schema) => schema >= 125;

    static void TouchEraE(Game g)
    {
        g.C.Listing.MinWeeklyRevenue = 1;
        g.C.Rbf.MinWeeklyRevenue = 1;
        g.Me.Reputation = 600;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Try(Command.ListCompany(g.Id));
        g.Try(Command.TakeRevenueAdvance(g.Id, 1, 500));
        g.Try(Command.EndowFoundation(g.Id, 1000, Foundation.Safety));
        if (LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq) >= 4) g.Try(Command.SaleLeaseback(g.Id, 4));
        FundingSystem.ApplyRaise(g.C, g.State, g.Me, g.C.Funding.Round("angel"));
        g.Try(Command.IssueNote(g.Id, 1, 500));
        g.Weeks(2);
    }

    static void TouchEraF(Game g)
    {
        int tick2027 = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.State.Tick = Math.Max(g.State.Tick, tick2027);
        g.Me.Reputation = Math.Max(g.Me.Reputation, 600);
        ModelState released = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
        if (released == null)
        {
            ModelState trained = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained);
            if (trained != null) g.Try(Command.ReleaseModel(g.Id, trained.Id, 1000, "game-ai"));
            else
            {
                g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "hardening" });
            }
        }
        g.Try(Command.SetCreed(g.Id, Creed.Profit, Creed.Openness));
        g.Try(Command.SignPlatform(g.Id, 1));
        g.Try(Command.DesignDevice(g.Id, 1, 0));
        int war = g.C.WorldEvents.FindIndex(e => e.Id == "talent-war");
        if (war >= 0)
        {
            Shocks.OnWorldEvent(g.C, g.State, war);
            if (Shocks.Pending(g.Me)) g.Try(Command.AnswerShock(g.Id, 2));
        }
        if (g.Me.ResearchPoints >= 300)
        {
            g.Try(Command.SubmitPaper(g.Id, 1, 1));
            g.Try(Command.PublishPaper(g.Id));
            if (g.Me.ReplPending > 0) g.Try(Command.ReleaseCode(g.Id, 0));
        }
        g.Weeks(2);
    }

    static void TouchEraG(Game g)
    {
        ModelState trained = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained);
        if (trained == null)
        {
            ModelState m = g.Train("mini", 200);
            g.UntilTrained(m);
            trained = m;
        }
        g.Try(Command.BetOnTech(g.Id, "regression-tests"));
        g.Try(Command.OpenWaitlist(g.Id, trained.Id, 15));
        g.Try(Command.OpenCareersPage(g.Id));
        g.Try(Command.OpenFreeTier(g.Id, 1));
        g.Try(Command.OpenWatchDesk(g.Id));
        g.Weeks(2);
    }

    static void TouchEraH(Game g)
    {
        g.Me.ResearchPoints = 50_000;
        g.Try(Command.StartProgramme(g.Id, "beowulf-clusters"));
        g.Try(Command.ChooseApproach(g.Id, 1));
        ModelState m = g.Train("mini", 120);
        if (m != null) g.Try(Command.SetRunPriority(g.Id, m.Id, 2));
        if (g.Me.Staff.Count >= 2)
            g.Try(Command.FormPod(g.Id, g.Me.Staff[0].Id, g.Me.Staff[1].Id));
        g.Weeks(6);
        if (m != null && m.Status == ModelStatus.Trained)
        {
            g.State.Tick = Math.Max(g.State.Tick, (2027 - 1995) * g.C.Calendar.WeeksPerYear);
            g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
            m.Alignment = 800;
            g.Try(Command.ApplyLicence(g.Id, 1));
            g.Try(Command.PledgeSupport(g.Id, m.Id, 52));
        }
        g.Weeks(2);
    }

    static void TouchEraI(Game g)
    {
        g.C.Fabric.EraA = 0;
        g.C.Fabric.EraB = 0;
        g.C.Fabric.EraC = 0;
        if (LabQueries.TotalGpus(g.Me) < 6)
            Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 10);
        ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released);
        if (m == null)
        {
            m = g.Train("mini", 500);
            if (m != null)
            {
                g.UntilTrained(m);
                g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
            }
        }
        ModelState next = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released && x != m);
        if (m != null) g.Try(Command.OpenApi(g.Id, m.Id));
        g.Try(Command.SetOrg(g.Id, Org.Functional));
        g.Try(Command.OrderFabric(g.Id));
        g.Try(Command.JoinRegister(g.Id));
        if (next != null)
        {
            next.Alignment = 800;
            next.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
            g.Try(Command.StartConformity(g.Id, next.Id, 1));
        }
        g.Weeks(2);
    }

    static void TouchEraJ(Game g)
    {
        foreach (FundingRoundConfig r in g.C.Funding.Rounds)
        {
            r.OpensYear = 1995;
            r.MinReputation = 0;
        }
        g.Me.Reputation = 600;
        g.Me.ResearchPoints = 50_000;
        g.Try(Command.AcquireData(g.Id, 1, 40));
        g.Try(Command.CleanData(g.Id, 1, 25));
        ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released);
        if (m == null)
        {
            m = g.Train("mini", 600);
            if (m != null)
            {
                g.UntilTrained(m);
                g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
            }
        }
        g.Try(Command.SetUserData(g.Id, 1));
        g.Try(Command.RaiseFunding(g.Id, "angel"));
        g.Try(Command.SetGovernance(g.Id, 1));
        g.Try(Command.SetSpotShare(g.Id, 200));
        g.Weeks(2);
    }

    static void TouchEraK(Game g)
    {
        g.C.PostTraining.MinEra = 0;
        g.C.Agency.MinEra = 0;
        g.Me.BoardInvestorSeats = Math.Max(1, g.Me.BoardInvestorSeats);
        g.Try(Command.AcquireData(g.Id, 1, 80));
        g.Try(Command.CleanData(g.Id, 1, 60));
        ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Trained);
        if (m == null)
        {
            m = g.Train("mini", 600);
            if (m != null) g.UntilTrained(m);
        }
        if (m != null)
        {
            g.Try(Command.ContractAnnotators(g.Id, 1));
            g.Try(Command.PostTrain(g.Id, m.Id, 1));
            StaffMember owner = g.Me.Staff.FirstOrDefault(s => s.RoleId == "researcher");
            if (owner != null)
            {
                g.Try(Command.AssignOwner(g.Id, m.Id, owner.Id));
                g.Try(Command.WriteDocs(g.Id, m.Id));
            }
            g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        }
        g.Try(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(4);
    }

    static void TouchErasUpTo(int maxSchema, Game g)
    {
        if (maxSchema >= 81) TouchEraC(g);
        if (maxSchema >= 85) TouchEraD(g);
        if (maxSchema >= 90) TouchEraE(g);
        if (maxSchema >= 95) TouchEraF(g);
        if (maxSchema >= 100) TouchEraG(g);
        if (maxSchema >= 105) TouchEraH(g);
        if (maxSchema >= 110) TouchEraI(g);
        if (maxSchema >= 115) TouchEraJ(g);
        if (maxSchema >= 120) TouchEraK(g);
        if (maxSchema >= 125) TouchEraL(g);
    }

    static void TouchEraL(Game g)
    {
        g.C.Drills.MinEra = 0;
        g.C.EvalHarness.MinEra = 0;
        g.C.ToolPolicy.MinEra = 0;
        g.C.HumanQueue.MinEra = 0;
        g.C.ReleaseChecklist.MinEra = 0;
        g.Try(Command.StartDrill(g.Id));
        ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Trained);
        if (m == null)
        {
            m = g.Train("mini", 700);
            if (m != null) g.UntilTrained(m);
        }
        if (m != null)
        {
            g.Try(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
            g.Try(Command.RegisterEval(g.Id, m.Id, 1));
            m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
            m.DocLevel = Math.Max(m.DocLevel, g.C.ReleaseChecklist.DocBar);
            g.Try(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs));
            g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        }
        g.Weeks(3);
    }

    public static IEnumerable<object[]> SchemaRows()
    {
        for (int schema = 1; schema <= GameState.CurrentSchema; schema++)
            yield return new object[] { schema };
    }

    [Theory]
    [MemberData(nameof(SchemaRows))]
    public void A_save_at_each_schema_loads_plays_on_and_balances_the_books(int schema)
    {
        var c = TestData.Config();
        string json;
        if (schema <= 4)
        {
            json = schema switch
            {
                1 => MigrationTests.Schema1Save(),
                2 => MigrationTests.Schema2Save(),
                3 => File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema3-save.json")),
                _ => File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema4-save.json")),
            };
        }
        else
        {
            Game rich = RichSave();
            if (schema >= 81) TouchEraC(rich);
            if (schema >= 85) TouchEraD(rich);
            if (schema >= 90) TouchEraE(rich);
            if (schema >= 95) TouchEraF(rich);
            if (schema >= 100) TouchEraG(rich);
            if (schema >= 105) TouchEraH(rich);
            if (schema >= 110) TouchEraI(rich);
            if (schema >= 115) TouchEraJ(rich);
            if (schema >= 120) TouchEraK(rich);
            if (schema >= 125) TouchEraL(rich);
            json = SaveSchemaPretend.At(c, schema, rich.Sim.Save());
        }

        Simulation sim = Simulation.Load(c, json);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        HardeningInvariants.AssertAllLabs(c, sim.State, EraBounds(schema), EraBBounds(schema), EraCBounds(schema), EraDBounds(schema), EraEBounds(schema), EraFBounds(schema), EraGBounds(schema), EraHBounds(schema), EraIBounds(schema), EraJBounds(schema), EraKBounds(schema), EraLBounds(schema));
        HardeningInvariants.StepWeeksChecking(sim, PlayWeeks, EraBounds(schema), EraBBounds(schema), EraCBounds(schema), EraDBounds(schema), EraEBounds(schema), EraFBounds(schema), EraGBounds(schema), EraHBounds(schema), EraIBounds(schema), EraJBounds(schema), EraKBounds(schema), EraLBounds(schema));
    }

    [Fact]
    public void A_rich_save_at_every_schema_round_trips_without_problems()
    {
        var c = TestData.Config();
        Game rich = RichSave();
        TouchErasUpTo(GameState.CurrentSchema, rich);
        string current = rich.Sim.Save();
        for (int schema = 77; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(c, schema, current);
            Simulation sim = Simulation.Load(c, old);
            Assert.Empty(SaveGame.Problems(c, sim.State));
            string again = sim.Save();
            Simulation reloaded = Simulation.Load(c, again);
            Assert.Equal(sim.Digest(), reloaded.Digest());
        }
    }
}