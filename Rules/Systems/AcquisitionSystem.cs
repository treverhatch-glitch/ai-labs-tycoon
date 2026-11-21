using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class AcquisitionSystem
  {
    public struct AcquisitionQuote
    {
      public long FleetPart;
      public long ModelsPart;
      public long StaffPart;
      public long CashPart;
      public long IntegrationCost;
      public long Total;
      public bool Distressed;
    }

    public static bool IsDistressed(GameConfig c, GameState state, LabState target)
    {
      if (target.Bankrupt || target.Cash < 0 || target.WeeksInDebt > 0) return true;
      return BubbleSystem.InCrash(state);
    }

    public static AcquisitionQuote Quote(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      AcquisitionConfig cfg = c.Acquisitions;
      var q = new AcquisitionQuote();
      if (cfg == null || buyer == null || target == null) return q;
      q.Distressed = IsDistressed(c, state, target);
      q.CashPart = target.Cash > 0 ? target.Cash : 0;
      for (int i = 0; i < target.Models.Count; i++)
      {
        ModelState m = target.Models[i];
        if (m.Status == ModelStatus.Retired || m.Status == ModelStatus.Training) continue;
        q.ModelsPart += (long)m.Capability * cfg.CapabilityValuePerPoint;
      }
      q.StaffPart = (long)target.Staff.Count * cfg.StaffValuePerHead;
      q.FleetPart = MathI.MulPermille(LabQueries.FleetResaleValue(c, state, target), cfg.FleetResalePermille);
      long subtotal = q.CashPart + q.ModelsPart + q.StaffPart + q.FleetPart;
      if (q.Distressed)
      {
        int permille = BubbleSystem.InCrash(state) ? cfg.CrashDistressPricePermille : cfg.DistressPricePermille;
        subtotal = MathI.MulPermille(subtotal, permille);
      }
      q.IntegrationCost = MathI.MulPermille(subtotal, cfg.IntegrationCostPermille);
      q.Total = subtotal + q.IntegrationCost;
      return q;
    }

    public static bool CanAcquire(GameConfig c, GameState state, LabState buyer, LabState target, out string reason)
    {
      reason = "";
      AcquisitionConfig cfg = c.Acquisitions;
      if (cfg == null) { reason = "Acquisitions are not configured."; return false; }
      if (buyer == null || target == null || buyer.Id == target.Id) { reason = "Invalid labs."; return false; }
      if (buyer.Bankrupt) { reason = buyer.Name + " is bankrupt."; return false; }
      if (target.Id == state.PlayerLabId && target.Controller == LabController.Player) { reason = "You cannot buy your own lab."; return false; }
      if (target.AbsorbedByLabId >= 0) { reason = target.Name + " is already absorbed."; return false; }
      if (target.Controller != LabController.Plan) { reason = "Only rival labs can be acquired."; return false; }
      int released = 0;
      for (int i = 0; i < target.Models.Count; i++)
        if (target.Models[i].Status == ModelStatus.Released) released++;
      if (!IsDistressed(c, state, target) && released < cfg.MinInstalledModels) { reason = "The target has too little to buy."; return false; }
      AcquisitionQuote q = Quote(c, state, buyer, target);
      if (buyer.Cash < q.Total) { reason = "Not enough cash for the deal."; return false; }
      return true;
    }

    public static void Apply(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      AcquisitionQuote q = Quote(c, state, buyer, target);
      Finance.Spend(buyer, LedgerLine.Hardware, q.Total);
      TransferModels(c, state, buyer, target);
      TransferStaff(c, buyer, target);
      TransferFleet(c, state, buyer, target);
      for (int s = 0; s < state.Sectors.Count; s++)
      {
        SectorState sector = state.Sectors[s];
        for (int i = sector.Entries.Count - 1; i >= 0; i--)
          if (sector.Entries[i].LabId == target.Id) sector.Entries.RemoveAt(i);
        if (sector.ChampionLabId == target.Id) sector.ChampionLabId = buyer.Id;
      }
      target.Models.Clear();
      target.Staff.Clear();
      target.Fleet.Clear();
      target.RentedGpus = 0;
      target.AbsorbedByLabId = buyer.Id;
      if (q.Total >= c.Acquisitions.RegulatorDealRevenue)
        RegulatorSystem.OnAcquisition(c, state, buyer, c.Acquisitions.RegulatorAttention);
      Events.Add(state, EventKind.LabAcquired, buyer.Id, target.Id, q.Total, target.RivalId);
    }

    static void TransferModels(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      for (int i = 0; i < target.Models.Count; i++)
      {
        ModelState m = target.Models[i];
        if (m.Status == ModelStatus.Retired) continue;
        m.Id = state.TakeId();
        buyer.Models.Add(m);
        for (int s = 0; s < m.Sectors.Count; s++)
        {
          SectorState sector = state.Sector(m.Sectors[s]);
          if (sector == null) continue;
          SectorEntry entry = sector.Entry(buyer.Id);
          if (entry == null)
          {
            entry = new SectorEntry { LabId = buyer.Id, ModelId = m.Id };
            sector.Entries.Add(entry);
          }
          else entry.ModelId = m.Id;
        }
      }
    }

    static void TransferStaff(GameConfig c, LabState buyer, LabState target)
    {
      int hit = c.StaffPeople.MoraleBaseline > 0 ? c.StaffPeople.MoraleBaseline / 4 : 15;
      for (int i = 0; i < target.Staff.Count; i++)
      {
        StaffMember s = target.Staff[i];
        s.Morale = (int)MathI.Max(0, s.Morale - hit);
        buyer.Staff.Add(s);
      }
    }

    static void TransferFleet(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      for (int i = 0; i < target.Fleet.Count; i++)
      {
        HardwareBatch b = target.Fleet[i];
        if (b.HoldingId != HardwareBatch.Hq) continue;
        b.Id = state.TakeId();
        buyer.Fleet.Add(b);
      }
    }

    public static void RunMonth(GameConfig c, GameState state)
    {
      if (!BubbleSystem.InCrash(state)) return;
      if (c.Acquisitions == null) return;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState buyer = state.Labs[i];
        if (buyer.Controller != LabController.Plan || buyer.Bankrupt || buyer.AbsorbedByLabId >= 0) continue;
        for (int k = 0; k < state.Labs.Count; k++)
        {
          LabState target = state.Labs[k];
          if (target.Id == buyer.Id || target.Controller != LabController.Plan || target.Bankrupt || target.AbsorbedByLabId >= 0) continue;
          if (target.AbsorbedByLabId >= 0 || !IsDistressed(c, state, target)) continue;
          if (!CanAcquire(c, state, buyer, target, out _)) continue;
          if (!state.Rng.Rivals.Chance(60)) continue;
          Apply(c, state, buyer, target);
          return;
        }
      }
    }
  }
}