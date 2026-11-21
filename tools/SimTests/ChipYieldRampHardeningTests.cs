using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipYieldRampHardeningTests
{
    [Fact]
    public void Ready_design_yield_rises_each_week()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        int before = d.YieldPermille;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(d.YieldPermille > before);
    }

    [Fact]
    public void Weekly_ramp_matches_base_plus_engineers()
    {
        Game g = ChipHardeningSupport.Lab(2013, tweak: c => { c.Chips.YieldRampPerWeek = 10; c.Chips.YieldRampPerEngineer = 2; });
        ChipHardeningSupport.HireEngineers(g, 3);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        int before = d.YieldPermille;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        long expectedRamp = g.C.Chips.YieldRampPerWeek + 3L * g.C.Chips.YieldRampPerEngineer;
        Assert.Equal(before + expectedRamp, d.YieldPermille);
    }

    [Fact]
    public void Engineer_bonus_is_capped()
    {
        Game g = ChipHardeningSupport.Lab(2013, tweak: c => { c.Chips.YieldRampPerEngineer = 5; c.Chips.YieldRampEngineerCap = 2; });
        ChipHardeningSupport.HireEngineers(g, 4);
        Assert.Equal(4, ChipQueries.Engineers(g.C, g.Me));
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        int before = d.YieldPermille;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        long engineers = MathI.Min(ChipQueries.Engineers(g.C, g.Me), g.C.Chips.YieldRampEngineerCap);
        long expectedRamp = g.C.Chips.YieldRampPerWeek + engineers * g.C.Chips.YieldRampPerEngineer;
        Assert.Equal(before + expectedRamp, d.YieldPermille);
    }

    [Fact]
    public void Hw_test_lab_doubles_the_ramp()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, g.C.Chips.TestLabRoomId, x, y, 4, 3));
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        int before = d.YieldPermille;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        long baseRamp = g.C.Chips.YieldRampPerWeek;
        long withLab = MathI.MulPermille(baseRamp, g.C.Chips.TestLabRampPermille);
        Assert.Equal(before + withLab, d.YieldPermille);
    }

    [Fact]
    public void Yield_stops_at_max_from_spec()
    {
        Game g = ChipHardeningSupport.Lab(2013, tweak: c => c.Chips.YieldRampPerWeek = 500);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 900);
        int max = ChipQueries.Spec(g.C, d).MaxYieldPermille;
        for (int i = 0; i < 5; i++) ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(max, d.YieldPermille);
    }

    [Fact]
    public void Non_ready_designs_ignore_ramp()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        var d = new ChipDesign { Id = g.State.TakeId(), Status = ChipDesignStatus.Designing, YieldPermille = 0 };
        g.Me.ChipDesigns.Add(d);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, d.YieldPermille);
    }

    [Fact]
    public void Bankrupt_lab_still_ramps_when_invoked_directly()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 600);
        g.Me.Bankrupt = true;
        int before = d.YieldPermille;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(d.YieldPermille > before);
    }

    [Fact]
    public void Ramp_runs_for_every_ready_design()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign a = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        ChipDesign b = ChipHardeningSupport.InjectReady(g, g.Me, yield: 520);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(a.YieldPermille > 500);
        Assert.True(b.YieldPermille > 520);
    }

    [Fact]
    public void Yield_values_survive_save_load()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 500);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        int yield = d.YieldPermille;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(yield, loaded.Me.Design(d.Id).YieldPermille);
    }
}