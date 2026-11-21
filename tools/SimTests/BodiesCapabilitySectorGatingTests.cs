using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BodiesCapabilitySectorGatingTests
{
    const string Arena = "robotics-arena";

    static Game BodiesLab(int year = 2029)
    {
        var g = ExpansionTestSupport.SoloAtYear(year);
        g.WithStarterLab(gpus: 32, engineers: 4);
        return g;
    }

    [Fact]
    public void Mini_line_still_gets_bodies_capability_with_arena()
    {
        Game g = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.True(model.CapExpansion > 0);
    }

    [Fact]
    public void Code_specialist_gets_no_bodies_capability_without_robotics_sectors()
    {
        Game g = BodiesLab();
        g.Me.Techs.Add("code-models");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("code-specialist", 400);
        g.UntilTrained(model);
        Assert.Equal(0, model.CapExpansion);
        Assert.Equal(0, ExpansionSystem.CapabilityBonusPermille(g.C, g.State, g.Me, "code-specialist"));
    }

    [Fact]
    public void Code_specialist_with_robotics_sector_overlap_gets_capability()
    {
        Game g = BodiesLab();
        g.Me.Techs.Add("code-models");
        g.C.Line("code-specialist").SpecialistSectorIds.Add("robotics-industry");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("code-specialist", 400);
        g.UntilTrained(model);
        Assert.True(model.CapExpansion > 0);
    }

    [Fact]
    public void Image_specialist_without_matching_sectors_has_no_expansion_bonus()
    {
        Game g = BodiesLab();
        g.Me.Techs.Add("conv-nets");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("image-specialist", 400);
        g.UntilTrained(model);
        Assert.Equal(0, model.CapExpansion);
    }

    [Fact]
    public void Without_arena_even_mini_loses_capability_bonus()
    {
        Game g = BodiesLab();
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.Equal(0, model.CapExpansion);
    }

    [Fact]
    public void Breakdown_shows_zero_from_expansion_for_gated_specialist()
    {
        Game g = BodiesLab();
        g.Me.Techs.Add("code-models");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("code-specialist", 400);
        g.UntilTrained(model);
        Assert.Equal(0, DisplayQueries.TrainedCapabilityParts(model).FromExpansion);
    }

    [Fact]
    public void Before_bodies_era_gating_and_bonus_stay_off()
    {
        Game g = BodiesLab(2028);
        g.Me.Rooms.Add(new RoomState { TypeId = Arena, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.Equal(0, model.CapExpansion);
    }

    [Fact]
    public void Upward_kit_capability_still_never_applies()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2034);
        g.WithStarterLab(gpus: 48);
        ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
        Assert.Equal(0, ExpansionSystem.CapabilityBonusPermille(g.C, g.State, g.Me, "mini"));
    }

    [Fact]
    public void Gating_survives_save_load_on_finished_model()
    {
        Game g = BodiesLab();
        g.Me.Techs.Add("code-models");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("code-specialist", 400);
        g.UntilTrained(model);
        int cap = model.Capability;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState after = loaded.Me.Models.Find(m => m.Id == model.Id);
        Assert.Equal(cap, after.Capability);
        Assert.Equal(0, after.CapExpansion);
    }
}