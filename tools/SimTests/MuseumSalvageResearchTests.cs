using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class MuseumSalvageResearchTests
{
    static Game SoloMuseumSalvageOnly() =>
        Game.Solo(1, c => c.Endings.Museum.ResearchPointsPerFullLine = 0);

    static void AddRetired(Game g, string line, int capability, int version)
    {
        int when = g.State.Tick - 10_000 + version * 10;
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = line, Version = version, Name = "v" + version,
            Status = ModelStatus.Retired, Capability = capability, CapBase = capability,
            StartedTick = when, TrainedTick = when, ReleasedTick = when,
        });
    }

    [Fact]
    public void Prune_grants_research_from_oldest_retired_models()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", 100, v);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long expected = 2 * MathI.MulPermille(100, g.C.Endings.Museum.ResearchPermillePerCapability);
        Assert.Equal(before + expected, g.Me.ResearchPoints);
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == "mini" && m.Status == ModelStatus.Retired));
    }

    [Fact]
    public void Zero_salvage_permille_prunes_without_research()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", 200, v);
        g.C.Endings.Museum.ResearchPermillePerCapability = 0;
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
        Assert.Equal(6, g.Me.Models.Count(m => m.Status == ModelStatus.Retired));
    }

    [Fact]
    public void Salvage_emits_world_news_with_total_granted()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 80, v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TechResearched && e.Ref == "museum-salvage" && e.Amount > 0);
    }

    [Fact]
    public void Each_line_pruned_independently()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 8; v++)
        {
            AddRetired(g, "mini", 50, v);
            AddRetired(g, "mid", 120, v + 100);
        }
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == "mini"));
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == "mid"));
    }

    [Fact]
    public void No_prune_when_at_or_below_cap_leaves_research_unchanged()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", 90, v);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Higher_capability_retirees_yield_more_salvage_per_model()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", v == 0 ? 300 : 100, v);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long lowOnly = MathI.MulPermille(100, g.C.Endings.Museum.ResearchPermillePerCapability);
        long withHigh = lowOnly + MathI.MulPermille(300, g.C.Endings.Museum.ResearchPermillePerCapability);
        Assert.Equal(before + withHigh, g.Me.ResearchPoints);
    }

    [Fact]
    public void Salvage_survives_save_load_round_trip()
    {
        var g = SoloMuseumSalvageOnly();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", 100, v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long points = g.Me.ResearchPoints;
        int count = g.Me.Models.Count;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(points, loaded.Me.ResearchPoints);
        Assert.Equal(count, loaded.Me.Models.Count);
    }

    [Fact]
    public void Museum_salvage_can_fund_frontier_after_endgame_tech_done()
    {
        var g = SoloMuseumSalvageOnly();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2045);
        foreach (TechConfig tech in g.C.Research.Techs)
            if (EraSystem.TechOpen(g.C, tech, g.Tick) && !g.Me.Techs.Contains(tech.Id)) g.Me.Techs.Add(tech.Id);
        long cost = EndingSystem.FrontierCost(g.C, g.Me);
        g.Me.ResearchPoints = cost - 1;
        for (int v = 0; v < 20; v++) AddRetired(g, "mini", 500, v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchPoints >= cost);
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(1, g.Me.FrontierLevel);
    }

    [Fact]
    public void Released_models_are_not_pruned_or_salvaged()
    {
        var g = SoloMuseumSalvageOnly();
        ModelState released = ExpansionTestSupport.AddReleasedModel(g, 150, "game-ai");
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", 60, v);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.Me.Models, m => m.Id == released.Id);
        Assert.Equal(before + 2 * MathI.MulPermille(60, g.C.Endings.Museum.ResearchPermillePerCapability), g.Me.ResearchPoints);
    }
}