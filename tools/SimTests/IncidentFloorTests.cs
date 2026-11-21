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

/// <summary>The floors under incident cuts: insurance, the on-call rota, a chief safety officer and any later plan cannot make incidents free.</summary>
public class IncidentFloorTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        return m;
    }

    static GameEvent Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        int before = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        return g.State.Events.Skip(before).First(e => e.Kind == EventKind.Incident);
    }

    /// <summary>The fine the lab pays for the same incident with and without the cuts.</summary>
    static void Fines(Action<GameConfig> tweak, Action<Game> stack, out long gross, out long net, out Game stacked)
    {
        Game plain = Lab(tweak);
        gross = Incident(plain, Selling(plain)).Amount;
        stacked = Lab(tweak);
        ModelState m = Selling(stacked);
        stack(stacked);
        net = Incident(stacked, m).Amount;
    }

    static void EverythingOn(Game g)
    {
        g.Me.InsuranceLevel = 3;
        g.Me.OnCallLevel = 3;
    }

    // ---- the data

    [Fact]
    public void The_floors_ship_and_the_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(150, c.IncidentFloor.FineFloorPermille);
        Assert.Equal(400, c.IncidentFloor.ChanceFloorPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.IncidentFloor.FineFloorPermille = 1001;
        c.IncidentFloor.ChanceFloorPermille = -1;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("incidentFloor.fineFloorPermille"));
        Assert.Contains(problems, p => p.Contains("incidentFloor.chanceFloorPermille"));
    }

    // ---- the helpers

    [Fact]
    public void The_fine_helpers_keep_a_fine_between_the_floor_and_the_gross_fine()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(150, IncidentFloor.FineFloor(c, 1000));
        Assert.Equal(0, IncidentFloor.FineFloor(c, 0));
        Assert.Equal(150, IncidentFloor.CutFine(c, 1000, 0));
        Assert.Equal(150, IncidentFloor.CutFine(c, 1000, -50));
        Assert.Equal(400, IncidentFloor.CutFine(c, 1000, 400));
        Assert.Equal(1000, IncidentFloor.CutFine(c, 1000, 5000));
        Assert.Equal(0, IncidentFloor.CutFine(c, 0, 0));
        Assert.Equal(250, IncidentFloor.MaxPayout(c, 1000, 400));
        Assert.Equal(0, IncidentFloor.MaxPayout(c, 1000, 150));
        Assert.Equal(0, IncidentFloor.MaxPayout(c, 1000, 100));
    }

    [Fact]
    public void The_chance_helper_keeps_a_share_of_the_uncut_chance()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(400, IncidentFloor.Chance(c, 1000, 0));
        Assert.Equal(400, IncidentFloor.Chance(c, 1000, 100));
        Assert.Equal(700, IncidentFloor.Chance(c, 1000, 700));
        Assert.Equal(200, IncidentFloor.Chance(c, 500, 50));
        Assert.Equal(1000, IncidentFloor.Chance(c, 3000, 3000));
        Assert.Equal(0, IncidentFloor.Chance(c, 0, 0));
    }

    [Fact]
    public void A_zero_floor_turns_it_off_and_a_full_floor_turns_every_cut_off()
    {
        GameConfig c = TestData.Config();
        c.IncidentFloor.FineFloorPermille = 0;
        c.IncidentFloor.ChanceFloorPermille = 0;
        Assert.Equal(0, IncidentFloor.CutFine(c, 1000, 0));
        Assert.Equal(0, IncidentFloor.Chance(c, 1000, 0));
        c.IncidentFloor.FineFloorPermille = 1000;
        c.IncidentFloor.ChanceFloorPermille = 1000;
        Assert.Equal(1000, IncidentFloor.CutFine(c, 1000, 0));
        Assert.Equal(1000, IncidentFloor.Chance(c, 1000, 0));
        Assert.Equal(0, IncidentFloor.MaxPayout(c, 1000, 1000));
    }

    // ---- fines

    [Fact]
    public void With_the_shipped_data_insurance_and_the_rota_together_stay_above_the_floor_without_the_floor_binding()
    {
        long gross, net;
        Game g;
        Fines(null, EverythingOn, out gross, out net, out g);
        long afterRota = MathI.MulPermille(gross, 700);
        long paid = MathI.MulPermille(afterRota, 750);
        Assert.Equal(afterRota - paid, net);
        Assert.True(net >= IncidentFloor.FineFloor(g.C, gross));
        Assert.True(net < gross);
    }

    [Fact]
    public void Data_that_would_make_the_fine_nothing_leaves_the_floor_instead()
    {
        long gross, net;
        Game g;
        Fines(c => { c.Finance.InsuranceMaxLevel = 1; c.Finance.InsuranceCoverPermille = 1000; c.OnCall.FineCutPermillePerLevel = 300; },
            EverythingOn, out gross, out net, out g);
        Assert.True(gross > 0);
        Assert.Equal(IncidentFloor.FineFloor(g.C, gross), net);
        Assert.Equal(MathI.MulPermille(gross, 150), net);
    }

    [Fact]
    public void Without_the_floor_the_same_data_would_make_the_incident_free()
    {
        long gross, net;
        Game g;
        Fines(c => { c.Finance.InsuranceMaxLevel = 1; c.Finance.InsuranceCoverPermille = 1000; c.OnCall.FineCutPermillePerLevel = 300; c.IncidentFloor.FineFloorPermille = 0; },
            x => { x.Me.InsuranceLevel = 1; x.Me.OnCallLevel = 3; }, out gross, out net, out g);
        Assert.Equal(0, net);
    }

    [Fact]
    public void The_insurer_pays_only_what_leaves_the_floor_and_the_claim_still_counts()
    {
        Game plain = Lab(c => { c.Finance.InsuranceMaxLevel = 1; c.Finance.InsuranceCoverPermille = 1000; });
        long gross = Incident(plain, Selling(plain)).Amount;
        Game g = Lab(c => { c.Finance.InsuranceMaxLevel = 1; c.Finance.InsuranceCoverPermille = 1000; });
        ModelState m = Selling(g);
        g.Me.InsuranceLevel = 1;
        GameEvent e = Incident(g, m);
        Assert.Equal(MathI.MulPermille(gross, 150), e.Amount);
        GameEvent claim = g.State.Events.Single(x => x.Kind == EventKind.InsuranceClaimPaid);
        Assert.Equal(gross - MathI.MulPermille(gross, 150), claim.Amount);
        Assert.Equal(1, g.Me.InsuranceClaims);
    }

    [Fact]
    public void A_full_floor_means_no_cut_does_anything_and_the_insurer_pays_nothing()
    {
        long gross, net;
        Game g;
        Fines(c => c.IncidentFloor.FineFloorPermille = 1000, EverythingOn, out gross, out net, out g);
        Assert.Equal(gross, net);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.InsuranceClaimPaid);
    }

    [Fact]
    public void Insurance_alone_and_the_rota_alone_are_not_touched_by_the_floor()
    {
        long gross, net;
        Game g;
        Fines(null, x => x.Me.InsuranceLevel = 3, out gross, out net, out g);
        Assert.Equal(gross - MathI.MulPermille(gross, 750), net);
        Fines(null, x => x.Me.OnCallLevel = 3, out gross, out net, out g);
        Assert.Equal(MathI.MulPermille(gross, 700), net);
    }

    [Fact]
    public void A_future_plan_that_cuts_the_fine_goes_through_the_same_floor()
    {
        GameConfig c = TestData.Config();
        long gross = 100_000;
        long afterRota = MathI.MulPermille(gross, 700);
        long afterPlanToo = IncidentFloor.CutFine(c, gross, MathI.MulPermille(afterRota, 100));
        Assert.Equal(15_000, afterPlanToo);
        long maxInsurer = IncidentFloor.MaxPayout(c, gross, afterPlanToo);
        Assert.Equal(0, maxInsurer);
    }

    // ---- the weekly chance

    [Fact]
    public void A_chief_safety_officer_cannot_cut_the_chance_below_the_floor()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        g.C.Chiefs.IncidentCutPermille = 900;
        long uncut = SafetySystem.IncidentChancePermille(g.C, g.State.Tick, m) + SafetySystem.OverloadPermille(g.C, g.State, g.Me);
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(IncidentFloor.Chance(g.C, uncut, uncut - MathI.MulPermille(uncut, 900)), chance);
        Assert.True(chance >= MathI.MulPermille(Math.Min(uncut, 1000), 400));
        g.C.IncidentFloor.ChanceFloorPermille = 0;
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) < chance);
    }

    [Fact]
    public void With_the_shipped_data_the_chance_is_not_touched_by_the_floor()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        int none = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        int cut = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(none - MathI.MulPermille(none, 150), cut);
    }

    [Fact]
    public void The_chance_floor_follows_the_uncut_chance_including_what_optimisation_adds()
    {
        Game g = Lab(c => c.Optimise.EngineersPerLevel = 1);
        ModelState m = Selling(g);
        g.Do(Command.SetOptimisation(g.Id, 3));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        g.C.Chiefs.IncidentCutPermille = 1000;
        long uncut = SafetySystem.IncidentChancePermille(g.C, g.State.Tick, m) + SafetySystem.OverloadPermille(g.C, g.State, g.Me) + Crunch.IncidentPermille(g.C, g.Me) + Optimisation.IncidentPermille(g.C, g.Me);
        Assert.True(Optimisation.IncidentPermille(g.C, g.Me) > 0);
        Assert.Equal((int)MathI.MulPermille(Math.Min(uncut, 1000), 400), SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
    }

    // ---- whole games

    [Fact]
    public void A_long_game_with_every_cut_on_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c => { c.Finance.InsuranceCoverPermille = 330; c.OnCall.StaffPerLevel = 0; });
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(60);
        g.Try(Command.SetInsurance(g.Id, 3));
        g.Try(Command.SetOnCall(g.Id, 3));
        for (int i = 0; i < 200; i++) g.Autopilot(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
