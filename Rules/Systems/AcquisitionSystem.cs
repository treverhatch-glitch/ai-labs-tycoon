using System.Collections.Generic;
using AiLabs.Rules.Core;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class AcquisitionSystem
  {
    public struct AcquisitionQuote
    {
      public long FleetPart;
      public long ChipsPart;
      public long ModelsPart;
      public long StaffPart;
      public long CashPart;
      public long DebtAssumed;
      public long IntegrationCost;
      public long Total;
      public bool Distressed;
    }

    public static bool IsDistressed(GameConfig c, GameState state, LabState target)
    {
      if (target.Bankrupt) return true;
      if (target.Cash < 0 || target.WeeksInDebt > 0) return true;
      if (BubbleSystem.InCrash(state))
      {
        BubbleConfig bubble = c.Bubble;
        if (bubble == null) return false;
        int share = GovernmentSystem.RevenueSharePpm(state, target);
        return share < bubble.SurvivorRevenuePpm;
      }
      return false;
    }

    public static AcquisitionQuote Quote(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      AcquisitionConfig cfg = c.Acquisitions;
      var q = new AcquisitionQuote();
      if (cfg == null || buyer == null || target == null) return q;
      q.Distressed = IsDistressed(c, state, target);
      q.CashPart = target.Cash > 0 ? target.Cash : 0;
      q.DebtAssumed = target.Cash < 0 ? -target.Cash : 0;
      for (int i = 0; i < target.Models.Count; i++)
      {
        ModelState m = target.Models[i];
        if (m.Status == ModelStatus.Retired || m.Status == ModelStatus.Training) continue;
        q.ModelsPart += (long)m.Capability * cfg.CapabilityValuePerPoint;
      }
      q.StaffPart = (long)target.Staff.Count * cfg.StaffValuePerHead;
      q.FleetPart = LabQueries.FleetResaleValue(c, state, target);
      q.ChipsPart = ReadyChipBookValue(c, target, cfg.ReadyChipBookPermille)
        + ChipFleetBookValue(c, state, target, cfg.ChipFleetBookPermille);
      long distressedBundle = q.ModelsPart + q.StaffPart + q.ChipsPart;
      if (q.Distressed)
      {
        int permille = BubbleSystem.InCrash(state) ? cfg.CrashDistressPricePermille : cfg.DistressPricePermille;
        distressedBundle = MathI.MulPermille(distressedBundle, permille);
      }
      long subtotal = distressedBundle + q.FleetPart + q.CashPart;
      subtotal = (long)MathI.Max(0, subtotal - q.DebtAssumed);
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
      if (BubbleSystem.InCrash(state) && !IsDistressed(c, state, target))
      {
        reason = target.Name + " is not distressed enough to sell during the crash.";
        return false;
      }
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
      Finance.Spend(buyer, LedgerLine.Acquisition, q.Total);
      long cash = target.Cash;
      if (cash > 0)
      {
        Finance.Earn(buyer, LedgerLine.CapitalIn, cash);
        Finance.Spend(target, LedgerLine.CapitalIn, cash);
      }
      else if (cash < 0)
        Finance.Spend(buyer, LedgerLine.Acquisition, -cash);
      target.Cash = 0;
      TransferModels(c, state, buyer, target);
      TransferStaff(c, buyer, target);
      TransferChipDesigns(buyer, target);
      TransferChipListings(buyer, target);
      Dictionary<int, int> holdingMap = TransferHoldingsAndFleet(c, state, buyer, target);
      TransferChipOrders(buyer, target, holdingMap);
      for (int s = 0; s < state.Sectors.Count; s++)
      {
        SectorState sector = state.Sectors[s];
        for (int i = sector.Entries.Count - 1; i >= 0; i--)
          if (sector.Entries[i].LabId == target.Id) sector.Entries.RemoveAt(i);
        if (sector.ChampionLabId == target.Id) sector.ChampionLabId = buyer.Id;
      }
      SealAbsorbed(state, target);
      target.AbsorbedByLabId = buyer.Id;
      if (q.Total >= c.Acquisitions.RegulatorDealRevenue)
        RegulatorSystem.OnAcquisition(c, state, buyer, c.Acquisitions.RegulatorAttention);
      Events.Add(state, EventKind.LabAcquired, buyer.Id, target.Id, q.Total, target.RivalId);
    }

    static void SealAbsorbed(GameState state, LabState target)
    {
      ChipSystem.OnBankrupt(state, target);
      target.Models.Clear();
      target.Staff.Clear();
      target.Fleet.Clear();
      target.Holdings.Clear();
      target.RentedGpus = 0;
      target.ReservedContracts.Clear();
    }

    static void TransferModels(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      for (int i = 0; i < target.Models.Count; i++)
      {
        ModelState m = target.Models[i];
        if (m.Status == ModelStatus.Retired) continue;
        buyer.Models.Add(m);
        for (int s = 0; s < m.Sectors.Count; s++)
        {
          SectorState sector = state.Sector(m.Sectors[s]);
          if (sector == null) continue;
          SectorEntry entry = sector.Entry(buyer.Id);
          ModelState incumbent = entry == null ? null : buyer.Model(entry.ModelId);
          if (incumbent != null && incumbent.Capability >= m.Capability) continue;
          if (entry == null)
          {
            entry = new SectorEntry { LabId = buyer.Id, ModelId = m.Id };
            sector.Entries.Add(entry);
          }
          else entry.ModelId = m.Id;
        }
      }
    }

    static void TransferChipDesigns(LabState buyer, LabState target)
    {
      for (int i = 0; i < target.ChipDesigns.Count; i++) buyer.ChipDesigns.Add(target.ChipDesigns[i]);
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

    static Dictionary<int, int> TransferHoldingsAndFleet(GameConfig c, GameState state, LabState buyer, LabState target)
    {
      var holdingMap = new Dictionary<int, int>();
      for (int i = 0; i < target.Holdings.Count; i++)
      {
        Holding h = target.Holdings[i];
        int oldId = h.Id;
        h.Id = state.TakeId();
        holdingMap[oldId] = h.Id;
        buyer.Holdings.Add(h);
      }
      AcquisitionConfig acq = c.Acquisitions;
      int lockUntil = -1;
      if (acq != null && acq.FleetResaleLockWeeks > 0) lockUntil = state.Tick + acq.FleetResaleLockWeeks;
      for (int i = 0; i < target.Fleet.Count; i++)
      {
        HardwareBatch b = target.Fleet[i];
        if (b.HoldingId != HardwareBatch.Hq && holdingMap.TryGetValue(b.HoldingId, out int mapped)) b.HoldingId = mapped;
        else if (b.HoldingId != HardwareBatch.Hq) continue;
        b.Id = state.TakeId();
        if (b.DesignLabId == target.Id) b.DesignLabId = buyer.Id;
        if (lockUntil >= 0 && !string.IsNullOrEmpty(b.GpuGenId) && b.ChipDesignId <= 0) b.ResaleLockedUntilTick = lockUntil;
        buyer.Fleet.Add(b);
      }
      return holdingMap;
    }

    static void TransferChipOrders(LabState buyer, LabState target, Dictionary<int, int> holdingMap)
    {
      for (int i = 0; i < target.ChipOrders.Count; i++)
      {
        ChipOrder o = target.ChipOrders[i];
        o.BuyerLabId = buyer.Id;
        if (o.DesignLabId == target.Id) o.DesignLabId = buyer.Id;
        if (o.HoldingId != HardwareBatch.Cloud && o.HoldingId != HardwareBatch.Hq
            && holdingMap.TryGetValue(o.HoldingId, out int mapped))
          o.HoldingId = mapped;
        buyer.ChipOrders.Add(o);
      }
      target.ChipOrders.Clear();
    }

    static long ReadyChipBookValue(GameConfig c, LabState target, int bookPermille)
    {
      if (bookPermille <= 0 || target.ChipDesigns.Count == 0) return 0;
      long sum = 0;
      for (int i = 0; i < target.ChipDesigns.Count; i++)
      {
        ChipDesign d = target.ChipDesigns[i];
        if (d.Status != ChipDesignStatus.Ready) continue;
        if (c.Chips == null || string.IsNullOrEmpty(d.NodeId)) continue;
        ChipNodeConfig node = c.Chips.Node(d.NodeId);
        if (node == null) continue;
        sum += MathI.MulPermille(node.TapeoutCost, bookPermille);
      }
      return sum;
    }

    static long ChipFleetBookValue(GameConfig c, GameState state, LabState target, int bookPermille)
    {
      if (bookPermille <= 0 || target.Fleet.Count == 0) return 0;
      long sum = 0;
      for (int i = 0; i < target.Fleet.Count; i++)
      {
        HardwareBatch batch = target.Fleet[i];
        if (batch.ChipDesignId <= 0 || batch.Count <= 0) continue;
        ChipDesign d = target.Design(batch.ChipDesignId);
        if (d == null || d.Status != ChipDesignStatus.Ready) continue;
        ChipSpec spec = ChipQueries.Spec(c, d);
        if (!spec.Valid) continue;
        int yield = d.YieldPermille > 0 ? d.YieldPermille : MathI.Permille;
        long cost = ChipQueries.OrderCost(spec, yield, batch.Count);
        sum += MathI.MulPermille(cost, bookPermille);
      }
      return sum;
    }

    static void TransferChipListings(LabState buyer, LabState target)
    {
      for (int i = 0; i < target.ChipListings.Count; i++) buyer.ChipListings.Add(target.ChipListings[i]);
      target.ChipListings.Clear();
    }

    public static void RunMonth(GameConfig c, GameState state)
    {
      if (!BubbleSystem.InCrash(state)) return;
      AcquisitionConfig cfg = c.Acquisitions;
      if (cfg == null || cfg.RivalRollupChancePermille <= 0) return;
      var buyers = new List<LabState>();
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState buyer = state.Labs[i];
        if (buyer.Controller != LabController.Plan || buyer.Bankrupt || buyer.AbsorbedByLabId >= 0) continue;
        buyers.Add(buyer);
      }
      buyers.Sort((a, b) => b.Cash.CompareTo(a.Cash));
      for (int i = 0; i < buyers.Count; i++)
      {
        LabState buyer = buyers[i];
        var targets = new List<LabState>();
        for (int k = 0; k < state.Labs.Count; k++)
        {
          LabState target = state.Labs[k];
          if (target.Id == buyer.Id || target.Controller != LabController.Plan || target.Bankrupt || target.AbsorbedByLabId >= 0) continue;
          if (!IsDistressed(c, state, target)) continue;
          if (!CanAcquire(c, state, buyer, target, out _)) continue;
          targets.Add(target);
        }
        targets.Sort((a, b) => a.Cash.CompareTo(b.Cash));
        for (int k = 0; k < targets.Count; k++)
        {
          if (!state.Rng.Rivals.Chance(cfg.RivalRollupChancePermille)) continue;
          Apply(c, state, buyer, targets[k]);
          return;
        }
      }
    }
  }
}