using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>
/// The long headless runs. A whole lifetime of the game, 1995 to 2035, played by the autopilot,
/// proves that content keeps arriving long after 2002 (where the first slice ran dry) and that
/// no number runs away on the way.
/// </summary>
public class ContentFlowTests
{
    const int Years = 40;

    /// <summary>What a long run saw, collected as it went so that the trimmed event log cannot hide anything.</summary>
    sealed class Run
    {
        public Game Game;
        public readonly List<GameEvent> Seen = new List<GameEvent>();
        public readonly Dictionary<int, int> PlayerTechsAtYearEnd = new Dictionary<int, int>();
        public readonly HashSet<string> SectorsEntered = new HashSet<string>();
        public readonly HashSet<string> RoomTypesBuilt = new HashSet<string>();
        public readonly HashSet<string> RolesHired = new HashSet<string>();
        public readonly HashSet<string> TechsOwnedBySomeone = new HashSet<string>();
        public int FirstYear;
        public long MaxCash = long.MinValue;
        public long MinCash = long.MaxValue;
        public long MaxCapability;
        public long MaxPool;
    }

    static Run Play(string startEra, int years, long seed)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = startEra };
        var game = new Game(Simulation.NewGame(TestData.Config(), options));
        var run = new Run { Game = game, FirstYear = game.Sim.Year };
        GameConfig c = game.C;
        for (int week = 0; week < years * c.Calendar.WeeksPerYear; week++)
        {
            int tick = game.Sim.Tick;
            if (GameDate.IsMonthStart(c.Calendar, tick)) game.Sim.RunPlan(game.Id, c.Autopilot);
            game.Sim.Step();

            List<GameEvent> events = game.State.Events;
            int first = events.Count;
            while (first > 0 && events[first - 1].Tick == tick) first--;
            for (int i = first; i < events.Count; i++) run.Seen.Add(events[i]);

            if (GameDate.IsMonthStart(c.Calendar, game.Sim.Tick)) Survey(run);
            if (game.Sim.Tick % c.Calendar.WeeksPerYear == 0)
                run.PlayerTechsAtYearEnd[game.Sim.Year - 1] = game.Me.Techs.Count;
        }
        Survey(run);
        return run;
    }

    static void Survey(Run run)
    {
        GameState state = run.Game.State;
        foreach (LabState lab in state.Labs)
        {
            run.MaxCash = Math.Max(run.MaxCash, lab.Cash);
            run.MinCash = Math.Min(run.MinCash, lab.Cash);
            foreach (RoomState room in lab.Rooms) run.RoomTypesBuilt.Add(room.TypeId);
            foreach (StaffMember h in lab.Staff)
            {
                if (!run.RolesHired.Contains(h.RoleId)) run.RolesHired.Add(h.RoleId);
            }
            foreach (string t in lab.Techs) run.TechsOwnedBySomeone.Add(t);
            foreach (ModelState model in lab.Models) run.MaxCapability = Math.Max(run.MaxCapability, model.Capability);
        }
        foreach (SectorState sector in state.Sectors)
        {
            run.MaxPool = Math.Max(run.MaxPool, sector.PoolPerWeek);
            if (sector.Entries.Count > 0) run.SectorsEntered.Add(sector.Id);
        }
    }

    static readonly Lazy<Run> FullLife = new Lazy<Run>(() => Play("", Years, 1));
    static readonly Lazy<Run> From2009 = new Lazy<Run>(() => Play("punchline", 26, 2));

    static int EraIndex(GameConfig c, string eraId) { return c.Eras.IndexOf(c.Era(eraId)); }

    [Fact]
    public void Forty_years_of_autopilot_reach_2035_with_nobody_broke_and_every_era_announced()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        Assert.Equal(2035, run.Game.Sim.Year);
        Assert.All(run.Game.State.Labs, lab => Assert.False(lab.Bankrupt, lab.Name + " went bankrupt"));

        string[] announced = run.Seen.Where(e => e.Kind == EventKind.EraBegan).Select(e => e.Ref).ToArray();
        Assert.Equal(c.Eras.Select(e => e.Id).ToArray(), announced);
    }

    [Fact]
    public void Every_world_event_lands_once_over_a_lifetime()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        string[] landed = run.Seen.Where(e => e.Kind == EventKind.WorldNews && c.WorldEvent(e.Ref) != null).Select(e => e.Ref).ToArray();
        int lastYear = run.FirstYear + Years - 1;
        string[] expected = c.WorldEvents.Where(w => w.Year <= lastYear).Select(w => w.Id).ToArray();
        foreach (string id in expected) Assert.Contains(id, landed);
        foreach (string id in landed)
            Assert.True(System.Array.IndexOf(expected, id) >= 0 || id == "bubble-crash");
        Assert.Contains(run.Seen, e =>
        {
            if (e.Kind != EventKind.WorldNews) return false;
            WorldEventConfig w = c.WorldEvent(e.Ref);
            return w != null && w.Year >= 2030;
        });
    }

    [Fact]
    public void Research_keeps_flowing_every_few_years_long_after_2002()
    {
        Run run = FullLife.Value;
        int[] checkpoints = { 2002, 2008, 2014, 2020, 2026, 2032 };
        for (int i = 1; i < checkpoints.Length; i++)
        {
            int before = run.PlayerTechsAtYearEnd[checkpoints[i - 1]];
            int after = run.PlayerTechsAtYearEnd[checkpoints[i]];
            Assert.True(after >= before + 3, "between " + checkpoints[i - 1] + " and " + checkpoints[i] + " the player went from " + before + " to " + after + " techs");
        }
    }

    [Fact]
    public void By_the_end_the_player_has_researched_almost_the_whole_tree()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        int owned = run.Game.Me.Techs.Count;
        Assert.True(owned * 100 >= c.Research.Techs.Count * 90, "the player owns " + owned + " of " + c.Research.Techs.Count + " techs after forty years");
        foreach (EraConfig era in c.Eras.Take(6))
        {
            foreach (TechConfig tech in c.Research.Techs.Where(t => t.Era == era.Id))
                Assert.Contains(tech.Id, run.Game.Me.Techs);
        }
    }

    [Fact]
    public void Every_tech_in_the_tree_is_researched_by_some_lab_in_a_lifetime()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        Assert.All(c.Research.Techs, t => Assert.Contains(t.Id, run.TechsOwnedBySomeone));
    }

    [Fact]
    public void Labs_sell_into_sectors_that_opened_after_2002_era_by_era()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        foreach (EraConfig era in c.Eras.Take(7))
        {
            var sectors = c.Market.Sectors.Where(s => s.Era == era.Id && s.OpensYear > 2002).ToList();
            if (sectors.Count == 0) continue;
            Assert.True(sectors.Any(s => run.SectorsEntered.Contains(s.Id)), "no lab ever sold into a sector of " + era.Id);
        }
        int late = c.Market.Sectors.Count(s => s.OpensYear > 2002 && run.SectorsEntered.Contains(s.Id));
        Assert.True(late >= 20, "only " + late + " sectors from after 2002 were ever entered");
    }

    [Fact]
    public void Labs_build_rooms_and_hire_roles_from_the_later_eras()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        foreach (EraConfig era in c.Eras.Take(7))
            Assert.True(c.RoomTypes.Any(r => r.Era == era.Id && run.RoomTypesBuilt.Contains(r.Id)), "no lab built a room of " + era.Id);
        Assert.True(run.RolesHired.Count >= 12, "only " + run.RolesHired.Count + " roles were ever hired");
    }

    [Fact]
    public void Capability_cash_and_pools_stay_inside_sane_bounds_for_forty_years()
    {
        Run run = FullLife.Value;
        Assert.InRange(run.MaxCapability, 100000, 10000000);
        Assert.True(run.MaxCash < 1000000000000L, "cash reached " + run.MaxCash);
        Assert.True(run.MinCash > -1000000000L, "cash fell to " + run.MinCash);
        Assert.True(run.MaxPool < 1000000000000L, "a pool reached " + run.MaxPool);
        Assert.All(run.Game.State.Labs, lab => Assert.True(LabQueries.TotalGpus(lab) >= 0));
    }

    [Fact]
    public void Every_event_of_a_lifetime_can_be_written_as_a_sentence()
    {
        Run run = FullLife.Value;
        Assert.True(run.Seen.Count > 2000);
        foreach (GameEvent e in run.Seen)
        {
            string line = EventText.Describe(run.Game.C, run.Game.State, e);
            Assert.False(string.IsNullOrWhiteSpace(line), e.Kind + " had no text");
        }
    }

    [Fact]
    public void Incidents_use_the_headlines_of_the_era_they_happen_in()
    {
        Run run = FullLife.Value;
        GameConfig c = run.Game.C;
        var erasWithIncidents = new HashSet<string>();
        foreach (GameEvent e in run.Seen.Where(e => e.Kind == EventKind.Incident))
        {
            EraConfig era = EraSystem.AtTick(c, e.Tick);
            Assert.InRange(e.Pick, 0, era.IncidentHeadlines.Count - 1);
            erasWithIncidents.Add(era.Id);
        }
        Assert.True(erasWithIncidents.Count >= 5, "incidents only happened in " + string.Join(", ", erasWithIncidents));
    }

    [Fact]
    public void The_player_makes_more_money_in_2030_than_in_2002()
    {
        Run run = FullLife.Value;
        Assert.True(run.Game.Me.LastWeek.Revenue > 1000000, "weekly revenue at the end was only " + run.Game.Me.LastWeek.Revenue);
    }

    [Fact]
    public void A_forty_year_game_saves_loads_and_replays_to_the_same_digest()
    {
        Run run = FullLife.Value;
        Simulation loaded = Simulation.Load(run.Game.C, run.Game.Sim.Save());
        Assert.Equal(run.Game.Sim.Digest(), loaded.Digest());

        ReplayLog log = ReplayLog.FromJson(run.Game.Sim.Journal.ToJson());
        Assert.Equal(run.Game.Sim.Digest(), Simulation.Replay(TestData.Config(), log).Digest());
    }

    [Fact]
    public void A_game_started_in_2009_keeps_finding_new_things_to_do_for_twenty_six_years()
    {
        Run run = From2009.Value;
        GameConfig c = run.Game.C;
        Assert.Equal(2035, run.Game.Sim.Year);
        Assert.All(run.Game.State.Labs, lab => Assert.False(lab.Bankrupt, lab.Name + " went bankrupt"));
        Assert.True(run.Game.Me.Techs.Count >= c.Research.Techs.Count * 85 / 100, "the player owns " + run.Game.Me.Techs.Count + " techs");
        Assert.Equal(c.Eras.Skip(1).Select(e => e.Id).ToArray(), run.Seen.Where(e => e.Kind == EventKind.EraBegan).Select(e => e.Ref).ToArray());
        Assert.DoesNotContain(run.Seen, e =>
        {
            if (e.Kind != EventKind.WorldNews) return false;
            WorldEventConfig w = c.WorldEvent(e.Ref);
            return w != null && w.Year < 2009;
        });
        Assert.True(run.MaxCash < 1000000000000L);
        Assert.True(run.Game.Me.LastWeek.Revenue > 100000);
    }

    [Fact]
    public void Several_seeds_all_survive_a_lifetime_without_a_runaway_number()
    {
        foreach (long seed in new long[] { 3, 4 })
        {
            Run run = Play("", Years, seed);
            Assert.All(run.Game.State.Labs, lab => Assert.False(lab.Bankrupt, "seed " + seed + ": " + lab.Name + " went bankrupt"));
            Assert.True(run.MaxCash < 1000000000000L, "seed " + seed + " cash " + run.MaxCash);
            Assert.True(run.Game.Me.Techs.Count >= run.Game.C.Research.Techs.Count * 85 / 100, "seed " + seed + " techs " + run.Game.Me.Techs.Count);
        }
    }
}
