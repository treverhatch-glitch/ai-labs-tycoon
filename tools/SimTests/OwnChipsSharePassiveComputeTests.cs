using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class OwnChipsSharePassiveComputeTests
{
  const string Training = "training";

  static void LoadOwnTrainingChips(Game g, LabState lab, int count)
  {
    int hall = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
    lab.Holding(hall).HallKw = 1_000_000;
    ChipSpec spec = ChipQueries.Derive(g.C, Training, "n5", "medium", "fp8", "hbm", "air");
    var d = new ChipDesign();
    d.Id = g.State.TakeId();
    d.Version = ChipQueries.NextVersion(lab);
    d.Target = Training;
    d.NodeId = "n5";
    d.DieId = "medium";
    d.FormatId = "fp8";
    d.MemoryId = "hbm";
    d.CoolingId = "air";
    d.PointsRequired = spec.Points;
    d.PointsDone = spec.Points;
    d.Status = ChipDesignStatus.Ready;
    d.YieldPermille = 900;
    d.ReadyTick = g.State.Tick;
    lab.ChipDesigns.Add(d);
    lab.SoftwarePermille = 1000;
    ChipSystem.AddChips(g.State, lab, hall, lab.Id, d.Id, count);
  }

  [Fact]
  public void Passive_uplink_does_not_dilute_own_chip_share()
  {
    Game g = ChipHardeningSupport.Lab(2034, rivals: false);
    LoadOwnTrainingChips(g, g.Me, 400);
    int without = ChipQueries.OwnChipsShare(g.C, g.State, g.Me);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    int with = ChipQueries.OwnChipsShare(g.C, g.State, g.Me);
    Assert.Equal(without, with);
    Assert.True(LabQueries.Split(g.C, g.State, g.Me, false).General > without);
  }

  [Fact]
  public void Passive_robotics_arena_also_excluded_from_share_denominator()
  {
    Game g = ExpansionTestSupport.SoloAtYear(2029);
    LoadOwnTrainingChips(g, g.Me, 300);
    int before = ChipQueries.OwnChipsShare(g.C, g.State, g.Me);
    ExpansionTestSupport.BuildExpansionRoom(g, "robotics-arena");
    Assert.Equal(before, ChipQueries.OwnChipsShare(g.C, g.State, g.Me));
  }

  [Fact]
  public void Courting_triggers_with_uplink_when_share_would_miss_without_adjustment()
  {
    Game g = ChipHardeningSupport.Lab(2034);
    LoadOwnTrainingChips(g, g.Me, 350);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    g.Rent(200);
    int share = ChipQueries.OwnChipsShare(g.C, g.State, g.Me);
    Assert.True(share >= g.C.Vendor.CourtingSharePermille);
    ChipSystem.CourtRivals(g.C, g.State);
    Assert.Contains(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted && e.Ref == g.Id.ToString());
  }

  [Fact]
  public void Zero_own_chips_share_stays_zero_with_passive()
  {
    Game g = ExpansionTestSupport.SoloAtYear(2034);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    Assert.Equal(0, ChipQueries.OwnChipsShare(g.C, g.State, g.Me));
  }

  [Fact]
  public void Vendor_standing_boost_unchanged_by_passive_dilution_fix()
  {
    Game g = ChipHardeningSupport.Lab(2017);
    LoadOwnTrainingChips(g, g.Me, 1200);
    LabState rival = g.State.Labs[1];
    int before = g.State.Vendor.Standing(rival.Id).Standing;
    ChipSystem.CourtRivals(g.C, g.State);
    Assert.Equal(before + g.C.Vendor.CourtingStandingBoost, g.State.Vendor.Standing(rival.Id).Standing);
  }

  [Fact]
  public void Split_general_still_includes_passive_for_training()
  {
    Game g = ExpansionTestSupport.SoloAtYear(2034);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    long passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        Assert.True(LabQueries.Split(g.C, g.State, g.Me, false).General >= passive);
  }

  [Fact]
  public void Share_math_uses_general_minus_passive()
  {
    Game g = ChipHardeningSupport.Lab(2034, rivals: false);
    LoadOwnTrainingChips(g, g.Me, 100);
    ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
    long passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    split = LabQueries.Split(g.C, g.State, g.Me, false);
    passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
    long denom = split.General - passive;
    int expected = denom <= 0 ? 0 : (int)(split.OwnChips * 1000 / denom);
    Assert.Equal(expected, ChipQueries.OwnChipsShare(g.C, g.State, g.Me));
  }

  [Fact]
  public void Share_survives_save_load_with_uplink()
  {
    Game g = ChipHardeningSupport.Lab(2034, rivals: false);
    LoadOwnTrainingChips(g, g.Me, 500);
    ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
    int share = ChipQueries.OwnChipsShare(g.C, g.State, g.Me);
    var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
    Assert.Equal(share, ChipQueries.OwnChipsShare(loaded.C, loaded.State, loaded.Me));
  }

  [Fact]
  public void No_room_means_share_matches_raw_split_ratio()
  {
    Game g = ChipHardeningSupport.Lab(2017, rivals: false);
    LoadOwnTrainingChips(g, g.Me, 600);
    ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
    int manual = (int)(split.OwnChips * 1000 / split.General);
    Assert.Equal(manual, ChipQueries.OwnChipsShare(g.C, g.State, g.Me));
  }
}