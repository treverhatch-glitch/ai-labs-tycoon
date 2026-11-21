using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class GauntletExpansionBracketTests
{
    const string Arena = "robotics-arena";
    const string Uplink = "orbital-uplink";
    const string Robotics = "robotics-industry";
    const string MachineScience = "machine-science";

    static Game TwoLabYear(int year)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.Gauntlet.JitterPermille = 0), new NewGameOptions { Seed = 22, StartEraId = "punchline" }));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        Assert.True(g.State.Labs.Count >= 2);
        return g;
    }

    static ModelState Release(Game g, LabState lab, int capability, string sector)
    {
        var model = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = lab.Models.Count + 1,
            Name = lab.FamilyName + " v" + lab.Models.Count, Status = ModelStatus.Trained,
            Capability = capability, CapBase = capability, Alignment = 700, DisplayedAlignment = 700,
        };
        lab.Models.Add(model);
        g.Do(Command.ReleaseModel(lab.Id, model.Id, sector));
        return model;
    }

    [Fact]
    public void Robotics_arena_tips_sector_bracket_against_higher_rival()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Release(g, g.Me, 170_000, Robotics);
        Release(g, g.State.Labs[1], 190_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.Me.Id, g.State.Sectors.Find(s => s.Id == Robotics).ChampionLabId);
    }

    [Fact]
    public void Without_arena_higher_capability_rival_wins()
    {
        Game g = TwoLabYear(2029);
        Release(g, g.Me, 170_000, Robotics);
        Release(g, g.State.Labs[1], 190_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.State.Labs[1].Id, g.State.Sectors.Find(s => s.Id == Robotics).ChampionLabId);
    }

    [Fact]
    public void Expansion_bonus_only_in_matching_sectors()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Release(g, g.Me, 170_000, "game-ai");
        Release(g, g.State.Labs[1], 190_000, "game-ai");
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.State.Labs[1].Id, g.State.Sectors.Find(s => s.Id == "game-ai").ChampionLabId);
    }

    [Fact]
    public void Before_bodies_era_expansion_does_not_flip_bracket()
    {
        Game g = TwoLabYear(2028);
        g.Me.Rooms.Add(new RoomState { TypeId = Arena, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        const string punchline = "game-ai";
        Release(g, g.Me, 170_000, punchline);
        Release(g, g.State.Labs[1], 190_000, punchline);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.State.Labs[1].Id, g.State.Sectors.Find(s => s.Id == punchline).ChampionLabId);
    }

    [Fact]
    public void Crown_bracket_still_uses_raw_capability()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Release(g, g.Me, 160_000, Robotics);
        Release(g, g.State.Labs[1], 220_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.State.Labs[1].Id, g.State.Gauntlets.Last().CrownLabId);
    }

    [Fact]
    public void Bracket_score_reflects_expansion_scaled_quality()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = Release(g, g.Me, 200_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        SectorConfig sector = g.C.Sector(Robotics);
        long q = LabQueries.Quality(g.C, sector, model, g.State.Tick);
        long expected = MathI.MulPermille(q, MathI.Permille + g.C.Expansions.Bodies.AppealBonusPermille);
        GauntletEntry entry = g.State.Gauntlets.Last().Entries.Find(e => e.Bracket == Robotics && e.LabId == g.Id);
        Assert.NotNull(entry);
        Assert.Equal(expected, entry.Score);
    }

    [Fact]
    public void Orbital_uplink_boosts_machine_science_bracket()
    {
        Game g = TwoLabYear(2035);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Release(g, g.Me, 225_000, MachineScience);
        Release(g, g.State.Labs[1], 235_000, MachineScience);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.Me.Id, g.State.Sectors.Find(s => s.Id == MachineScience).ChampionLabId);
    }

    [Fact]
    public void Gauntlet_record_survives_save_load()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Release(g, g.Me, 200_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        int champion = g.State.Sectors.Find(s => s.Id == Robotics).ChampionLabId;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(champion, loaded.State.Sectors.Find(s => s.Id == Robotics).ChampionLabId);
    }

    [Fact]
    public void Market_appeal_and_bracket_both_use_expansion()
    {
        Game g = TwoLabYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = Release(g, g.Me, 200_000, Robotics);
        SectorState sectorState = g.State.Sectors.Find(s => s.Id == Robotics);
        SectorConfig sector = g.C.Sector(Robotics);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ExpansionBoost > 0);
        Release(g, g.State.Labs[1], 190_000, Robotics);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.Me.Id, sectorState.ChampionLabId);
    }
}