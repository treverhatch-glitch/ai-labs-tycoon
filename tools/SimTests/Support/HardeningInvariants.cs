using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using Xunit;

namespace AiLabs.SimTests.Support;

/// <summary>Shared assertions for migration fuzz, cross-system soak and era edge hardening.</summary>
public static class HardeningInvariants
{
    public static void AssertBooksBalance(GameConfig c, GameState state)
    {
        foreach (LabState lab in state.Labs)
        {
            long expected = lab.Ledger.StartCash + lab.Ledger.Lifetime.Net;
            Assert.Equal(expected, lab.Cash);
        }
    }

    public static void AssertEconomyBounds(GameConfig c, GameState state)
    {
        const long cashCeiling = 1_000_000_000_000L;
        const long cashFloor = -1_000_000_000L;
        foreach (LabState lab in state.Labs)
        {
            Assert.InRange(lab.Cash, cashFloor, cashCeiling);
            Assert.InRange(lab.Reputation, 0, 1000);
            Assert.True(lab.Reserve >= 0);
            Assert.True(LabQueries.TotalStaff(lab) >= 0);
            Assert.All(lab.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        }
        Assert.InRange(state.BubblePermille, 0, MathI.Permille);
    }

    public static void AssertNewEraStateBounds(GameConfig c, LabState lab)
    {
        if (c.Labour.Enabled != 0)
        {
            Assert.InRange(lab.LabourPressure, 0, 1000);
            Assert.InRange(lab.LabourStage, Labour.Quiet, Labour.Strike);
        }
        if (c.Accounts.Enabled != 0)
            Assert.All(lab.AccountTeams, t => Assert.InRange(t.People, 1, c.Accounts.MaxPeople));
        if (c.Conference.Enabled != 0) Assert.True(lab.Submissions.Count <= c.Conference.MaxPending);
        if (c.Alumni.Enabled != 0) Assert.True(lab.Alumni.Count <= c.Alumni.MaxSize);
        if (c.Pledge.MaxLevel > 0) Assert.InRange(lab.PledgeLevel, 0, c.Pledge.MaxLevel);
        if (c.Roadmap.MaxLevel > 0) Assert.InRange(lab.RoadmapLevel, 0, c.Roadmap.MaxLevel);
        if (c.Mentoring.Enabled != 0) Assert.InRange(Mentoring.Pairs(lab), 0, c.Mentoring.MaxPairs);
        foreach (ModelState m in lab.Models)
        {
            if (m.SunsetStart > 0) Assert.True(m.SunsetTick >= m.SunsetStart);
        }
    }

    public static void AssertEraBStateBounds(GameConfig c, LabState lab)
    {
        if (c.Internships.Enabled != 0)
        {
            Assert.True(lab.InternCount >= 0);
            if (lab.InternCount > 0) Assert.InRange(lab.InternCount, 1, c.Internships.MaxCohort);
        }
        if (c.Support.Enabled != 0)
        {
            Assert.True(lab.SupportBacklog >= 0);
            Assert.True(lab.SupportPeople >= 0);
        }
        if (c.Scholars.Enabled != 0)
        {
            Assert.InRange(lab.ScholarCount, 0, c.Scholars.MaxScholars);
            Assert.True(lab.ScholarCount == 0 || lab.ScholarDue >= 0);
        }
        if (c.Requisitions.Enabled != 0) Assert.True(lab.Requisitions.Count <= c.Requisitions.MaxOpen);
    }

    public static void AssertEraCStateBounds(GameConfig c, LabState lab)
    {
        if (c.Litigation.Enabled != 0) Assert.True(lab.Cases.Count <= c.Litigation.MaxOpen);
        if (c.Security.Enabled != 0)
        {
            Assert.InRange(lab.SecurityTier, 0, c.Security.MaxTier);
            Assert.InRange(lab.SecurityBuilding, 0, c.Security.MaxTier);
            Assert.True(lab.Exposure >= 0);
        }
        if (c.Scaling.Enabled != 0)
        {
            Assert.InRange(lab.PolicyLevel, 0, c.Scaling.MaxLevel);
            Assert.InRange(lab.PolicyHold, 0, c.Scaling.MaxLevel);
        }
        if (c.Charter.Enabled != 0) Assert.InRange(lab.Charter, Charter.ForProfit, Charter.Nonprofit);
    }

    public static void AssertEraDStateBounds(GameConfig c, LabState lab)
    {
        if (c.Tax.Enabled != 0)
        {
            Assert.True(lab.TaxAccrued >= 0);
            Assert.True(lab.TaxResearch >= 0);
        }
        if (c.Standards.Enabled != 0)
        {
            Assert.InRange(lab.StdLevel, 0, c.Standards.MaxLevel);
            Assert.InRange(lab.StdRole, Standards.Observer, Standards.Obstructor);
        }
        if (c.Reliability.Enabled != 0)
        {
            Assert.True(lab.FailCarry >= 0);
            Assert.True(lab.GpuFailures >= 0);
            Assert.True(lab.RmaUnits >= 0);
        }
        if (c.Covenants.Enabled != 0) Assert.True(lab.CovBreach >= 0);
        if (c.Allowances.Enabled != 0)
        {
            Assert.InRange(lab.CapPolicy, CapitalAllowances.Expense, CapitalAllowances.Enhanced);
            Assert.True(lab.CapPool >= 0);
        }
    }

    public static void AssertEraEStateBounds(GameConfig c, LabState lab)
    {
        if (c.Listing.Enabled != 0) Assert.True(lab.ListNext >= 0);
        if (c.Rbf.Enabled != 0)
        {
            Assert.True(lab.RbfAdvance >= 0);
            Assert.True(lab.RbfOwed >= 0);
        }
        if (c.Foundation.Enabled != 0)
        {
            Assert.True(lab.FoundFund >= 0);
            Assert.True(lab.FoundGiven >= 0);
        }
        if (c.Leaseback.Enabled != 0)
        {
            Assert.True(lab.LeaseUnits >= 0);
            Assert.True(lab.LeaseLeft >= 0);
        }
        if (c.ConvertibleNotes.Enabled != 0)
        {
            Assert.True(lab.NoteOwed >= 0);
            Assert.True(lab.NotePrincipal >= 0);
        }
    }

    public static void AssertEraFStateBounds(GameConfig c, LabState lab)
    {
        if (c.Devices.Enabled != 0)
        {
            Assert.True(lab.DevLine >= 0);
            Assert.True(lab.DevWeeks >= 0);
            Assert.True(lab.DevStage >= 0);
        }
        if (c.Platforms.Enabled != 0)
        {
            Assert.True(lab.PlatPlat >= 0);
            Assert.True(lab.PlatLeft >= 0);
        }
        if (c.Creed.Enabled != 0)
        {
            Assert.InRange(lab.CreedIntegrity, 0, 1000);
            Assert.True(lab.CreedBand >= 0);
        }
        if (c.Shocks.Enabled != 0)
        {
            Assert.InRange(lab.ShockKind, 0, 3);
            Assert.True(lab.ShockLeft >= 0);
        }
        if (c.Replication.Enabled != 0)
        {
            Assert.True(lab.ReplPending >= 0);
            Assert.True(lab.ReplLeft >= 0);
        }
    }

    public static void AssertEraGStateBounds(GameConfig c, LabState lab)
    {
        if (c.TechBets.Enabled != 0)
        {
            Assert.True(lab.TbRef >= 0);
            Assert.True(lab.TbLeft >= 0);
            Assert.True(lab.TbWins >= 0);
        }
        if (c.Waitlist.Enabled != 0)
        {
            Assert.True(lab.WlModel >= 0);
            Assert.True(lab.WlLeft >= 0);
            Assert.True(lab.WlHype >= 0);
        }
        if (c.Brand.Enabled != 0)
        {
            Assert.True(lab.BrandScore >= 0);
            Assert.True(lab.BrandOn >= 0);
        }
        if (c.FreeTier.Enabled != 0)
        {
            Assert.True(lab.FtLevel >= 0);
            Assert.True(lab.FtReach >= 0);
        }
        if (c.LaunchWatch.Enabled != 0)
        {
            Assert.True(lab.LwOn >= 0);
            Assert.True(lab.LwRival >= 0);
        }
    }

    public static void AssertEraHStateBounds(GameConfig c, LabState lab)
    {
        if (c.Programmes.Enabled != 0)
        {
            Assert.True(lab.PgTech >= 0);
            Assert.True(lab.PgLeft >= 0);
            Assert.InRange(lab.PgPhase, 0, 3);
        }
        if (c.RunQueue.Enabled != 0)
        {
            foreach (ModelState m in lab.Models)
                if (m.Status == ModelStatus.Training) Assert.InRange(m.QueueLevel, 0, 3);
        }
        if (c.Pods.Enabled != 0)
        {
            Assert.True(lab.PodIds.Count <= c.Pods.MaxSize);
            Assert.InRange(lab.PodChem, 0, MathI.Permille);
        }
        if (c.Licence.Enabled != 0)
        {
            Assert.InRange(lab.LicRegime, 0, 3);
            Assert.InRange(lab.LicPhase, 0, 2);
            Assert.True(lab.LicLeft >= 0);
        }
        if (c.Pledges.Enabled != 0)
        {
            Assert.True(lab.LtsKept >= 0);
            Assert.True(lab.LtsBroken >= 0);
        }
    }

    public static void AssertEraIStateBounds(GameConfig c, LabState lab)
    {
        if (c.Integrations.Enabled != 0)
        {
            Assert.True(lab.ApiMoved >= 0);
            Assert.True(lab.ApiLost >= 0);
            foreach (ModelState m in lab.Models)
            {
                Assert.InRange(m.ApiState, 0, 2);
                Assert.True(m.Integrations >= 0);
                if (c.Integrations.MaxIntegrations > 0)
                    Assert.True(m.Integrations <= c.Integrations.MaxIntegrations);
            }
        }
        if (c.Conformity.Enabled != 0)
        {
            Assert.True(lab.CfGranted >= 0);
            foreach (ModelState m in lab.Models)
            {
                Assert.InRange(m.CertStage, 0, Conformity.Certified);
                Assert.InRange(m.CertScope, 0, 2);
                Assert.True(m.CertLeft >= 0);
            }
        }
        if (c.Org.Enabled != 0)
        {
            Assert.InRange(lab.OrgForm, 0, Org.Squads);
            Assert.True(lab.OrgDisrupt >= 0);
            Assert.True(lab.OrgReorgs >= 0);
        }
        if (c.Fabric.Enabled != 0)
        {
            Assert.True(lab.FabTier >= 0);
            Assert.True(lab.FabLeft >= 0);
            Assert.True(lab.FabBuild >= 0);
        }
        if (c.Disclosure.Enabled != 0)
        {
            Assert.True(lab.DiscOn >= 0);
            Assert.True(lab.DiscOpen >= 0);
            Assert.True(lab.DiscFindings >= 0);
            Assert.True(lab.DiscPrep >= 0);
            Assert.True(lab.DiscFiled >= 0);
        }
    }

    public static void AssertEraJStateBounds(GameConfig c, LabState lab)
    {
        if (c.Datasets.Enabled != 0)
        {
            Assert.True(lab.DataRaw1 >= 0);
            Assert.True(lab.DataRaw2 >= 0);
            Assert.True(lab.DataRaw3 >= 0);
            Assert.True(lab.DataRaw4 >= 0);
            Assert.True(lab.DataSuits >= 0);
            Assert.True(lab.DataJobUnits >= 0);
            Assert.True(lab.DataJobLeft >= 0);
            Assert.InRange(lab.DataJobKind, 0, 4);
        }
        if (c.UserData.Enabled != 0)
        {
            Assert.InRange(lab.UserPolicy, 0, 3);
            Assert.True(lab.UserBanked >= 0);
            Assert.True(lab.UserTotal >= 0);
            Assert.True(lab.UserComplaints >= 0);
        }
        if (c.Governance.Enabled != 0)
        {
            Assert.InRange(lab.GovForm, 0, 2);
            Assert.True(lab.GovDirectors >= 0);
            Assert.True(lab.GovSearch >= 0);
            Assert.True(lab.GovResigned >= 0);
        }
        if (c.Buyback.Enabled != 0)
        {
            Assert.True(lab.BoardWon >= 0);
            Assert.True(lab.BoardLost >= 0);
            Assert.True(lab.BoughtRounds.Count >= 0);
        }
        if (c.Spot.Enabled != 0)
        {
            Assert.InRange(lab.SpotPermille, 0, MathI.Permille);
            Assert.True(lab.SpotOut >= 0);
            Assert.True(lab.SpotWaves >= 0);
        }
    }

    public static void AssertAllLabs(GameConfig c, GameState state, bool eraBounds, bool eraBBounds = false, bool eraCBounds = false, bool eraDBounds = false, bool eraEBounds = false, bool eraFBounds = false, bool eraGBounds = false, bool eraHBounds = false, bool eraIBounds = false, bool eraJBounds = false)
    {
        Assert.Empty(SaveGame.Problems(c, state));
        AssertBooksBalance(c, state);
        AssertEconomyBounds(c, state);
        foreach (LabState lab in state.Labs)
        {
            if (lab.Id != state.PlayerLabId) continue;
            if (eraBounds) AssertNewEraStateBounds(c, lab);
            if (eraBBounds) AssertEraBStateBounds(c, lab);
            if (eraCBounds) AssertEraCStateBounds(c, lab);
            if (eraDBounds) AssertEraDStateBounds(c, lab);
            if (eraEBounds) AssertEraEStateBounds(c, lab);
            if (eraFBounds) AssertEraFStateBounds(c, lab);
            if (eraGBounds) AssertEraGStateBounds(c, lab);
            if (eraHBounds) AssertEraHStateBounds(c, lab);
            if (eraIBounds) AssertEraIStateBounds(c, lab);
            if (eraJBounds) AssertEraJStateBounds(c, lab);
        }
    }

    public static void StepWeeksChecking(Simulation sim, int weeks, bool eraBounds, bool eraBBounds = false, bool eraCBounds = false, bool eraDBounds = false, bool eraEBounds = false, bool eraFBounds = false, bool eraGBounds = false, bool eraHBounds = false, bool eraIBounds = false, bool eraJBounds = false)
    {
        for (int w = 0; w < weeks; w++)
        {
            sim.Step();
            AssertAllLabs(sim.Config, sim.State, eraBounds, eraBBounds, eraCBounds, eraDBounds, eraEBounds, eraFBounds, eraGBounds, eraHBounds, eraIBounds, eraJBounds);
        }
    }
}