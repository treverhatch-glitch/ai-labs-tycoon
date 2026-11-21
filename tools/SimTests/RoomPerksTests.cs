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

/// <summary>The later rooms do something beyond seating people: each has a perk, read from the tiles a lab has built.</summary>
public class RoomPerksTests
{
    static Game Lab(string eraId, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = eraId, RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RolePerks.Clear(); if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000_000;
        return g;
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

    static RoomPerkConfig Perk(GameConfig c, string id) => c.RoomPerks.First(p => p.Id == id);

    // ---- the table

    [Fact]
    public void Every_perk_names_a_real_room_and_says_what_it_does()
    {
        GameConfig c = TestData.Config();
        Assert.NotEmpty(c.RoomPerks);
        foreach (RoomPerkConfig perk in c.RoomPerks)
        {
            Assert.NotNull(c.RoomType(perk.RoomId));
            Assert.False(string.IsNullOrWhiteSpace(perk.Blurb), perk.Id);
            Assert.InRange(perk.CapPermille, perk.PerTilePermille, 900);
            foreach (string s in perk.Sectors) Assert.NotNull(c.Sector(s));
        }
        Assert.Equal(c.RoomPerks.Count, c.RoomPerks.Select(p => p.Id).Distinct().Count());
    }

    [Fact]
    public void The_rooms_that_seat_the_data_roles_and_the_plain_halls_have_no_perk()
    {
        GameConfig c = TestData.Config();
        foreach (string room in new[] { "labelling-room", "data-team-room", "synthetic-data-lab", "gpu-hall", "cluster-hall", "ai-factory-hall", "server-closet" })
            Assert.DoesNotContain(c.RoomPerks, p => p.RoomId == room);
    }

    // ---- tiles and the cap

    [Fact]
    public void A_perk_is_the_tiles_of_its_room_times_the_share_and_two_rooms_of_one_type_add_up()
    {
        Game g = Lab("it-sees");
        RoomPerkConfig perk = Perk(g.C, "research-floor-speed");
        Assert.Equal(0, RoomPerks.Permille(g.C, g.Me, perk));
        g.Build("research-floor", 4, 4);
        Assert.Equal(16 * perk.PerTilePermille, RoomPerks.Permille(g.C, g.Me, perk));
        g.Build("research-floor", 5, 4);
        Assert.Equal(36, RoomPerks.Tiles(g.Me, "research-floor"));
        Assert.Equal(36 * perk.PerTilePermille, RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ResearchSpeed));
    }

    [Fact]
    public void A_perk_stops_at_its_cap_however_much_room_there_is()
    {
        Game g = Lab("it-sees");
        RoomPerkConfig perk = Perk(g.C, "research-floor-speed");
        g.Build("research-floor", 10, 6);
        Assert.True(60 * perk.PerTilePermille > perk.CapPermille);
        Assert.Equal(perk.CapPermille, RoomPerks.Permille(g.C, g.Me, perk));
    }

