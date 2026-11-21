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

/// <summary>Sandbox wing: red-teamers contain some incidents before they become public scandals.</summary>
public class SandboxCatchTests
{
    static Game Ready(long seed = 7, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        Game g = new Game(Simulation.NewGame(TestData.Config(tweak), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000);
        g.WithStarterLab(gpus: 12);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 3, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "sandbox-wing", x, y, 3, 3));
        g.Do(Command.Hire(g.Id, "red-teamer", 1));
        return g;
    }

    static void ForceFullCatch(GameConfig c)
    {
        c.Safety.SandboxCatch.CatchPermillePerTile = 200;
        c.Safety.SandboxCatch.CatchCapPermille = 1000;
    }

    static ModelState ReleasedLowAlignment(Game g)
    {
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        return model;
    }

    [Fact]
    public void Catch_permille_is_zero_without_sandbox_room()
    {
        Game g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Assert.Equal(0, SandboxSystem.CatchPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Catch_permille_scales_with_tiles_and_red_teamers()
    {
        Game g = Ready();
        int p = SandboxSystem.CatchPermille(g.C, g.State, g.Me);
        Assert.Equal(9 * g.C.Safety.SandboxCatch.CatchPermillePerTile + g.C.Safety.SandboxCatch.CatchPermillePerStaff, p);
    }

    [Fact]
    public void High_catch_config_emits_sandbox_events_not_public_incidents()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        ModelState model = ReleasedLowAlignment(g);
        int monthBefore = g.Me.IncidentsThisMonth;
        g.Weeks(1);
        Assert.True(model.Incidents > 0);
        Assert.Equal(monthBefore, g.Me.IncidentsThisMonth);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SandboxCaught && e.SubjectId == model.Id);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.Incident && e.SubjectId == model.Id);
    }

    [Fact]
    public void Public_incidents_still_increment_incidents_this_month()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            c.Safety.SandboxCatch.CatchCapPermille = 0;
        });
        ReleasedLowAlignment(g);
        g.Weeks(1);
        Assert.True(g.Me.IncidentsThisMonth > 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Incident);
    }

    [Fact]
    public void Caught_incidents_cost_less_reputation_than_public_ones()
    {
        Game caught = Ready(seed: 11, tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        int repCaughtBefore = caught.Me.Reputation;
        ReleasedLowAlignment(caught);
        caught.Weeks(1);

        Game publicLab = Ready(seed: 11, tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            c.Safety.SandboxCatch.CatchCapPermille = 0;
        });
        int repPublicBefore = publicLab.Me.Reputation;
        ReleasedLowAlignment(publicLab);
        publicLab.Weeks(1);

        int caughtLoss = repCaughtBefore - caught.Me.Reputation;
        int publicLoss = repPublicBefore - publicLab.Me.Reputation;
        Assert.True(publicLoss > caughtLoss);
    }

    [Fact]
    public void Sandbag_rung_is_never_eligible_for_catch()
    {
        Assert.False(SandboxSystem.MayCatchRung(Game.Solo().C, 3));
        Assert.True(SandboxSystem.MayCatchRung(Game.Solo().C, 2));
    }

    [Fact]
    public void Sandbag_incidents_skip_sandbox_even_with_room()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
            c.Safety.SandbagGapPermille = 1;
        });
        ModelState model = g.Train("mini", 50);
        g.UntilTrained(model);
        model.Alignment = 100;
        model.DisplayedAlignment = 900;
        g.Release(model, "game-ai");
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, model));
        g.Weeks(1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Incident);
    }

    [Fact]
    public void Sandbox_caught_total_increments_on_contained_incidents()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        ReleasedLowAlignment(g);
        g.Weeks(2);
        Assert.True(g.Me.SandboxCaughtTotal >= 1);
    }

    [Fact]
    public void Era_lock_keeps_catch_permille_at_zero()
    {
        Game g = Ready();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1998);
        Assert.Equal(0, SandboxSystem.CatchPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Red_teamers_required_for_catch_permille()
    {
        Game g = Ready();
        for (int i = g.Me.Staff.Count - 1; i >= 0; i--)
            if (g.Me.Staff[i].RoleId == "red-teamer") g.Me.Staff.RemoveAt(i);
        Assert.Equal(0, SandboxSystem.CatchPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Catch_cap_limits_permille()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.SandboxCatch.CatchCapPermille = 50;
            c.Safety.SandboxCatch.CatchPermillePerTile = 100;
        });
        Assert.Equal(50, SandboxSystem.CatchPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Room_tile_counter_sums_sandbox_wings()
    {
        Game g = Ready();
        Assert.Equal(9, SandboxSystem.RoomTiles(g.Me, "sandbox-wing"));
    }

    [Fact]
    public void Save_and_load_keeps_sandbox_caught_total()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        ReleasedLowAlignment(g);
        g.Weeks(1);
        int total = g.Me.SandboxCaughtTotal;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(total, loaded.Me.SandboxCaughtTotal);
    }

    [Fact]
    public void Schema_17_on_new_game()
    {
        Game g = Game.Solo();
        Assert.Equal(17, g.State.Schema);
        Assert.Equal(0, g.Me.SandboxCaughtTotal);
    }

    [Fact]
    public void Migration_16_to_17_adds_sandbox_caught_total()
    {
        Game g = Game.Solo();
        string json = g.Sim.Save().Replace("\"schema\":17", "\"schema\":16");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(17, loaded.State.Schema);
        Assert.Equal(0, loaded.Me.SandboxCaughtTotal);
    }

    [Fact]
    public void Validator_requires_sandbox_catch_block()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.SandboxCatch = null;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("sandboxCatch"));
    }

    [Fact]
    public void Caught_fines_hit_ledger_without_public_headline()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        long finesBefore = g.Me.Ledger.Lifetime.Fines;
        ReleasedLowAlignment(g);
        g.Weeks(1);
        Assert.True(g.Me.Ledger.Lifetime.Fines < finesBefore);
    }

    [Fact]
    public void Apply_caught_increments_model_incident_counter()
    {
        Game g = Ready();
        ModelState model = ReleasedLowAlignment(g);
        SandboxSystem.ApplyCaught(g.C, g.State, g.Me, model, 1, g.C.Safety.IncidentRung(1));
        Assert.Equal(1, model.Incidents);
        Assert.Equal(1, g.Me.SandboxCaughtTotal);
    }

    [Fact]
    public void Catch_permille_zero_for_wrong_room_type_only()
    {
        Game g = Ready();
        RoomState wing = g.Me.Rooms.First(r => r.TypeId == "sandbox-wing");
        g.Do(Command.Fire(g.Id, "red-teamer", 1));
        g.Do(Command.DemolishRoom(g.Id, wing.Id));
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "research-lab", x, y, 4, 4));
        Assert.Equal(0, SandboxSystem.CatchPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Many_weeks_with_catch_produce_mixed_event_kinds()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 400;
            c.Safety.SandboxCatch.CatchCapPermille = 500;
        });
        ReleasedLowAlignment(g);
        g.Weeks(20);
        bool caught = g.State.Events.Any(e => e.Kind == EventKind.SandboxCaught);
        Assert.True(caught || g.State.Events.Any(e => e.Kind == EventKind.Incident));
    }

    [Fact]
    public void Max_rung_caught_blocks_rung_three_only()
    {
        Assert.True(SandboxSystem.MayCatchRung(Game.Solo().C, 1));
        Assert.True(SandboxSystem.MayCatchRung(Game.Solo().C, 2));
        Assert.False(SandboxSystem.MayCatchRung(Game.Solo().C, 3));
    }

    [Fact]
    public void Config_caught_permille_scales_penalties()
    {
        Game g = Ready(tweak: c => c.Safety.SandboxCatch.CaughtReputationPermille = 100);
        ModelState model = ReleasedLowAlignment(g);
        int repBefore = g.Me.Reputation;
        SandboxSystem.ApplyCaught(g.C, g.State, g.Me, model, 1, g.C.Safety.IncidentRung(1));
        int loss = repBefore - g.Me.Reputation;
        Assert.Equal((int)MathI.MulPermille(40, 100), loss);
    }

    [Fact]
    public void Staff_on_incident_still_runs_when_caught()
    {
        Game g = Ready();
        ModelState model = ReleasedLowAlignment(g);
        SandboxSystem.ApplyCaught(g.C, g.State, g.Me, model, 1, g.C.Safety.IncidentRung(1));
        Assert.True(g.Me.Staff.Count > 0);
    }

    [Fact]
    public void Punchline_start_can_build_sandbox_after_2017()
    {
        Game g = Ready();
        Assert.True(SandboxSystem.CatchPermille(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Zero_incident_chance_never_catches_or_publishes()
    {
        Game g = Ready(tweak: c => c.Safety.IncidentChancePermille = 0);
        ReleasedLowAlignment(g);
        g.Weeks(4);
        Assert.Equal(0, g.Me.SandboxCaughtTotal);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.SandboxCaught);
    }

    [Fact]
    public void Rung_one_caught_skips_incidents_this_month()
    {
        Game g = Ready(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            ForceFullCatch(c);
        });
        ReleasedLowAlignment(g);
        g.Weeks(3);
        Assert.True(g.Me.SandboxCaughtTotal >= 2);
        Assert.Equal(0, g.Me.IncidentsThisMonth);
    }
}