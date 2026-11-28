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

/// <summary>Zero, max, bankrupt, and cross-system edges for era-i systems.</summary>
public class EraISystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Fabric.EraA = 0;
            c.Fabric.EraB = 0;
            c.Fabric.EraC = 0;
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 10);
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static ModelState Selling(Game g, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        m.Alignment = 800;
        return m;
    }

    [Fact]
    public void Api_slot_cap_refuses_a_third_open_api()
    {
        Game g = Lab(29, c => c.Integrations.MaxOpen = 1);
        ModelState a = Selling(g);
        ModelState b = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, a.Id));
        g.Refused(Command.OpenApi(g.Id, b.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Org_change_refuses_while_locked_after_recent_reorg()
    {
        Game g = Lab(30);
        g.Do(Command.SetOrg(g.Id, Org.Functional));
        g.Refused(Command.SetOrg(g.Id, Org.Squads), CommandError.InvalidArgument);
        g.Weeks(g.C.Org.CooldownWeeks);
        g.Do(Command.SetOrg(g.Id, Org.Squads));
        Assert.Equal(Org.Squads, g.Me.OrgForm);
    }

    [Fact]
    public void Fabric_cancel_after_deposit_loses_deposit_but_validates_save()
    {
        Game g = Lab(31);
        long cash = g.Me.Cash;
        g.Do(Command.OrderFabric(g.Id));
        g.Do(Command.CancelFabric(g.Id));
        Assert.True(g.Me.Cash < cash);
        Assert.Equal(0, g.Me.FabLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Bankrupt_lab_with_api_register_and_fabric_validates_save()
    {
        Game g = Lab(32);
        ModelState m = Selling(g);
        g.Do(Command.OpenApi(g.Id, m.Id));
        g.Do(Command.JoinRegister(g.Id));
        g.Do(Command.OrderFabric(g.Id));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Integration_and_register_on_same_incident_survive_save_load()
    {
        Game g = Lab(33);
        ModelState m = Selling(g);
        g.Do(Command.OpenApi(g.Id, m.Id));
        m.Integrations = 400;
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraIBounds: true);
    }
}