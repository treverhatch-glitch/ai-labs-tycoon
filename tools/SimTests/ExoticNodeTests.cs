using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q64: exotic foundry nodes (n3) need chip-codesign beyond calendar open year.</summary>
public class ExoticNodeTests
{
    [Fact]
    public void Shipped_n3_requires_chip_codesign()
    {
        GameConfig c = TestData.Config();
        ChipNodeConfig n3 = c.Chips.Node("n3");
        Assert.Equal("chip-codesign", n3.RequiresTech);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void N3_open_by_year_but_locked_without_tech()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipNodeConfig n3 = g.C.Chips.Node("n3");
        Assert.True(ChipQueries.NodeOpen(g.C, n3, g.State.Tick));
        Assert.False(ChipQueries.NodeUnlocked(g.C, g.Me, n3, g.State.Tick));
        g.Refused(Command.StartChipDesign(g.Id, ChipHardeningSupport.Inference, "n3", "medium", "fp16", "ddr", "air"),
            CommandError.MissingPrerequisite);
    }

    [Fact]
    public void N3_unlocks_with_chip_codesign()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("chip-codesign");
        ChipNodeConfig n3 = g.C.Chips.Node("n3");
        Assert.True(ChipQueries.NodeUnlocked(g.C, g.Me, n3, g.State.Tick));
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, node: "n3");
        Assert.Equal("n3", d.NodeId);
    }

    [Fact]
    public void Before_open_year_calendar_locked()
    {
        Game g = ChipHardeningSupport.Lab(2024);
        g.Me.Techs.Add("chip-codesign");
        ChipNodeConfig n3 = g.C.Chips.Node("n3");
        Assert.False(ChipQueries.NodeOpen(g.C, n3, g.State.Tick));
        g.Refused(Command.StartChipDesign(g.Id, ChipHardeningSupport.Inference, "n3", "medium", "fp16", "ddr", "air"),
            CommandError.EraLocked);
    }

    [Fact]
    public void N5_never_needs_exotic_tech()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipNodeConfig n5 = g.C.Chips.Node("n5");
        Assert.True(string.IsNullOrEmpty(n5.RequiresTech));
        Assert.True(ChipQueries.NodeUnlocked(g.C, g.Me, n5, g.State.Tick));
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, node: "n5");
        Assert.Equal("n5", d.NodeId);
    }

    [Fact]
    public void Inject_ready_n3_design_orders_when_unlocked()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("chip-codesign");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, node: "n3");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Bankrupt_lab_query_still_false_for_missing_tech()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Bankrupt = true;
        ChipNodeConfig n3 = g.C.Chips.Node("n3");
        Assert.False(ChipQueries.NodeUnlocked(g.C, g.Me, n3, g.State.Tick));
    }

    [Fact]
    public void Save_load_preserves_n3_design_node()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("chip-codesign");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, node: "n3");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ChipDesign loadedDesign = loaded.Me.Design(d.Id);
        Assert.Equal("n3", loadedDesign.NodeId);
        Assert.True(ChipQueries.NodeUnlocked(loaded.C, loaded.Me, loaded.C.Chips.Node("n3"), loaded.State.Tick));
    }

    [Fact]
    public void Chip_shortage_still_applies_to_n3_orders()
    {
        Game g = ChipHardeningSupport.Lab(2028, tweak: c =>
        {
            c.Chips.Shortage.MinStrengthPermille = 1200;
            c.Chips.Shortage.MaxStrengthPermille = 1200;
        });
        g.Me.Techs.Add("chip-codesign");
        g.State.ChipShortageUntilTick = g.State.Tick + 20;
        g.State.ChipShortageStrengthPermille = 1200;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, node: "n3");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        long costBefore = g.Me.Cash;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 3));
        Assert.True(g.Me.Cash < costBefore);
    }

    [Fact]
    public void Buyer_without_codesign_can_still_buy_listed_n3_chips()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: true);
        LabState seller = g.State.Labs[1];
        seller.Techs.Add("chip-codesign");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, seller, node: "n3", yield: 800);
        seller.Cash = 50_000_000;
        seller.SoftwarePermille = 800;
        g.Do(Command.SetChipListing(seller.Id, d.Id, 4000, 20));
        g.Me.Techs.Remove("chip-codesign");
        g.Me.Cash = 500_000;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 2));
        Assert.Equal(2, g.Me.ChipOrders[0].Count);
        Assert.Equal("n3", d.NodeId);
    }

    [Fact]
    public void Zero_exotic_nodes_before_2025()
    {
        Game g = ChipHardeningSupport.Lab(2024);
        foreach (ChipNodeConfig n in g.C.Chips.Nodes)
            if (!string.IsNullOrEmpty(n.RequiresTech))
                Assert.False(ChipQueries.NodeOpen(g.C, n, g.State.Tick));
    }

    [Fact]
    public void Max_tier_n3_is_tier_six()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(6, c.Chips.Node("n3").Tier);
    }

    [Fact]
    public void Chiplet_order_on_n5_still_works_without_codesign()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm", node: "n5");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 4));
        Assert.Equal("n5", d.NodeId);
    }
}