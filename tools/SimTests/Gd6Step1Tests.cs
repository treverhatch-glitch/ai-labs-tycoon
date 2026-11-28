using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>GD-6 build step 1: empty garage, draw-room, clutter, doors, desks (§13 step 1).</summary>
public class Gd6Step1Tests
{
    static Game Garage1995(long seed = 11, string founderId = "")
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new System.Collections.Generic.List<string>(), FounderId = founderId };
        return new Game(Simulation.NewGame(TestData.Config(), options));
    }

    static LotClutterState Tower(Game g)
    {
        return g.Me.Clutter.First(c => c.ClutterId == "founder-gaming-tower");
    }

    [Fact]
    public void A_1995_garage_starts_empty_with_sixty_thousand_dollars()
    {
        Game g = Garage1995();
        Assert.Equal(1995, g.Sim.Year);
        Assert.Equal("garage", g.Me.LotId);
        Assert.Equal(60_000, g.Me.Cash);
        Assert.Empty(g.Me.Rooms);
        Assert.Equal(3, g.Me.Clutter.Count(c => c.Sold == 0));
        Assert.True(g.Me.GarageDrawActive > 0);
    }

    [Fact]
    public void Selling_the_gaming_tower_pays_nine_thousand()
    {
        Game g = Garage1995();
        LotClutterState tower = Tower(g);
        g.Do(Command.SellGarageClutter(g.Id, tower.Id));
        Assert.Equal(69_000, g.Me.Cash);
        Assert.Equal(1, g.Me.Clutter.Count(c => c.Sold > 0));
    }

    [Fact]
    public void A_four_by_four_general_dev_room_costs_sixteen_tiles_times_price()
    {
        Game g = Garage1995();
        RoomTypeConfig type = g.C.RoomType("general-dev-room");
        long before = g.Me.Cash;
        g.Do(Command.BuildRoom(g.Id, "general-dev-room", 5, 0, 4, 4));
        Assert.Equal(before - 16 * type.BuildCostPerTile, g.Me.Cash);
    }

    [Fact]
    public void A_two_by_two_general_dev_room_is_refused()
    {
        Game g = Garage1995();
        g.Refused(Command.BuildRoom(g.Id, "general-dev-room", 5, 5, 2, 2), CommandError.TooSmall);
    }

    [Fact]
    public void Engineering_bay_is_off_the_day_one_build_menu()
    {
        Game g = Garage1995();
        g.Refused(Command.BuildRoom(g.Id, "engineering-bay", 5, 0, 3, 3), CommandError.DrawMenuLocked);
    }

    [Fact]
    public void Unseated_engineer_does_not_count_as_workforce()
    {
        Game g = Garage1995();
        int room = g.Do(Command.BuildRoom(g.Id, "general-dev-room", 5, 0, 4, 4)).CreatedId;
        g.Do(Command.PlaceRoomDoor(g.Id, room, 0, 0));
        long headsBefore = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering);
        StaffMember founder = g.Me.Staff.First(p => p.IsFounder > 0);
        Assert.True(headsBefore == 0, "founder without a matching desk should not produce engineering work");
        g.Do(Command.PlaceDesk(g.Id, room, "dev-desk-tier-1", 1, 1));
        g.Do(Command.SeatStaffAtDesk(g.Id, founder.Id, room, 1, 1));
        long headsAfter = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering);
        Assert.True(headsAfter > 0);
    }

    [Fact]
    public void Demolishing_a_room_refunds_half_the_tile_cost()
    {
        Game g = Garage1995();
        long before = g.Me.Cash;
        int room = g.Do(Command.BuildRoom(g.Id, "coffee-corner", 6, 0, 2, 2)).CreatedId;
        g.Do(Command.PlaceRoomDoor(g.Id, room, 0, 0));
        long afterBuild = g.Me.Cash;
        RoomTypeConfig type = g.C.RoomType("coffee-corner");
        long buildCost = 4 * type.BuildCostPerTile;
        Assert.Equal(before - buildCost - g.C.GarageDraw.DoorCost, afterBuild);
        g.Do(Command.DemolishRoom(g.Id, room));
        Assert.Equal(afterBuild + MathI.MulPermille(buildCost, g.C.GarageDraw.DemolishRefundPermille), g.Me.Cash);
    }

    [Fact]
    public void Save_and_load_keeps_rooms_desks_and_seating()
    {
        Game g = Garage1995();
        int room = g.Do(Command.BuildRoom(g.Id, "general-dev-room", 5, 0, 4, 4)).CreatedId;
        g.Do(Command.PlaceRoomDoor(g.Id, room, 0, 1));
        g.Do(Command.PlaceDesk(g.Id, room, "dev-desk-tier-1", 2, 2));
        StaffMember founder = g.Me.Staff.First(p => p.IsFounder > 0);
        g.Do(Command.SeatStaffAtDesk(g.Id, founder.Id, room, 2, 2));
        string save = g.Sim.Save();
        Game loaded = new Game(Simulation.Load(g.C, save));
        Assert.Single(loaded.Me.Rooms);
        Assert.Equal(room, loaded.Me.Rooms[0].Id);
        Assert.True(loaded.Me.Rooms[0].DoorPlaced);
        Assert.Single(loaded.Me.Desks);
        Assert.Equal(2, loaded.Me.Staff[0].SeatDeskLocalX);
    }
}