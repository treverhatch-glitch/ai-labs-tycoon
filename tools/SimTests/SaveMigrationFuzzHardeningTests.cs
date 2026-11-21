using System.Collections.Generic;
using System.IO;
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
            json = SaveSchemaPretend.At(c, schema, rich.Sim.Save());
        }

        Simulation sim = Simulation.Load(c, json);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        HardeningInvariants.AssertAllLabs(c, sim.State, EraBounds(schema), EraBBounds(schema), EraCBounds(schema), EraDBounds(schema), EraEBounds(schema));
        HardeningInvariants.StepWeeksChecking(sim, PlayWeeks, EraBounds(schema), EraBBounds(schema), EraCBounds(schema), EraDBounds(schema), EraEBounds(schema));
    }

    [Fact]
    public void A_rich_save_at_every_schema_round_trips_without_problems()
    {
        var c = TestData.Config();
        Game rich = RichSave();
        if (GameState.CurrentSchema >= 81) TouchEraC(rich);
        if (GameState.CurrentSchema >= 85) TouchEraD(rich);
        if (GameState.CurrentSchema >= 90) TouchEraE(rich);
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