using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q208: player enterprise seat pipeline — pursue, sign tier, weekly SLA strikes, revenue bonus from market share.</summary>
    public static class EnterpriseDealSystem
    {
        public static bool Enabled(GameConfig c, GameState state, LabState lab)
        {
            if (lab == null || lab.Id != state.PlayerLabId || c.EnterpriseDeals == null) return false;
            return GameDate.Year(c.Calendar, state.Tick) >= c.EnterpriseDeals.OpensYear;
        }

        public static bool SectorEligible(GameConfig c, string sectorId)
        {
            var ids = c.EnterpriseDeals.EligibleSectorIds;
            for (int i = 0; i < ids.Count; i++) if (ids[i] == sectorId) return true;
            return false;
        }

        public static EnterpriseDealState DealForSector(LabState lab, string sectorId)
        {
            for (int i = 0; i < lab.EnterpriseDeals.Count; i++)
            {
                EnterpriseDealState d = lab.EnterpriseDeals[i];
                if (d.SectorId != sectorId) continue;
                if (d.Phase == EnterpriseDealPhase.Churned) continue;
                return d;
            }
            return null;
        }

        public static EnterpriseDealState ActiveOrProspect(LabState lab, string sectorId)
        {
            EnterpriseDealState d = DealForSector(lab, sectorId);
            if (d == null) return null;
            if (d.Phase == EnterpriseDealPhase.Completed) return null;
            return d;
        }

        public static bool ModelInSector(GameState state, LabState lab, int modelId, string sectorId)
        {
            SectorState ss = state.Sector(sectorId);
            if (ss == null) return false;
            SectorEntry e = ss.Entry(lab.Id);
            return e != null && e.ModelId == modelId;
        }

        public static long SectorRevenueLastWeek(GameState state, LabState lab, string sectorId)
        {
            SectorState ss = state.Sector(sectorId);
            if (ss == null) return 0;
            SectorEntry e = ss.Entry(lab.Id);
            return e == null ? 0 : e.LastRevenue;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt || lab.EnterpriseDeals.Count == 0) return;
            EnterpriseDealsConfig cfg = c.EnterpriseDeals;
            if (cfg == null) return;

            for (int i = 0; i < lab.EnterpriseDeals.Count; i++)
            {
                EnterpriseDealState deal = lab.EnterpriseDeals[i];
                if (deal.Phase == EnterpriseDealPhase.Prospect && deal.ProspectEndsTick > 0 && state.Tick >= deal.ProspectEndsTick)
                    deal.Phase = EnterpriseDealPhase.Churned;

                if (deal.Phase != EnterpriseDealPhase.Active && deal.Phase != EnterpriseDealPhase.AtRisk) continue;

                if (deal.EndsTick > 0 && state.Tick >= deal.EndsTick)
                {
                    deal.Phase = EnterpriseDealPhase.Completed;
                    Events.Add(state, EventKind.EnterpriseSeatCompleted, lab.Id, deal.ModelId, 0, deal.SectorId);
                    continue;
                }

                bool bound = ModelInSector(state, lab, deal.ModelId, deal.SectorId);
                int served = lab.LastWeek.ServedPermille;
                bool met = bound && served >= deal.SlaServedPermille;

                if (met)
                {
                    deal.Strikes = 0;
                    if (deal.Phase == EnterpriseDealPhase.AtRisk) deal.Phase = EnterpriseDealPhase.Active;
                    long extra = MathI.MulPermille(SectorRevenueLastWeek(state, lab, deal.SectorId), deal.RevenueBonusPermille);
                    if (extra > 0) Finance.Earn(lab, LedgerLine.Revenue, extra);
                }
                else
                {
                    if (ServiceLevelOverlap.PledgeServedBreachSuppressesDealStrike(c, lab))
                    {
                        // Q214: pledge (Q329) already punishes this served miss — no deal strike, churn rep, or slip event.
                        continue;
                    }
                    deal.Strikes++;
                    if (deal.Strikes >= deal.StrikeLimit)
                    {
                        deal.Phase = EnterpriseDealPhase.Churned;
                        int loss = cfg.ChurnReputationPenalty;
                        if (loss > 0) lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
                        Events.Add(state, EventKind.EnterpriseSeatChurned, lab.Id, deal.ModelId, loss, deal.SectorId);
                    }
                    else
                    {
                        deal.Phase = EnterpriseDealPhase.AtRisk;
                        Events.Add(state, EventKind.EnterpriseSeatSlip, lab.Id, deal.ModelId, served, deal.SectorId);
                    }
                }
            }
        }

        public static void BeginProspect(GameConfig c, GameState state, LabState lab, string sectorId, EnterpriseDealTierConfig tier)
        {
            var deal = new EnterpriseDealState
            {
                SectorId = sectorId,
                TierId = tier.Id,
                Phase = EnterpriseDealPhase.Prospect,
                ProspectEndsTick = state.Tick + c.EnterpriseDeals.ProspectWindowWeeks,
            };
            lab.EnterpriseDeals.Add(deal);
            Events.Add(state, EventKind.EnterpriseSeatPursued, lab.Id, -1, (int)tier.PursuitCash, sectorId);
        }

        public static void Sign(GameConfig c, GameState state, LabState lab, EnterpriseDealState deal, EnterpriseDealTierConfig tier, int modelId)
        {
            deal.Phase = EnterpriseDealPhase.Active;
            deal.ModelId = modelId;
            deal.SlaServedPermille = tier.SlaServedPermille;
            deal.RevenueBonusPermille = tier.RevenueBonusPermille;
            deal.StrikeLimit = tier.StrikeLimit;
            deal.Strikes = 0;
            deal.SignedTick = state.Tick;
            deal.EndsTick = state.Tick + tier.TermWeeks;
            deal.ProspectEndsTick = -1;
            Events.Add(state, EventKind.EnterpriseSeatSigned, lab.Id, modelId, tier.TermWeeks, deal.SectorId);
        }
    }
}