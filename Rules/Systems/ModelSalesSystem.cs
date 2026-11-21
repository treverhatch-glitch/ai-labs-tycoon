using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class ModelSalesSystem
  {
    public struct ModelSaleQuote
    {
      public long TrainingCostBasis;
      public long WeeklyIncome;
      public long OfferCash;
      public long IncomeForegonePerWeek;
      public List<string> Sectors;
    }

    public static long WeeklyIncome(GameState state, LabState lab, ModelState model)
    {
      long total = 0;
      for (int s = 0; s < state.Sectors.Count; s++)
      {
        SectorState sector = state.Sectors[s];
        for (int i = 0; i < sector.Entries.Count; i++)
        {
          SectorEntry e = sector.Entries[i];
          if (e.LabId == lab.Id && e.ModelId == model.Id) total += e.LastRevenue;
        }
      }
      return total;
    }

    public static ModelSaleQuote Quote(GameConfig c, GameState state, LabState lab, ModelState model)
    {
      ModelSalesConfig cfg = c.ModelSales;
      var q = new ModelSaleQuote { Sectors = new List<string>() };
      if (cfg == null || model == null) return q;
      ModelLineConfig line = c.Line(model.LineId);
      if (line != null) q.TrainingCostBasis = EraSystem.RunPrepCost(c, line, state.Tick);
      q.WeeklyIncome = WeeklyIncome(state, lab, model);
      q.IncomeForegonePerWeek = q.WeeklyIncome;
      if (q.WeeklyIncome > 0)
        q.OfferCash = q.WeeklyIncome * cfg.EarningsMultipleWeeks;
      else
        q.OfferCash = MathI.MulPermille(q.TrainingCostBasis, cfg.UnprofitableSalePermille);
      if (q.OfferCash < cfg.MinOffer) q.OfferCash = cfg.MinOffer;
      for (int i = 0; i < model.Sectors.Count; i++) q.Sectors.Add(model.Sectors[i]);
      return q;
    }

    public static bool CanSell(LabState lab, ModelState model, out string reason)
    {
      reason = "";
      if (model == null) { reason = "Unknown model."; return false; }
      if (model.Status != ModelStatus.Released && model.Status != ModelStatus.Trained)
      {
        reason = model.Name + " cannot be sold in status " + model.Status + ".";
        return false;
      }
      int active = 0;
      for (int i = 0; i < lab.Models.Count; i++)
      {
        ModelState m = lab.Models[i];
        if (m.Status == ModelStatus.Released || m.Status == ModelStatus.Trained || m.Status == ModelStatus.Training) active++;
      }
      if (active <= 1)
      {
        reason = "Cannot sell the lab's last active model.";
        return false;
      }
      return true;
    }

    public static void Apply(GameConfig c, GameState state, LabState lab, ModelState model)
    {
      ModelSalesConfig cfg = c.ModelSales;
      ModelSaleQuote q = Quote(c, state, lab, model);
      Finance.Earn(lab, LedgerLine.CapitalIn, q.OfferCash);
      int hit = cfg.SectorReputationHit;
      for (int s = 0; s < state.Sectors.Count; s++)
      {
        SectorState sector = state.Sectors[s];
        for (int i = sector.Entries.Count - 1; i >= 0; i--)
        {
          SectorEntry e = sector.Entries[i];
          if (e.LabId == lab.Id && e.ModelId == model.Id) sector.Entries.RemoveAt(i);
        }
        if (model.Sectors.Contains(sector.Id))
          lab.Reputation = (int)MathI.Max(0, lab.Reputation - hit);
      }
      model.Sectors.Clear();
      model.Status = ModelStatus.Retired;
      Events.Add(state, EventKind.ModelSold, lab.Id, model.Id, q.OfferCash, model.Name);
    }
  }
}