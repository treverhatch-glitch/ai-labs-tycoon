using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The HQ: rooms on the lot's grid, people at desks, GPUs in racks or on rent.</summary>
public class LabTests
{
    [Fact]
    public void A_new_lab_is_an_empty_garage_with_some_cash_in_1995()
    {
        Game g = Game.Solo();
        Assert.Equal(1995, g.Sim.Year);
        Assert.Equal("1995 M1 W1", g.Sim.DateLabel);
        Assert.Equal("garage", g.Me.LotId);
        Assert.Equal(g.C.PlayerStart.Cash, g.Me.Cash);
        Assert.Empty(g.Me.Rooms);
        Assert.Equal(0, LabQueries.TotalStaff(g.Me));
        Assert.Equal(0, LabQueries.TotalGpus(g.Me));
    }

    [Fact]
    public void The_calendar_has_four_week_months_and_twelve_month_years()
    {
        CalendarConfig cal = TestData.Config().Calendar;
        Assert.Equal(48, cal.WeeksPerYear);
        Assert.Equal("1995 M1 W4", GameDate.Label(cal, 3));
        Assert.Equal("1995 M2 W1", GameDate.Label(cal, 4));
        Assert.Equal("1995 M12 W4", GameDate.Label(cal, 47));
        Assert.Equal("1996 M1 W1", GameDate.Label(cal, 48));
        Assert.True(GameDate.IsMonthEnd(cal, 3));
        Assert.True(GameDate.IsYearEnd(cal, 47));
        Assert.False(GameDate.IsYearEnd(cal, 48));
        Assert.Equal(96, GameDate.TickOfYear(cal, 1997));
    }

    [Fact]
    public void Building_a_room_costs_money_by_the_tile_and_gives_desks()
    {
        Game g = Game.Solo();
        long before = g.Me.Cash;
        g.Do(Command.BuildRoom(g.Id, "research-lab", 0, 0, 6, 6));

        RoomTypeConfig type = g.C.RoomType("research-lab");
        Assert.Equal(before - 36 * type.BuildCostPerTile, g.Me.Cash);
        Assert.Equal(9, LabQueries.Seats(g.C, g.Me, "researcher"));
        Assert.Equal(0, LabQueries.Seats(g.C, g.Me, "engineer"));
    }

    [Fact]
    public void Rooms_cannot_overlap_hang_off_the_lot_or_be_too_small()
    {
        Game g = Game.Solo();
        g.Do(Command.BuildRoom(g.Id, "research-lab", 0, 0, 4, 4));

        g.Refused(Command.BuildRoom(g.Id, "sales-office", 3, 3, 3, 3), CommandError.Overlap);
        g.Refused(Command.BuildRoom(g.Id, "sales-office", 12, 0, 3, 3), CommandError.OutOfBounds);
        g.Refused(Command.BuildRoom(g.Id, "sales-office", -1, 5, 3, 3), CommandError.OutOfBounds);
        g.Refused(Command.BuildRoom(g.Id, "sales-office", 5, 5, 2, 3), CommandError.TooSmall);
        g.Refused(Command.BuildRoom(g.Id, "ball-pit", 5, 5, 3, 3), CommandError.UnknownId);

        // Rooms may touch: the tile right next to the first room is free.
        g.Do(Command.BuildRoom(g.Id, "sales-office", 4, 0, 3, 3));
    }

