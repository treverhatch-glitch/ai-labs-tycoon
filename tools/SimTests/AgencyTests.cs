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

public class AgencyTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Shocks.Enabled = 0;
            c.Agency.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Weeks(2);
        return m;
    }

    static void Wing(Game g)
    {
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8 + g.Me.Rooms.Count, Y = 0, Width = 4, Height = 4 });
    }

    [Fact]
    public void A_model_with_no_autonomy_earns_and_risks_exactly_what_it_did()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        Assert.Equal(0, Agency.IncidentPermille(g.C, m));
        Assert.Equal(0, Agency.Lift(g.C, g.State, g.Me));
        g.Weeks(10);
        Assert.Equal(0, g.Me.AgentEarned);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.AutonomySet || e.Kind == EventKind.AgentBlowup);
    }

    [Fact]
    public void The_levels_are_checked_and_each_one_asks_for_something_the_lab_must_have()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Refused(Command.SetAutonomy(g.Id, m.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetAutonomy(g.Id, 999, 1), CommandError.UnknownId);
        g.Refused(Command.SetAutonomy(g.Id, m.Id, 2), CommandError.MissingPrerequisite);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        Wing(g);
        g.Weeks(g.C.Agency.Cooldown);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        Wing(g);
        m.Alignment = 100;
        g.Weeks(g.C.Agency.Cooldown);
        g.Refused(Command.SetAutonomy(g.Id, m.Id, 3), CommandError.MissingPrerequisite);
        m.Alignment = 900;
        g.Do(Command.SetAutonomy(g.Id, m.Id, 3));
        Assert.Equal(3, m.AgentLevel);
        Game early = Lab(c => c.Agency.MinEra = c.Eras.Count - 1);
        ModelState em = Selling(early);
        early.Refused(Command.SetAutonomy(early.Id, em.Id, 1), CommandError.EraLocked);
        Game off = Lab(c => c.Agency.Enabled = 0);
        ModelState om = Selling(off);
        off.Refused(Command.SetAutonomy(off.Id, om.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_raise_costs_setup_and_a_cooldown_and_lowering_is_free()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        Assert.Equal(before - g.C.Agency.SetupCost, g.Me.Ledger.Lifetime.Training);
        Wing(g);
        g.Refused(Command.SetAutonomy(g.Id, m.Id, 2), CommandError.InvalidArgument);
        g.Weeks(g.C.Agency.Cooldown);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        long mid = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.SetAutonomy(g.Id, m.Id, 0));
        Assert.Equal(mid, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(0, m.AgentSince);
        Game poor = Lab();
        ModelState pm = Selling(poor);
        poor.Me.Cash = 100;
        poor.Refused(Command.SetAutonomy(poor.Id, pm.Id, 1), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Agents_lift_what_the_model_earns_and_add_to_its_incident_chance()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        long revenue = MarketSystem.ModelRevenue(g.State, g.Id, m.Id);
        Assert.True(revenue > 0);
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        Assert.Equal(revenue * g.C.Agency.Lift1 / 1000, Agency.Lift(g.C, g.State, g.Me));
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) > chance);
        Assert.Equal(g.C.Agency.Incident1, Agency.IncidentPermille(g.C, m));
        long earned = g.Me.AgentEarned;
        g.Weeks(4);
        Assert.True(g.Me.AgentEarned > earned);
    }

    [Fact]
    public void Containment_wings_cut_the_chance_of_a_blow_up_and_a_blow_up_costs_a_refund_and_trust()
    {
        Game g = Lab(c => { c.Agency.Blowup2 = 1000; c.Agency.Blowup3 = 1000; c.Agency.RoomCut = 250; });
        ModelState m = Selling(g);
        Wing(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        Assert.Equal(750, Agency.BlowupChance(g.C, g.Me, m));
        Wing(g);
        Wing(g);
        Wing(g);
        Wing(g);
        Assert.Equal(250, Agency.BlowupChance(g.C, g.Me, m));
        g.C.Agency.RoomCut = 0;
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        long fines = g.Me.Ledger.Lifetime.Fines;
        long revenue = MarketSystem.ModelRevenue(g.State, g.Id, m.Id);
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, m.AgentBlowups);
        Assert.Equal(0, m.AgentStreak);
        Assert.Equal(fines - revenue * g.C.Agency.RefundWeeks, g.Me.Ledger.Lifetime.Fines);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) < trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AgentBlowup);
    }

    [Fact]
    public void A_clean_streak_earns_trust_and_a_blow_up_resets_it()
    {
        Game g = Lab(c => { c.Agency.StreakWeeks = 3; });
        ModelState m = Selling(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        for (int i = 0; i < 3; i++) Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(3, m.AgentStreak);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AgentStreak);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) >= trust);
        m.AgentLevel = 2;
        g.C.Agency.Blowup2 = 1000;
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, m.AgentStreak);
    }

    [Fact]
    public void A_model_that_leaves_sale_loses_its_agents()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        m.Status = ModelStatus.Retired;
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, m.AgentLevel);
        Assert.Equal(0, Agency.Lift(g.C, g.State, g.Me));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_autonomy()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 119, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.AgentLevel + x.AgentStreak + x.AgentBlowups));
        Assert.Equal(0, older.State.Labs.First(l => l.Id == g.Id).AgentEarned);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
