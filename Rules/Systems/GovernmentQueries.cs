using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class GovernmentQueries
  {
    public struct AuditFineBreakdown
    {
      public long FromLastWeekRevenue;
      public int RevenueWeeksMultiplier;
      public long Total { get { return FromLastWeekRevenue * RevenueWeeksMultiplier; } }
    }

    public static AuditFineBreakdown AuditFineParts(GameConfig c, LabState lab)
    {
      var b = new AuditFineBreakdown();
      b.FromLastWeekRevenue = lab.LastWeek.Revenue;
      b.RevenueWeeksMultiplier = c.World.Attention.AuditFineRevenueWeeks;
      return b;
    }

    public static long AuditFineAmount(GameConfig c, LabState lab)
    {
      return AuditFineParts(c, lab).Total;
    }

    /// <summary>Amount charged at audit level (era compliance may adjust on merge).</summary>
    public static long ApplyAuditFine(GameConfig c, LabState lab)
    {
      return AuditFineAmount(c, lab);
    }
  }
}