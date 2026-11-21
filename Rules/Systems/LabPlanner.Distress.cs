using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Crash distress: model fire-sales, chip dump listings, and peer acquisitions for plan-run labs.</summary>
    public static partial class LabPlanner
    {
        static void CrashDistress(Ctx x)
        {
            if (!BubbleSystem.InCrash(x.State)) return;
            if (AcquisitionSystem.IsDistressed(x.C, x.State, x.Lab))
            {
                TryFireSellModel(x);
                TryDistressChipListings(x);
            }
            else
                TryAcquireDistressedPeer(x);
        }

        /// <summary>Distressed crash sellers still list chips even when the plan returns early for debt.</summary>
        static void TryDistressChipListings(Ctx x)
        {
            if (x.Plan.ListsChipsMarkupPermille <= 0) return;
            if (x.C.Chips.DistressListingPricePermille <= 0) return;
            ListChips(x);
        }

        /// <summary>Sell the weakest released model once per quarter while distressed in crash.</summary>
        static void TryFireSellModel(Ctx x)
        {
            LabState lab = x.Lab;
            if (lab.WeeksInDebt < 1 && lab.Cash >= 0) return;
            ModelState pick = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released && m.Status != ModelStatus.Trained) continue;
                if (!ModelSalesSystem.CanSell(lab, m, out _)) continue;
                if (pick == null || m.Capability < pick.Capability) pick = m;
            }
            if (pick == null) return;
            int quarter = DecisionLog.QuarterOf(x.C, x.State.Tick);
            for (int i = lab.Log.Count - 1; i >= 0; i--)
            {
                DecisionEntry e = lab.Log[i];
                if (DecisionLog.QuarterOf(x.C, e.Tick) != quarter) break;
                if (e.Action == DecisionAction.SoldModel) return;
            }
            var q = ModelSalesSystem.Quote(x.C, x.State, lab, pick);
            if (Decide(x, Command.SellModel(lab.Id, pick.Id), DecisionAction.SoldModel, pick.Name, q.OfferCash,
                    Reason.Of(ReasonCode.CrashDistress, x.C.ModelSales.CrashDistressOfferPermille),
                    Reason.Of(ReasonCode.InDebt, lab.WeeksInDebt)).Ok)
                return;
        }

        /// <summary>A cash-rich rival tries to buy the most distressed eligible peer.</summary>
        static void TryAcquireDistressedPeer(Ctx x)
        {
            AcquisitionConfig cfg = x.C.Acquisitions;
            if (cfg == null || cfg.PlannerAcquireChancePermille <= 0) return;
            if (!x.State.Rng.Rivals.Chance(cfg.PlannerAcquireChancePermille)) return;
            int quarter = DecisionLog.QuarterOf(x.C, x.State.Tick);
            for (int i = x.Lab.Log.Count - 1; i >= 0; i--)
            {
                DecisionEntry e = x.Lab.Log[i];
                if (DecisionLog.QuarterOf(x.C, e.Tick) != quarter) break;
                if (e.Action == DecisionAction.AcquiredLab) return;
            }

            LabState buyer = x.Lab;
            LabState target = null;
            for (int i = 0; i < x.State.Labs.Count; i++)
            {
                LabState t = x.State.Labs[i];
                if (t.Id == buyer.Id || t.Controller != LabController.Plan || t.Bankrupt || t.AbsorbedByLabId >= 0) continue;
                if (!AcquisitionSystem.IsDistressed(x.C, x.State, t)) continue;
                if (!AcquisitionSystem.CanAcquire(x.C, x.State, buyer, t, out _)) continue;
                if (target == null || t.Cash < target.Cash) target = t;
            }
            if (target == null) return;
            AcquisitionSystem.AcquisitionQuote q = AcquisitionSystem.Quote(x.C, x.State, buyer, target);
            Decide(x, Command.AcquireLab(buyer.Id, target.Id), DecisionAction.AcquiredLab, target.Name, q.Total,
                Reason.Of(ReasonCode.CrashDistress, 1), Reason.Of(ReasonCode.InDebt, target.WeeksInDebt));
        }
    }
}