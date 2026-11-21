using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class VendorKeynoteAmbushHardeningTests
{
    [Fact]
    public void Keynote_ambushes_a_fresh_ready_design_inside_lookback()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int keynoteTick = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = keynoteTick;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, readyTick: keynoteTick - g.C.Vendor.AmbushLookbackWeeks);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.Equal(keynoteTick + g.C.Vendor.AmbushWeeks, g.Me.ChipAmbushUntilTick);
    }

    [Fact]
    public void Design_older_than_lookback_is_not_ambushed()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int keynoteTick = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = keynoteTick;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: keynoteTick - g.C.Vendor.AmbushLookbackWeeks - 1);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.True(g.Me.ChipAmbushUntilTick < keynoteTick);
    }

    [Fact]
    public void Month_start_keynote_fires_event_and_sets_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017, tweak: c => { c.Vendor.GpusPerStanding = 1_000_000; });
        int keynoteTick = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = keynoteTick;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: keynoteTick);
        VendorSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Keynote);
        Assert.Equal(keynoteTick + g.C.Vendor.AmbushWeeks, g.Me.ChipAmbushUntilTick);
    }

    [Fact]
    public void Every_ready_design_in_a_lab_gets_ambush_on_keynote()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t - 1);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.Equal(t + g.C.Vendor.AmbushWeeks, g.Me.ChipAmbushUntilTick);
    }

    [Fact]
    public void Bankrupt_labs_skip_keynote_ambush_scan()
    {
        Game g = ChipHardeningSupport.Lab(2017, rivals: true);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        LabState rival = g.State.Labs[1];
        rival.Bankrupt = true;
        ChipHardeningSupport.InjectReady(g, rival, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.True(rival.ChipAmbushUntilTick < t);
    }

    [Fact]
    public void A_design_still_taping_out_never_triggers_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        var d = new ChipDesign
        {
            Id = g.State.TakeId(), Status = ChipDesignStatus.TapingOut, ReadyTick = -1,
            StageEndsTick = t + 4, NodeId = "n28", Target = ChipHardeningSupport.Inference,
        };
        g.Me.ChipDesigns.Add(d);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.True(g.Me.ChipAmbushUntilTick < t);
    }

    [Fact]
    public void Ambush_window_survives_save_load()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        int until = g.Me.ChipAmbushUntilTick;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(until, loaded.Me.ChipAmbushUntilTick);
    }

    [Fact]
    public void Planner_skips_listed_chip_buys_while_ambushed()
    {
        Game g = ChipHardeningSupport.Lab(2013, seed: 11, tweak: c =>
        {
            PlanConfig p = c.Rival("deepsneak").Plan;
            p.BuysListedChips = true;
            p.Gpus = 400;
            p.ChipProgramYear = 2013;
        });
        g.State.PendingRivals.Remove("deepsneak");
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("deepsneak"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 5_000_000_000L);
        buyer.PlanStartTick = 0;
        ChipDesign listed = ChipHardeningSupport.InjectReady(g, g.Me, yield: 900);
        g.Me.SoftwarePermille = 1000;
        g.Do(Command.SetChipListing(g.Id, listed.Id, 1000, 50));
        buyer.ChipAmbushUntilTick = g.State.Tick + 12;
        LabPlanner.Run(g.C, g.State, buyer, g.C.Rival("deepsneak").Plan, cmd => g.Try(cmd));
        Assert.DoesNotContain(buyer.Log, e => e.Action == DecisionAction.BoughtChips);
    }

    [Fact]
    public void Shorter_ambush_config_shortens_the_window()
    {
        Game g = ChipHardeningSupport.Lab(2017, tweak: c => c.Vendor.AmbushWeeks = 4);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        Assert.Equal(t + 4, g.Me.ChipAmbushUntilTick);
    }
}