    [Fact]
    public void Pulling_the_room_down_takes_the_perk_with_it_the_same_week()
    {
        Game g = Lab("it-sees");
        int id = g.Build("research-floor", 4, 4);
        Assert.True(RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ResearchSpeed) > 0);
        g.Do(Command.DemolishRoom(g.Id, id));
        Assert.Equal(0, RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ResearchSpeed));
    }

    [Fact]
    public void The_lines_list_only_the_perks_of_rooms_the_lab_has()
    {
        Game g = Lab("agents");
        Assert.Empty(RoomPerks.Lines(g.C, g.Me));
        g.Build("agent-ops-room", 4, 4);
        List<RoomPerks.RoomPerkLine> lines = RoomPerks.Lines(g.C, g.Me);
        Assert.Single(lines);
        Assert.Equal("agent-ops-training", lines[0].Perk.Id);
        Assert.Equal(16, lines[0].Tiles);
        Assert.False(lines[0].AtCap);
        g.Build("agent-ops-room", 8, 8);
        Assert.True(RoomPerks.Lines(g.C, g.Me)[0].AtCap);
    }

    // ---- research speed

    static long ResearchInAWeek(Game g)
    {
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 4);
        g.Weeks(1);
        return g.Me.LastWeek.ResearchGained;
    }

    [Fact]
    public void A_research_floor_makes_the_same_researchers_earn_more_points()
    {
        Game plain = Lab("it-sees");
        long baseline = ResearchInAWeek(plain);
        Game floored = Lab("it-sees");
        floored.Build("research-floor", 6, 6);
        long boosted = ResearchInAWeek(floored);
        int perk = RoomPerks.Permille(floored.C, floored.Me, RoomPerkKind.ResearchSpeed);
        Assert.True(perk > 0);
        Assert.True(baseline > 0);
        Assert.Equal(MathI.MulPermille(baseline, 1000 + perk), boosted);
    }

    [Fact]
    public void The_later_research_rooms_stack_with_one_another()
    {
        Game g = Lab("upward");
        g.Build("research-floor", 4, 4);
        g.Build("harness-control-room", 4, 4);
        g.Build("launch-control", 4, 4);
        int expected = 16 * (Perk(g.C, "research-floor-speed").PerTilePermille + Perk(g.C, "harness-control-speed").PerTilePermille + Perk(g.C, "launch-control-speed").PerTilePermille);
        Assert.Equal(expected, RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ResearchSpeed));
    }

    [Fact]
    public void Research_with_no_perk_rooms_is_exactly_what_it_was()
    {
        Game with = Lab("it-sees");
        Game without = Lab("it-sees", c => c.RoomPerks.Clear());
        ResearchInAWeek(with);
        ResearchInAWeek(without);
        with.Weeks(20);
        without.Weeks(20);
        Assert.Equal(without.Sim.Digest(), with.Sim.Digest());
    }

    [Fact]
    public void With_nobody_doing_research_a_research_floor_gives_nothing()
    {
        Game g = Lab("it-sees");
        g.Build("research-floor", 6, 6);
        g.Weeks(5);
        Assert.Equal(0, g.Me.LastWeek.ResearchGained);
    }

    // ---- training speed

    [Fact]
    public void An_agent_ops_room_makes_training_compute_do_more_and_shortens_a_run()
    {
        Game plain = Lab("agents");
        plain.WithStarterLab();
        Game roomed = Lab("agents");
        roomed.WithStarterLab();
        roomed.Build("agent-ops-room", 6, 6);
        int perk = RoomPerks.Permille(roomed.C, roomed.Me, RoomPerkKind.TrainingSpeed);
        Assert.True(perk > 0);
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) + perk, TrainingSystem.SpeedPermille(roomed.C, roomed.Me));

        ModelLineConfig line = roomed.C.Line("mini");
        Assert.True(TrainingSystem.EstimateWeeks(roomed.C, roomed.State, roomed.Me, line, 200) <= TrainingSystem.EstimateWeeks(plain.C, plain.State, plain.Me, line, 200));
        int slow = plain.UntilTrained(plain.Train("mini", 200));
        int quick = roomed.UntilTrained(roomed.Train("mini", 200));
        Assert.True(quick < slow, quick + " weeks with the room, " + slow + " without");
    }

    // ---- incident catch

    /// <summary>A lab selling one released model that is bound to have incidents, with a sandbox wing of the given size (none for 0).</summary>
    static Game Testing(int wingSide, Action<GameConfig> tweak = null)
    {
        Game g = Lab("chat-boom", c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            if (tweak != null) tweak(c);
        });
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"));
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Me.Cash = 1_000_000_000_000_000;
        if (wingSide > 0) g.Build("sandbox-wing", wingSide, wingSide);
        return g;
    }

    static int Incidents(Game g) => g.State.Events.Count(e => e.Kind == EventKind.Incident);

    [Fact]
    public void A_sandbox_wing_catches_incidents_and_adds_to_the_red_teamers()
    {
        Game g = Testing(6, c => c.RolePerks.AddRange(TestData.Config().RolePerks));
        int room = RoomPerks.Permille(g.C, g.Me, RoomPerkKind.IncidentCut);
        Assert.Equal(36 * Perk(g.C, "sandbox-wing-catch").PerTilePermille, room);
        Assert.Equal(room, RolePerks.IncidentCatchPermille(g.C, g.Me));
        g.Hire("red-teamer", 3);
        Assert.Equal(room + 3 * g.C.RolePerks.First(p => p.Kind == RolePerkKind.IncidentCut).PerStaffPermille, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void Staff_and_rooms_together_never_catch_more_than_nine_in_ten()
    {
        Game g = Testing(0, c =>
        {
            c.RolePerks.AddRange(TestData.Config().RolePerks);
            foreach (RolePerkConfig p in c.RolePerks.Where(p => p.Kind == RolePerkKind.IncidentCut)) { p.PerStaffPermille = 400; p.CapPermille = 900; }
            foreach (RoomPerkConfig p in c.RoomPerks.Where(p => p.Kind == RoomPerkKind.IncidentCut)) { p.PerTilePermille = 20; p.CapPermille = 900; }
        });
        g.Build("sandbox-wing", 12, 12);
        g.Hire("red-teamer", 3);
        Assert.True(RoomPerks.Permille(g.C, g.Me, RoomPerkKind.IncidentCut) + RolePerks.Permille(g.C, g.Me, RolePerkKind.IncidentCut) > 900);
        Assert.Equal(900, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void A_wing_means_fewer_incidents_over_the_same_weeks()
    {
        Game bare = Testing(0), wing = Testing(12);
        bare.Weeks(100);
        wing.Weeks(100);
        int without = Incidents(bare), with = Incidents(wing);
        Assert.True(without > 40, "bare lab had " + without);
        Assert.True(with < without && with > 0, with + " with the wing, " + without + " without");
    }

    [Fact]
    public void Without_perk_rooms_incidents_come_exactly_as_they_did()
    {
        Game with = Testing(6), without = Testing(6, c => c.RoomPerks.Clear());
        with.Weeks(30);
        without.Weeks(30);
        Assert.NotEqual(without.Sim.Digest(), with.Sim.Digest());
        Game a = Testing(0), b = Testing(0, c => c.RoomPerks.Clear());
        a.Weeks(30);
        b.Weeks(30);
        Assert.Equal(b.Sim.Digest(), a.Sim.Digest());
    }

    // ---- sector appeal

    [Fact]
    public void A_robotics_arena_lifts_appeal_in_robot_sectors_only()
    {
        Game g = Lab("bodies");
        g.Build("robotics-arena", 6, 6);
        RoomPerkConfig perk = Perk(g.C, "robotics-arena-appeal");
        Assert.Equal(Math.Min(36 * perk.PerTilePermille, perk.CapPermille), RoomPerks.SectorAppealPermille(g.C, g.Me, "robotics-industry"));
        Assert.Equal(0, RoomPerks.SectorAppealPermille(g.C, g.Me, "consumer-assistant"));
        Assert.Equal(RoomPerks.SectorAppealPermille(g.C, g.Me, "robotics-industry"), RolePerks.SectorAppealPermille(g.C, g.Me, "robotics-industry"));
        Assert.Equal(0, RolePerks.SectorAppealPermille(g.C, g.Me, "consumer-assistant"));
    }

    [Fact]
    public void The_arena_stacks_with_roboticists()
    {
        Game g = Lab("bodies", c => c.RolePerks.AddRange(TestData.Config().RolePerks));
        g.Build("robotics-arena", 6, 6);
        int room = RoomPerks.SectorAppealPermille(g.C, g.Me, "robotics-industry");
        g.Hire("roboticist", 3);
        int staff = RolePerks.Permille(g.C, g.Me, RolePerkKind.SectorAppeal);
        Assert.True(staff > 0);
        Assert.Equal(room + RolePerks.Permille(g.C, g.Me, g.C.RolePerks.First(p => p.RoleId == "roboticist")), RolePerks.SectorAppealPermille(g.C, g.Me, "robotics-industry"));
    }

    [Fact]
    public void The_arena_shows_up_as_the_role_boost_in_the_appeal_parts_which_still_add_up()
    {
        Game g = Lab("bodies");
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "robotics-industry"));
        g.Me.Cash = 1_000_000_000_000_000;
        SectorConfig sector = g.C.Sector("robotics-industry");
        var before = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("robotics-industry"), g.Me, model);
        Assert.Equal(0, before.RoleBoost);
        g.Build("robotics-arena", 6, 6);
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("robotics-industry"), g.Me, model);
        Assert.True(parts.RoleBoost > 0);
        Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost, parts.Total);
    }

    // ---- saves, the validator, the data

    [Fact]
    public void Rooms_with_perks_save_and_load_and_play_on_the_same()
    {
        Game g = Lab("agents");
        g.WithStarterLab();
        g.Build("agent-ops-room", 5, 5);
        g.Build("sandbox-wing", 5, 5);
        g.Weeks(15);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(20);
        again.StepMany(20);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_perk_that_could_not_work_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.RoomPerks[0].RoomId = "bunker"), "bunker");
        AssertProblem(Problems(c => c.RoomPerks[0].PerTilePermille = 0), "perTilePermille");
        AssertProblem(Problems(c => c.RoomPerks[0].CapPermille = 1000), "cap");
        AssertProblem(Problems(c => c.RoomPerks[0].Blurb = ""), "blurb");
        AssertProblem(Problems(c => c.RoomPerks[0].Sectors.Add("consumer-assistant")), "only a SectorAppeal");
        AssertProblem(Problems(c => c.RoomPerks.First(p => p.Kind == RoomPerkKind.SectorAppeal).Sectors.Add("moon-mining")), "moon-mining");
        AssertProblem(Problems(c => c.RoomPerks.Add(c.RoomPerks[0])), "roomPerks");
        Assert.Empty(Problems(c => c.RoomPerks.Clear()));
    }

    [Fact]
    public void The_perk_table_is_strict_json_and_round_trips()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(c.ToJson(), GameConfig.FromJson(c.ToJson()).ToJson());
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"roomPerks\"", "\"roomPerksX\"")));
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"ResearchSpeed\"", "\"Telepathy\"")));
    }

    [Fact]
    public void The_room_perks_page_lists_every_perk_and_its_room()
    {
        GameConfig c = TestData.Config();
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "room-perks.md"));
        foreach (RoomPerkConfig perk in c.RoomPerks)
        {
            Assert.Contains("`" + perk.Id + "`", page);
            Assert.Contains("`" + perk.RoomId + "`", page);
        }
    }
}
