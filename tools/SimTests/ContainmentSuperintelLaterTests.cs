using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ContainmentSuperintelLaterTests
{
    static ModelState Released(Game g, int capability, int alignment) =>
        new ModelState
        {
            Id = g.State.TakeId(),
            Capability = capability,
            Alignment = alignment,
            DisplayedAlignment = alignment,
            Status = ModelStatus.Released,
            LineId = "mini",
            Version = 1
        };

    static void Wing(Game g, int tiles)
    {
        int side = tiles >= 16 ? 4 : 4;
        int other = tiles / side;
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = side, Height = other });
    }

    [Fact]
    public void Containment_wing_tiles_add_alignment_work_up_to_the_cap()
    {
        var g = Game.Solo();
        int baseWork = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Wing(g, 16);
        Assert.Equal(200, LabQueries.ContainmentAlignmentBonus(g.C, g.Me));
        Assert.Equal(baseWork + 200, TrainingSystem.AlignmentWorkPermille(g.C, g.Me));
        Wing(g, 16);
        Assert.Equal(200, LabQueries.ContainmentAlignmentBonus(g.C, g.Me));
    }

    [Fact]
    public void Without_a_containment_wing_the_alignment_bonus_is_zero()
    {
        var g = Game.Solo();
        Assert.Equal(0, LabQueries.ContainmentTiles(g.C, g.Me));
        Assert.Equal(0, LabQueries.ContainmentAlignmentBonus(g.C, g.Me));
    }

    [Fact]
    public void Best_released_model_is_the_highest_capability_release()
    {
        var g = Game.Solo();
        g.Me.Models.Add(Released(g, 400, 500));
        g.Me.Models.Add(Released(g, 950, 500));
        g.Me.Models.Add(Released(g, 700, 500));
        Assert.Equal(950, LabQueries.BestReleasedModel(g.Me).Capability);
    }

    [Fact]
    public void Superintelligence_paperclipped_when_alignment_is_low()
    {
        var g = Game.Solo();
        g.Me.Models.Add(Released(g, 950, 100));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Single(g.State.Endings, e => e.EndingId == EndingSystem.PaperclippedId && e.LabId == g.Id);
    }

    [Fact]
    public void Superintelligence_long_summer_when_alignment_and_frontier_are_high()
    {
        var g = Game.Solo();
        g.Me.FrontierLevel = 2;
        g.Me.Models.Add(Released(g, 950, 900));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Single(g.State.Endings, e => e.EndingId == EndingSystem.LongSummerId);
    }

    [Fact]
    public void Superintelligence_loose_when_middling_alignment_and_no_containment()
    {
        var g = Game.Solo();
        g.Me.Models.Add(Released(g, 950, 400));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Single(g.State.Endings, e => e.EndingId == EndingSystem.LooseId);
    }

    [Fact]
    public void Containment_wing_blocks_the_loose_ending_in_the_middle_band()
    {
        var g = Game.Solo();
        Wing(g, 16);
        g.Me.Models.Add(Released(g, 950, 400));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.LooseId);
    }

    [Fact]
    public void Below_released_capability_threshold_skips_superintelligence_endings()
    {
        var g = Game.Solo();
        g.Me.Models.Add(Released(g, 500, 50));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Empty(g.State.Endings);
    }

    [Fact]
    public void Rival_releases_do_not_trigger_player_superintelligence_endings()
    {
        var g = Game.New();
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 950, 50));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Empty(g.State.Endings);
    }

    [Fact]
    public void Year_end_hook_can_reach_superintelligence_endings()
    {
        var g = Game.Solo();
        g.Me.Models.Add(Released(g, 950, 100));
        int year = GameDate.Year(g.C.Calendar, g.Tick);
        int end = GameDate.TickOfYear(g.C.Calendar, year + 1) - 1;
        g.State.Tick = end;
        g.Weeks(1);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.PaperclippedId);
    }

    [Fact]
    public void Default_config_loads_containment_and_superintelligence_numbers()
    {
        GameConfig c = TestData.Config();
        Assert.Equal("containment-wing", c.Safety.Containment.RoomId);
        Assert.Equal(25, c.Safety.Containment.AlignmentPermillePerTile);
        Assert.Equal(900, c.Endings.Superintelligence.ReleasedCapabilityMin);
        Assert.True(c.Endings.Superintelligence.LooseRequiresZeroContainment);
    }

    [Fact]
    public void Schema_15_migration_preserves_state_without_new_keys()
    {
        var g = Game.Solo(seed: 4);
        g.Me.Models.Add(Released(g, 200, 500));
        string old = SaveSchemaPretend.At(g.C, 14, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(sim);
    }
}