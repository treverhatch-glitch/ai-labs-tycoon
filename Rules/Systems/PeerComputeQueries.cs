using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class PeerComputeQueries
  {
    public struct WeeklyRentBreakdown
    {
      public long FromSpot;
      public long FromLeasePermille;
      public long FromShortage;
      public long Total { get { return FromSpot + FromLeasePermille + FromShortage; } }
    }

    public static WeeklyRentBreakdown WeeklyRentParts(GameConfig c, GameState state, int gpuCount)
    {
      var b = new WeeklyRentBreakdown();
      if (gpuCount < 1) return b;
      GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
      long spot = gen.RentPerGpuWeek * gpuCount;
      b.FromSpot = spot;
      long afterLease = MathI.MulPermille(spot, c.Compute.PeerLeaseRentPermille);
      b.FromLeasePermille = afterLease - spot;
      long total = PeerComputeSystem.WeeklyRent(c, state, gpuCount);
      b.FromShortage = total - afterLease;
      return b;
    }
  }
}