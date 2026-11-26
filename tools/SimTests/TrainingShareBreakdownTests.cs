using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q75: LabQueries.TrainingShareParts decomposes training compute share.</summary>
public class TrainingShareBreakdownTests
{
    [Fact]
    public void Shipped_in_house_and_chiplet_bonus_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chips.InHouseTrainingBonusPermille > 0);
        Assert.True(c.Chips.ChipletTrainingBonusPermille > 0);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Main_path_parts_sum_to_training_share_of()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.TrainingSharePermille = 700;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5", "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 4);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        LabQueries.TrainingShareBreakdown p = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split);
        Assert.Equal(p.Total, LabQueries.TrainingShareOf(g.C, g.State, g.Me, split));
    }

    [Fact]
    public void Zero_own_chips_no_chiplet_bonus_line()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        g.Me.TrainingSharePermille = 500;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        LabQueries.TrainingShareBreakdown p = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split);
        Assert.Equal(0, p.OwnShareBeforeBonus);
        Assert.Equal(0, p.ChipletBonusPermille);
    }

    [Fact]
    public void Max_training_share_permille_on_bought_line()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 10);
        g.Me.TrainingSharePermille = 1000;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        LabQueries.TrainingShareBreakdown p = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split);
        Assert.Equal(split.General, p.BoughtShare);
    }

    [Fact]
    public void Bankrupt_lab_parts_still_query()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.Bankrupt = true;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.True(LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).Total >= 0);
    }

    [Fact]
    public void Save_load_preserves_breakdown_total()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.TrainingSharePermille = 800;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5");
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Hq, g.Id, d.Id, 3);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long total = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).Total;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ComputeSplit split2 = LabQueries.Split(loaded.C, loaded.State, loaded.Me, false);
        Assert.Equal(total, LabQueries.TrainingShareParts(loaded.C, loaded.State, loaded.Me, split2).Total);
    }

    [Fact]
    public void Chiplet_bonus_only_on_chiplet_training_die()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.TrainingSharePermille = 1000;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        ChipDesign plain = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5", "medium");
        ChipDesign chiplet = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5", "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, plain.Id, 2);
        ComputeSplit splitPlain = LabQueries.Split(g.C, g.State, g.Me, false);
        int bonusPlain = LabQueries.TrainingShareParts(g.C, g.State, g.Me, splitPlain).ChipletBonusPermille;
        Fleet.Take(g.Me, g.Me.Fleet[0], 2);
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, chiplet.Id, 2);
        ComputeSplit splitChip = LabQueries.Split(g.C, g.State, g.Me, false);
        int bonusChip = LabQueries.TrainingShareParts(g.C, g.State, g.Me, splitChip).ChipletBonusPermille;
        Assert.Equal(0, bonusPlain);
        Assert.Equal(g.C.Chips.ChipletTrainingBonusPermille, bonusChip);
    }

    [Fact]
    public void Zero_chiplet_bonus_config()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false, tweak: c => c.Chips.ChipletTrainingBonusPermille = 0);
        g.Me.TrainingSharePermille = 1000;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5", "chiplet-mcm");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 2);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(0, LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).ChipletBonusPermille);
    }

    [Fact]
    public void Inference_chips_not_in_own_share()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Inference, "n28");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 3);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        LabQueries.TrainingShareBreakdown p = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split);
        Assert.Equal(0, p.OwnShareBeforeBonus);
        Assert.Equal(0, p.ChipletBonusPermille);
        Assert.False(ChipQueries.RunsChipletTrainingChips(g.C, g.State, g.Me));
    }

    [Fact]
    public void Training_system_uses_same_total()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 12);
        g.Me.TrainingSharePermille = 650;
        ModelState m = g.Train("mini", 50);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long parts = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).Total;
        Assert.Equal(parts, LabQueries.TrainingShareOf(g.C, g.State, g.Me, split));
        Assert.True(parts >= 0);
    }

    [Fact]
    public void Peer_compute_in_general_line()
    {
        Game g = Game.New();
        g.Me.Cash = 400_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        for (int i = 0; i < 8; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        g.Me.TrainingSharePermille = 500;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.True(LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).BoughtShare > 0);
    }

    [Fact]
    public void In_house_bonus_permille_on_own_line_only()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.TrainingSharePermille = 1000;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.SoftwarePermille = 1000;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, ChipHardeningSupport.Training, "n5");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 2);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        LabQueries.TrainingShareBreakdown p = LabQueries.TrainingShareParts(g.C, g.State, g.Me, split);
        Assert.Equal(g.C.Chips.InHouseTrainingBonusPermille, p.InHouseBonusPermille);
        Assert.True(p.OwnShareBeforeBonus > 0);
        Assert.True(p.OwnShare > p.OwnShareBeforeBonus);
    }

    [Fact]
    public void Reserved_contract_included_in_bought_share()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Do(Command.SignReservedCompute(g.Id, 2));
        g.Me.TrainingSharePermille = 1000;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.True(LabQueries.TrainingShareParts(g.C, g.State, g.Me, split).BoughtShare > 0);
    }
}