    [Fact]
    public void A_room_the_lab_cannot_pay_for_is_refused()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 1000);
        g.Refused(Command.BuildRoom(g.Id, "research-lab", 0, 0, 4, 4), CommandError.NotEnoughCash);
    }

    [Fact]
    public void People_need_a_desk_in_a_room_built_for_their_job()
    {
        Game g = Game.Solo();
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NoSeats);

        g.Build("research-lab", 4, 4);
        g.Refused(Command.Hire(g.Id, "engineer", 1), CommandError.NoSeats);
        g.Refused(Command.Hire(g.Id, "researcher", 5), CommandError.NoSeats);

        long before = g.Me.Cash;
        g.Hire("researcher", 4);
        Assert.Equal(4, LabQueries.StaffCount(g.Me, "researcher"));
        Assert.Equal(before - 4 * g.C.Role("researcher").HireCost, g.Me.Cash);
        Assert.Equal(4, LabQueries.StaffByFunction(g.C, g.Me, RoleFunction.Research));
    }

    [Fact]
    public void A_room_with_people_in_it_cannot_be_knocked_down_until_they_have_somewhere_to_sit()
    {
        Game g = Game.Solo();
        int first = g.Build("research-lab", 4, 4);
        g.Hire("researcher", 3);
        g.Refused(Command.DemolishRoom(g.Id, first), CommandError.StaffNeedSeats);

        g.Build("research-lab", 4, 3);
        g.Do(Command.DemolishRoom(g.Id, first));
        Assert.Single(g.Me.Rooms);
        Assert.Equal(3, LabQueries.Seats(g.C, g.Me, "researcher"));
    }

    [Fact]
    public void Letting_people_go_costs_severance_and_frees_their_desks()
    {
        Game g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 4);
        long before = g.Me.Cash;

        g.Do(Command.Fire(g.Id, "researcher", 3));
        Assert.Equal(1, LabQueries.StaffCount(g.Me, "researcher"));
        Assert.Equal(before - 3 * g.C.Role("researcher").Severance, g.Me.Cash);
        g.Refused(Command.Fire(g.Id, "researcher", 2), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void A_bigger_lot_keeps_every_room_where_it_stands()
    {
        Game g = Game.Solo();
        g.Build("research-lab", 6, 6);
        g.Build("engineering-bay", 6, 6);
        g.Refused(Command.BuildRoom(g.Id, "safety-lab", 0, 6, 5, 5), CommandError.OutOfBounds);

        long before = g.Me.Cash;
        g.Do(Command.BuyLot(g.Id));
        Assert.Equal("strip-mall", g.Me.LotId);
        Assert.Equal(before - g.C.Lot("strip-mall").Price, g.Me.Cash);
        Assert.Equal(0, g.Me.Rooms[0].X);
        Assert.Equal(6, g.Me.Rooms[1].X);

        g.Do(Command.BuildRoom(g.Id, "safety-lab", 0, 6, 5, 5));
    }

    [Fact]
    public void The_biggest_lot_is_the_end_of_the_road_for_now()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 5000000000);
        for (int i = 1; i < g.C.Lots.Count; i++) g.Do(Command.BuyLot(g.Id));
        g.Refused(Command.BuyLot(g.Id), CommandError.NoBiggerLot);
    }

    [Fact]
    public void Rented_gpus_arrive_at_once_and_cost_rent_every_week()
    {
        Game g = Game.Solo();
        g.Rent(10);
        Assert.Equal(10, LabQueries.TotalGpus(g.Me));
        Assert.Equal(g.C.PlayerStart.Cash, g.Me.Cash);

        g.Weeks(1);
        Assert.Equal(-10 * g.C.Compute.GpuGenerations[0].RentPerGpuWeek, g.Me.Ledger.LastWeek.GpuRent);

        g.Rent(0);
        g.Weeks(1);
        Assert.Equal(0, g.Me.Ledger.LastWeek.GpuRent);
        g.Refused(Command.SetRentedGpus(g.Id, -1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Owned_gpus_need_rack_space_in_a_server_closet()
    {
        Game g = Game.Solo();
        g.Refused(Command.BuyGpus(g.Id, 1), CommandError.NoGpuSlots);

        int closet = g.Build("server-closet", 2, 2);
        Assert.Equal(8, LabQueries.GpuSlots(g.C, g.Me));
        g.Refused(Command.BuyGpus(g.Id, 9), CommandError.NoGpuSlots);

        long before = g.Me.Cash;
        g.Do(Command.BuyGpus(g.Id, 8));
        Assert.Equal(before - 8 * g.C.Compute.GpuGenerations[0].BuyPrice, g.Me.Cash);
        g.Refused(Command.DemolishRoom(g.Id, closet), CommandError.GpusNeedSlots);

        g.Weeks(1);
        Assert.Equal(-8 * g.C.Compute.GpuGenerations[0].UpkeepPerGpuWeek, g.Me.Ledger.LastWeek.GpuUpkeep);
        Assert.Equal(0, g.Me.Ledger.LastWeek.GpuRent);
    }

    [Fact]
    public void Selling_gpus_gets_back_only_part_of_the_price()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 4));
        long before = g.Me.Cash;

        g.Do(Command.SellGpus(g.Id, 4));
        Assert.Equal(before + 4 * g.C.Compute.GpuGenerations[0].BuyPrice * g.C.Compute.ResalePermille / 1000, g.Me.Cash);
        g.Refused(Command.SellGpus(g.Id, 1), CommandError.NotEnoughGpus);
    }

    [Fact]
    public void Renting_is_cheaper_at_first_and_owning_wins_after_about_eighteen_months()
    {
        // Two labs that do nothing but hold ten GPUs: one rents, one buys. Same seed, same world.
        Game renter = Game.Solo(tweak: c => { c.PlayerStart.Cash = 1000000; c.Reliability.Enabled = 0; c.Tax.Enabled = 0; });
        Game owner = Game.Solo(tweak: c => { c.PlayerStart.Cash = 1000000; c.Reliability.Enabled = 0; c.Tax.Enabled = 0; });
        renter.Rent(10);
        owner.Build("server-closet", 3, 2);
        owner.Do(Command.BuyGpus(owner.Id, 10));
        long closetUpkeep = LabQueries.WeeklyRoomUpkeep(owner.C, owner.Me);

        int crossover = -1;
        for (int week = 1; week <= 120; week++)
        {
            renter.Weeks(1);
            owner.Weeks(1);
            // Leave the closet's build cost and upkeep out: this compares the GPUs themselves, as the design table does.
            long ownerCash = owner.Me.Cash + 6 * owner.C.RoomType("server-closet").BuildCostPerTile + closetUpkeep * week;
            if (crossover < 0 && ownerCash >= renter.Me.Cash) crossover = week;
        }

        Assert.InRange(crossover, 68, 76);
        Assert.Equal(owner.Me.LastWeek.Compute, renter.Me.LastWeek.Compute);
    }

    [Fact]
    public void A_bankrupt_or_unknown_lab_takes_no_orders()
    {
        Game g = Game.Solo();
        g.Refused(Command.Hire(999, "researcher", 1), CommandError.UnknownLab);
        g.Me.Bankrupt = true;
        g.Refused(Command.SetRentedGpus(g.Id, 5), CommandError.LabBankrupt);
    }

    [Fact]
    public void Everything_that_happens_reads_as_a_plain_sentence()
    {
        Game g = Game.New(seed: 21, tweak: c => c.EventLogMax = 100000);
        g.Do(Command.BuildRoom(g.Id, "engineering-bay", 0, 0, 3, 3));
        Assert.EndsWith("built an Engineering Bay for $3600.", EventText.Describe(g.C, g.State, g.State.Events[^1]));
        g.Do(Command.DemolishRoom(g.Id, g.Me.Rooms[0].Id));
        g.Autopilot(6 * 48);

        var kinds = new System.Collections.Generic.HashSet<EventKind>();
        foreach (GameEvent e in g.State.Events)
        {
            string text = EventText.Describe(g.C, g.State, e);
            Assert.Matches(@"^\d{4} M\d+ W\d: \S", text);
            Assert.EndsWith(".", text);
            Assert.DoesNotContain("{", text);
            Assert.DoesNotContain("  ", text);
            kinds.Add(e.Kind);
        }
        Assert.Contains(EventKind.TrainingFinished, kinds);
        Assert.Contains(EventKind.Incident, kinds);
        Assert.Contains(EventKind.GauntletCrown, kinds);
        Assert.Contains(EventKind.LotBought, kinds);
    }
}
