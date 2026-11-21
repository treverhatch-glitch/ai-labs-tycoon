using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Cash-aware growth caps so rivals retrench before debt spirals (Q200).</summary>
    public static partial class LabPlanner
    {
        static int RunwayWeeks(Ctx x)
        {
            long cost = LabQueries.WeeklyRunningCost(x.C, x.State, x.Lab);
            if (cost <= 0) return 999;
            return (int)MathI.Clamp(x.Lab.Cash / cost, 0, 999);
        }

        /// <summary>True when the lab should stop wealth-fuelled expansion and live within revenue.</summary>
        static bool Retrenching(Ctx x)
        {
            LabState lab = x.Lab;
            if (lab.WeeksInDebt > 0) return true;
            long reserve = Reserve(x);
            if (Overstretched(x) && lab.Cash < reserve * 2) return true;
            if (lab.Cash < reserve / 2) return true;
            if (RunwayWeeks(x) <= x.Plan.ReserveWeeks / 2) return true;
            return false;
        }

        /// <summary>Trim rent toward plan floor before cash goes negative.</summary>
        static void Retrench(Ctx x)
        {
            if (x.Lab.WeeksInDebt > 0) return;
            if (!Retrenching(x)) return;
            TryLiquidityModelSale(x);
            LabState lab = x.Lab;
            int floor = x.Plan.Gpus;
            int runway = RunwayWeeks(x);
            if (runway <= Math.Max(1, x.Plan.ReserveWeeks / 2) && Overstretched(x))
                TryTrimStaff(x);
            if (lab.RentedGpus > floor && lab.RentedGpus > 0)
            {
                int cut = runway <= Math.Max(1, x.Plan.ReserveWeeks / 2) ? (lab.RentedGpus + 4) / 5 : (lab.RentedGpus + 9) / 10;
                int rent = lab.RentedGpus - cut;
                if (rent < floor) rent = floor;
                if (rent < lab.RentedGpus)
                    Decide(x, Command.SetRentedGpus(lab.Id, rent), DecisionAction.Tightened, "", lab.RentedGpus - rent,
                        Reason.Of(ReasonCode.RunwayWeeks, RunwayWeeks(x)));
            }
        }

        /// <summary>Sell the weakest sellable model when runway is inside the reserve window (before debt).</summary>
        static void TryLiquidityModelSale(Ctx x)
        {
            int runway = RunwayWeeks(x);
            int reserveWindow = x.Plan.ReserveWeeks;
            if (runway > reserveWindow) return;
            LabState lab = x.Lab;
            ModelState pick = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released && m.Status != ModelStatus.Trained) continue;
                if (!ModelSalesSystem.CanSell(lab, m, out _)) continue;
                if (pick == null || m.Capability < pick.Capability) pick = m;
            }
            if (pick == null) return;
            bool urgent = runway <= Math.Max(1, reserveWindow / 2);
            int period = urgent ? GameDate.Month(x.C.Calendar, x.State.Tick) : DecisionLog.QuarterOf(x.C, x.State.Tick);
            for (int i = lab.Log.Count - 1; i >= 0; i--)
            {
                DecisionEntry e = lab.Log[i];
                int ePeriod = urgent ? GameDate.Month(x.C.Calendar, e.Tick) : DecisionLog.QuarterOf(x.C, e.Tick);
                if (ePeriod != period) break;
                if (e.Action == DecisionAction.SoldModel) return;
            }
            var q = ModelSalesSystem.Quote(x.C, x.State, lab, pick);
            Decide(x, Command.SellModel(lab.Id, pick.Id), DecisionAction.SoldModel, pick.Name, q.OfferCash,
                Reason.Of(ReasonCode.RunwayWeeks, RunwayWeeks(x)));
        }

        static void TryTrimStaff(Ctx x)
        {
            int quarter = DecisionLog.QuarterOf(x.C, x.State.Tick);
            LabState lab = x.Lab;
            for (int i = lab.Log.Count - 1; i >= 0; i--)
            {
                DecisionEntry e = lab.Log[i];
                if (DecisionLog.QuarterOf(x.C, e.Tick) != quarter) break;
                if (e.Action == DecisionAction.Tightened && e.Reasons.Any(r => r.Code == ReasonCode.RunwayWeeks)) return;
            }
            string biggestRole = "";
            int biggestCount = 0;
            GameConfig cfg = x.C;
            for (int i = 0; i < cfg.Roles.Count; i++)
            {
                int n = LabQueries.StaffCount(lab, cfg.Roles[i].Id);
                if (n > biggestCount) { biggestCount = n; biggestRole = cfg.Roles[i].Id; }
            }
            if (biggestCount <= 1) return;
            Decide(x, Command.Fire(lab.Id, biggestRole, 1), DecisionAction.Tightened, biggestRole, 1,
                Reason.Of(ReasonCode.RunwayWeeks, RunwayWeeks(x)));
        }

        static int CapGrowthTarget(Ctx x, int grown, int have, int planBaseline)
        {
            if (!Retrenching(x)) return grown;
            int cap = have > planBaseline ? have : planBaseline;
            return grown < cap ? grown : cap;
        }
    }
}