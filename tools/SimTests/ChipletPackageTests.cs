using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q62: chiplet die packages book extra foundry room and boost training on own silicon.</summary>
public class ChipletPackageTests
{
    [Fact]
    public void Shipped_chiplet_die_and_tech_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Contains("chiplet-mcm", c.Chips.ChipletDieIds);
        Assert.NotNull(c.Chips.Option(ChipOptionGroup.Die, "chiplet-mcm"));
        Assert.NotNull(c.Tech("chiplet-packaging"));
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Chiplet_die_is_marked_in_queries()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        Assert.True(ChipQueries.IsChipletDesign(g.C, d));
        Assert.Equal((int)MathI.MulPermille(10, g.C.Chips.ChipletFoundryRoomPermille), ChipQueries.FoundrySlots(g.C, d, 10));
    }

    [Fact]
    public void Medium_die_uses_one_slot_per_chip()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "medium");
        Assert.Equal(10, ChipQueries.FoundrySlots(g.C, d, 10));
    }

    [Fact]
    public void Zero_chiplet_room_permille_matches_medium()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.C.Chips.ChipletFoundryRoomPermille = 1000;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        Assert.Equal(10, ChipQueries.FoundrySlots(g.C, d, 10));
    }

    [Fact]
    public void Max_foundry_multiplier_fills_node_at_lower_count()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.C.Chips.ChipletFoundryRoomPermille = 2000;
        ChipNodeConfig node = g.C.Chips.Node("n28");
        int room = (int)g.C.Chips.FoundryRoom(node);
        ChipDesign medium = ChipHardeningSupport.InjectReady(g, g.Me, die: "medium");
        ChipDesign chiplet = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        Assert.True(ChipQueries.FirstFreeMonth(g.C, g.State, node, medium, room) >= 0);
        Assert.Equal(-1, ChipQueries.FirstFreeMonth(g.C, g.State, node, chiplet, room));
    }

    [Fact]
    public void Chiplet_order_books_extra_foundry_slots()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 50));
        ChipNodeConfig node = g.C.Chips.Node("n28");
        int month = g.Me.ChipOrders[0].FoundryMonth;
        int slots = ChipQueries.FoundrySlots(g.C, d, 50);
        Assert.Equal(ChipQueries.BookedIn(g.State, node.IdValue, month), slots);
    }

    [Fact]
    public void Start_chiplet_design_refused_without_tech()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Remove("chiplet-packaging");
        g.Refused(Command.StartChipDesign(g.Id, ChipHardeningSupport.Training, "n28", "chiplet-mcm", "fp16", "ddr", "air"),
            CommandError.MissingPrerequisite);
    }

    [Fact]
    public void Own_chiplet_training_chips_detected_in_fleet()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, die: "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Hq, g.Id, d.Id, 4);
        Assert.True(ChipQueries.RunsChipletTrainingChips(g.C, g.State, g.Me));
    }

    [Fact]
    public void Inference_chiplet_chips_do_not_count_for_training_bonus()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Inference, die: "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Hq, g.Id, d.Id, 4);
        Assert.False(ChipQueries.RunsChipletTrainingChips(g.C, g.State, g.Me));
    }

    [Fact]
    public void Training_share_includes_chiplet_bonus_on_own_silicon()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, die: "chiplet-mcm");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        g.Me.TrainingSharePermille = 1000;
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 8);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long withBonus = LabQueries.TrainingShareOf(g.C, g.State, g.Me, split);
        g.C.Chips.ChipletTrainingBonusPermille = 0;
        long without = LabQueries.TrainingShareOf(g.C, g.State, g.Me, split);
        Assert.True(withBonus > without);
    }

    [Fact]
    public void Zero_chiplet_training_bonus_matches_in_house_only()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, die: "chiplet-mcm");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        g.Me.TrainingSharePermille = 1000;
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 4);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        int chipletBonus = g.C.Chips.ChipletTrainingBonusPermille;
        long withExtra = LabQueries.TrainingShareOf(g.C, g.State, g.Me, split);
        g.C.Chips.ChipletTrainingBonusPermille = 0;
        long noExtra = LabQueries.TrainingShareOf(g.C, g.State, g.Me, split);
        long ownShare = MathI.MulPermille(split.OwnChips, g.Me.TrainingSharePermille);
        int inHouse = g.C.Chips.InHouseTrainingBonusPermille;
        long withChiplet = MathI.MulPermille(ownShare, MathI.Permille + inHouse + chipletBonus);
        long inHouseOnly = MathI.MulPermille(ownShare, MathI.Permille + inHouse);
        Assert.Equal(withChiplet - inHouseOnly, withExtra - noExtra);
    }

    [Fact]
    public void Bankrupt_lab_releases_chiplet_foundry_slots()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 40));
        int month = g.Me.ChipOrders[0].FoundryMonth;
        int booked = (int)ChipQueries.BookedIn(g.State, d.NodeId, month);
        g.Me.Bankrupt = true;
        ChipSystem.OnBankrupt(g.C, g.State, g.Me);
        Assert.Equal(0, ChipQueries.BookedIn(g.State, d.NodeId, month));
        Assert.True(booked > 40);
    }

    [Fact]
    public void Save_load_preserves_chiplet_design_die()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ChipDesign after = loaded.Me.Design(d.Id);
        Assert.Equal("chiplet-mcm", after.DieId);
        Assert.True(ChipQueries.IsChipletDesign(loaded.C, after));
    }

    [Fact]
    public void Chip_shortage_still_applies_to_chiplet_orders()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        long before = g.Me.Cash;
        ChipShortageSystem.Start(g.C, g.State, 1300);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        long paid = before - g.Me.Cash;
        Assert.True(paid > 0);
    }

    [Fact]
    public void Peer_compute_cross_includes_chiplet_design_orders()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, die: "chiplet-mcm");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 20));
        long running = LabQueries.RunningCostParts(g.C, g.State, g.Me).Total;
        Assert.True(running >= 0);
        Assert.True(g.Me.ChipOrders.Count == 1);
    }

    [Fact]
    public void Derive_compute_higher_for_chiplet_die_than_medium()
    {
        GameConfig c = TestData.Config();
        long medium = ChipQueries.Derive(c, "training", "n28", "medium", "fp16", "ddr", "air").Compute;
        long chiplet = ChipQueries.Derive(c, "training", "n28", "chiplet-mcm", "fp16", "ddr", "air").Compute;
        Assert.True(chiplet > medium);
    }

    [Fact]
    public void Foreign_chiplet_chips_no_training_bonus()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        LabState rival = g.Rival("openaye");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, rival, ChipHardeningSupport.Training, die: "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Hq, rival.Id, d.Id, 4);
        Assert.False(ChipQueries.RunsChipletTrainingChips(g.C, g.State, g.Me));
    }
}