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

/// <summary>Rooms that age: one era older each time an era begins, dearer and gloomier until the lab pays to refurbish them.</summary>
public class RoomAgeTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.RoomPerks.Clear();
            if (tweak != null) tweak(c);
        });
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static RoomState Room(Game g, string type, int w, int h)
    {
        return g.Me.Room(g.Build(type, w, h));
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the table

    [Fact]
    public void The_table_has_five_stages_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "fresh", "settled", "dated", "beige", "museum-piece" }, c.Ageing.Stages.Select(s => s.IdValue).ToArray());
        Assert.Equal(250, c.Ageing.RefurbishCostPermille);
        Assert.Equal(60, c.Ageing.MaxMoralePenalty);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Theory]
    [InlineData(0, "fresh")]
    [InlineData(1, "settled")]
    [InlineData(2, "dated")]
    [InlineData(3, "beige")]
    [InlineData(4, "beige")]
    [InlineData(5, "museum-piece")]
    [InlineData(9, "museum-piece")]
    public void A_room_has_the_stage_with_the_highest_start_at_or_below_its_age(int eras, string stage)
    {
        Assert.Equal(stage, RoomAge.Stage(TestData.Config(), eras).IdValue);
    }

    // ---- ageing

    [Fact]
    public void A_new_room_is_fresh_and_costs_nothing_extra()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        Assert.Equal(0, room.EraAge);
        var report = RoomAge.Of(g.C, g.Me);
        Assert.Equal("fresh", report.Rooms[0].StageId);
        Assert.Equal(0, report.MoralePenalty);
        Assert.Equal(0, report.ExtraUpkeepPerWeek);
        Assert.Equal(0, report.Rooms[0].RefurbishCost);
    }

    [Fact]
    public void A_new_era_makes_every_room_of_the_players_lab_one_era_older()
    {
        Game g = Lab();
        RoomState a = Room(g, "research-lab", 4, 4);
        RoomState b = Room(g, "server-closet", 3, 3);
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(1, a.EraAge);
        Assert.Equal(1, b.EraAge);
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(2, a.EraAge);
    }

    [Fact]
    public void Rivals_rooms_do_not_age()
    {
        Game g = Game.New(seed: 3);
        g.Weeks(60);
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        if (rival.Rooms.Count == 0) rival.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "research-lab", X = 0, Y = 0, Width = 4, Height = 4 });
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.All(rival.Rooms, r => Assert.Equal(0, r.EraAge));
        rival.Rooms[0].EraAge = 4;
        Assert.Equal(0, RoomAge.Eras(rival, rival.Rooms[0]));
        Assert.Equal(0, RoomAge.UpkeepExtraPermille(g.C, rival, rival.Rooms[0]));
        Assert.Equal(0, RoomAge.MoralePenalty(g.C, rival));
    }

    [Fact]
    public void Passing_into_a_real_era_ages_the_room_and_a_second_one_makes_it_dated()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        g.Until(() => g.Tick >= EraSystem.StartTick(g.C, g.C.Era("punchline")) + 1, 800, "the Punchline era");
        Assert.Equal(1, room.EraAge);
        g.Until(() => g.Tick >= EraSystem.StartTick(g.C, g.C.Era("it-sees")) + 1, 800, "the It Sees era");
        Assert.Equal(2, room.EraAge);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RoomDated && e.SubjectId == room.Id && e.Amount == 2 && e.Ref == "research-lab");
    }

    [Fact]
    public void The_first_era_begins_without_aging_anything()
    {
        Game g = Lab();
        Room(g, "research-lab", 4, 4);
        Assert.Equal(0, g.Me.Rooms[0].EraAge);
        g.Weeks(5);
        Assert.Equal(0, g.Me.Rooms[0].EraAge);
    }

    [Fact]
    public void A_room_is_talked_about_only_when_it_gets_worse_in_a_way_that_costs()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RoomDated);
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.RoomDated));
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.RoomDated));
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.RoomDated));
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Equal(3, g.State.Events.Count(e => e.Kind == EventKind.RoomDated));
    }

    // ---- upkeep

    [Fact]
    public void A_dated_room_costs_more_to_keep()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        long plain = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        room.EraAge = 2;
        long dated = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        Assert.Equal(plain + plain * 150 / 1000, dated);
        room.EraAge = 3;
        Assert.Equal(plain + plain * 300 / 1000, LabQueries.WeeklyRoomUpkeep(g.C, g.Me));
        room.EraAge = 6;
        Assert.Equal(plain + plain * 500 / 1000, LabQueries.WeeklyRoomUpkeep(g.C, g.Me));
    }

    [Fact]
    public void The_extra_upkeep_is_charged_on_the_ledger_each_week()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        room.EraAge = 3;
        g.Weeks(1);
        Assert.Equal(-LabQueries.WeeklyRoomUpkeep(g.C, g.Me), g.Me.Ledger.LastWeek.RoomUpkeep);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_server_closet_ages_and_costs_more_too()
    {
        Game g = Lab();
        RoomState room = Room(g, "server-closet", 3, 3);
        long plain = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        room.EraAge = 3;
        Assert.True(LabQueries.WeeklyRoomUpkeep(g.C, g.Me) > plain);
    }

    // ---- morale

    [Fact]
    public void A_dated_room_takes_morale_off_where_its_people_settle()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        Assert.Equal(500, Workplace.RestingMorale(g.C, g.Me));
        room.EraAge = 2;
        Assert.Equal(480, Workplace.RestingMorale(g.C, g.Me));
        room.EraAge = 3;
        Assert.Equal(460, Workplace.RestingMorale(g.C, g.Me));
        room.EraAge = 5;
        Assert.Equal(443, Workplace.RestingMorale(g.C, g.Me));
    }

    [Fact]
    public void The_penalty_is_the_tile_weighted_average_of_the_rooms_that_seat_people()
    {
        Game g = Lab();
        RoomState big = Room(g, "research-lab", 6, 4);
        RoomState small = Room(g, "engineering-bay", 4, 3);
        big.EraAge = 0;
        small.EraAge = 5;
        // 24 fresh tiles and 12 museum-piece tiles: (24 * 0 + 12 * 60) / 36 = 20.
        Assert.Equal(20, RoomAge.MoralePenalty(g.C, g.Me));
    }

    [Fact]
    public void A_room_that_seats_nobody_does_not_count_for_morale()
    {
        Game g = Lab();
        RoomState closet = Room(g, "server-closet", 3, 3);
        closet.EraAge = 6;
        Assert.Equal(0, RoomAge.MoralePenalty(g.C, g.Me));
    }

    [Fact]
    public void The_penalty_never_passes_its_cap()
    {
        Game g = Lab(c => c.Ageing.MaxMoralePenalty = 25);
        Room(g, "research-lab", 4, 4).EraAge = 6;
        Assert.Equal(25, RoomAge.MoralePenalty(g.C, g.Me));
    }

    [Fact]
    public void The_peoples_morale_really_settles_lower()
    {
        Game g = Lab();
        Room(g, "research-lab", 4, 4);
        g.Hire("researcher", 2);
        foreach (RoomState r in g.Me.Rooms) r.EraAge = 3;
        g.Weeks(80);
        Assert.All(g.Me.Staff.Where(p => p.TraitId == "steady"), p => Assert.Equal(460, p.Morale));
    }

    // ---- refurbishing

    [Fact]
    public void Refurbishing_costs_a_quarter_of_the_build_price_and_makes_the_room_new_again()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        room.EraAge = 3;
        long cash = g.Me.Cash;
        long cost = 400L * 16 * 250 / 1000;
        Assert.Equal(cost, RoomAge.RefurbishCost(g.C, room));
        g.Do(Command.RefurbishRoom(g.Id, room.Id));
        Assert.Equal(0, room.EraAge);
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(500, Workplace.RestingMorale(g.C, g.Me));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RoomRefurbished && e.SubjectId == room.Id && e.Amount == cost);
    }

    [Fact]
    public void Refurbishing_goes_on_the_construction_line_so_cash_is_explained()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        room.EraAge = 2;
        long before = g.Me.Ledger.Lifetime.Construction;
        g.Do(Command.RefurbishRoom(g.Id, room.Id));
        Assert.Equal(before - 400L * 16 * 250 / 1000, g.Me.Ledger.Lifetime.Construction);
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_refurbished_room_costs_the_plain_upkeep_again()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        long plain = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        room.EraAge = 4;
        Assert.True(LabQueries.WeeklyRoomUpkeep(g.C, g.Me) > plain);
        g.Do(Command.RefurbishRoom(g.Id, room.Id));
        Assert.Equal(plain, LabQueries.WeeklyRoomUpkeep(g.C, g.Me));
    }

    [Fact]
    public void Refurbishing_is_refused_for_a_room_that_is_up_to_date_unknown_or_unaffordable_and_changes_nothing()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        g.Refused(Command.RefurbishRoom(g.Id, room.Id), CommandError.InvalidArgument);
        g.Refused(Command.RefurbishRoom(g.Id, 99999), CommandError.UnknownId);
        room.EraAge = 3;
        g.Me.Cash = 10;
        g.Refused(Command.RefurbishRoom(g.Id, room.Id), CommandError.NotEnoughCash);
        Assert.Equal(3, room.EraAge);
    }

    // ---- the report and the chat

    [Fact]
    public void The_report_lists_every_room_with_its_stage_and_what_it_would_cost_to_fix()
    {
        Game g = Lab();
        RoomState a = Room(g, "research-lab", 4, 4);
        RoomState b = Room(g, "engineering-bay", 4, 4);
        long plain = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        a.EraAge = 3;
        var report = RoomAge.Of(g.C, g.Me);
        Assert.Equal(2, report.Rooms.Count);
        Assert.Equal("beige", report.Rooms[0].StageId);
        Assert.Equal(3, report.Rooms[0].Eras);
        Assert.Equal(40, report.Rooms[0].MoralePenalty);
        Assert.Equal(400L * 16 * 250 / 1000, report.Rooms[0].RefurbishCost);
        Assert.Equal("fresh", report.Rooms[1].StageId);
        Assert.Equal(0, report.Rooms[1].RefurbishCost);
        Assert.Equal(20, report.MoralePenalty);
        a.EraAge = 0;
        long fresh = LabQueries.WeeklyRoomUpkeep(g.C, g.Me);
        a.EraAge = 3;
        Assert.Equal(LabQueries.WeeklyRoomUpkeep(g.C, g.Me) - fresh, report.ExtraUpkeepPerWeek);
        Assert.Equal(plain, fresh);
    }

    [Fact]
    public void Staff_complain_about_a_dated_room_in_the_chat_and_are_pleased_with_a_refurbished_one()
    {
        Game g = Lab();
        g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "researcher", TraitId = "steady", Morale = 500 });
        RoomState room = Room(g, "research-lab", 4, 4);
        g.State.Events.Clear();
        RoomAge.OnEraBegan(g.C, g.State);
        RoomAge.OnEraBegan(g.C, g.State);
        Assert.Contains("The Research Lab has gone beige", CompanyChat.Feed(g.C, g.State, g.Me).Last().Text);
        g.Do(Command.RefurbishRoom(g.Id, room.Id));
        Assert.Contains("done up", CompanyChat.Feed(g.C, g.State, g.Me).Last().Text);
    }

    [Fact]
    public void The_events_have_plain_english()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        RoomAge.OnEraBegan(g.C, g.State);
        RoomAge.OnEraBegan(g.C, g.State);
        GameEvent dated = g.State.Events.Last(e => e.Kind == EventKind.RoomDated);
        Assert.Contains("is now dated, 2 eras behind", EventText.Describe(g.C, g.State, dated));
        g.Do(Command.RefurbishRoom(g.Id, room.Id));
        GameEvent done = g.State.Events.Last(e => e.Kind == EventKind.RoomRefurbished);
        Assert.Contains("refurbished its Research Lab for $", EventText.Describe(g.C, g.State, done));
    }

    // ---- saving

    [Fact]
    public void A_rooms_age_survives_a_save_and_a_load()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        room.EraAge = 3;
        g.Weeks(3);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(3, again.Player.Room(room.Id).EraAge);
        for (int i = 0; i < 20; i++) { g.Sim.Step(); again.Step(); }
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_every_room_fresh()
    {
        Game g = Lab();
        Room(g, "research-lab", 4, 4);
        g.Weeks(5);
        string old = SaveSchemaPretend.At(g.C, 29, g.Sim.Save());
        Assert.DoesNotContain("eraAge", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        Assert.All(loaded.State.Labs.SelectMany(l => l.Rooms), r => Assert.Equal(0, r.EraAge));
    }

    [Fact]
    public void A_save_with_a_room_older_than_there_are_eras_is_reported()
    {
        Game g = Lab();
        RoomState room = Room(g, "research-lab", 4, 4);
        room.EraAge = g.C.Eras.Count + 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("eras old"));
        room.EraAge = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("eras old"));
    }

    [Fact]
    public void Two_runs_of_the_same_game_land_on_the_same_digest()
    {
        Func<string> run = () =>
        {
            Game g = Lab();
            RoomState room = Room(g, "research-lab", 4, 4);
            RoomAge.OnEraBegan(g.C, g.State);
            RoomAge.OnEraBegan(g.C, g.State);
            g.Do(Command.RefurbishRoom(g.Id, room.Id));
            g.Weeks(5);
            return g.Sim.Digest();
        };
        Assert.Equal(run(), run());
    }

    // ---- the validator

    [Fact]
    public void The_validator_names_each_ageing_number_that_is_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.Ageing.RefurbishCostPermille = -1), "ageing.refurbishCostPermille");
        AssertProblem(Problems(c => c.Ageing.RefurbishCostPermille = 3001), "ageing.refurbishCostPermille");
        AssertProblem(Problems(c => c.Ageing.MaxMoralePenalty = 1001), "ageing.maxMoralePenalty");
        AssertProblem(Problems(c => c.Ageing.Stages.Clear()), "ageing.stages is empty");
        AssertProblem(Problems(c => c.Ageing.Stages[0].MinEras = 1), "first age stage");
        AssertProblem(Problems(c => c.Ageing.Stages[2].MinEras = 1), "does not start above");
        AssertProblem(Problems(c => c.Ageing.Stages[2].UpkeepExtraPermille = -1), "upkeepExtraPermille");
        AssertProblem(Problems(c => c.Ageing.Stages[2].MoralePenalty = 1001), "moralePenalty");
        AssertProblem(Problems(c => c.Ageing.Stages[3].UpkeepExtraPermille = 100), "costs less than the stage before");
        AssertProblem(Problems(c => c.Ageing.Stages[1].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Ageing.Stages[1].Blurb = ""), "has no blurb");
        AssertProblem(Problems(c => c.Ageing.Stages[1].IdValue = "fresh"), "ageing.stages");
    }
}
