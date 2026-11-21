using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q215: player data license pipeline — legal access to risky mixes, weekly training-use validity.</summary>
    public static class DataLicenseSystem
    {
        public static bool Enabled(GameConfig c, GameState state, LabState lab)
        {
            if (lab == null || lab.Id != state.PlayerLabId || c.DataLicenses == null) return false;
            return GameDate.Year(c.Calendar, state.Tick) >= c.DataLicenses.OpensYear;
        }

        public static bool SourceRequiresLicense(GameConfig c, string sourceId)
        {
            var ids = c.DataLicenses?.LicenseableSourceIds;
            if (ids == null) return false;
            for (int i = 0; i < ids.Count; i++) if (ids[i] == sourceId) return true;
            return false;
        }

        public static bool MayTrainOnSource(GameConfig c, GameState state, LabState lab, string sourceId)
        {
            if (!SourceRequiresLicense(c, sourceId)) return true;
            if (!Enabled(c, state, lab)) return true;
            DataLicenseState lic = ActiveLicenseForSource(lab, sourceId);
            return lic != null && lic.Phase == DataLicensePhase.Active;
        }

        public static int PrepDiscountPermille(GameConfig c, GameState state, LabState lab, string sourceId)
        {
            if (!Enabled(c, state, lab)) return 0;
            DataLicenseState lic = ActiveLicenseForSource(lab, sourceId);
            if (lic == null || lic.Phase != DataLicensePhase.Active) return 0;
            return lic.PrepDiscountPermille;
        }

        public static DataLicenseState LicenseForSource(LabState lab, string sourceId)
        {
            for (int i = 0; i < lab.DataLicenses.Count; i++)
            {
                DataLicenseState d = lab.DataLicenses[i];
                if (d.SourceId != sourceId) continue;
                if (d.Phase == DataLicensePhase.Revoked) continue;
                return d;
            }
            return null;
        }

        public static DataLicenseState ActiveLicenseForSource(LabState lab, string sourceId)
        {
            DataLicenseState d = LicenseForSource(lab, sourceId);
            if (d == null) return null;
            if (d.Phase == DataLicensePhase.Completed) return null;
            return d;
        }

        public static bool TrainedOnSourceThisWeek(LabState lab, string sourceId)
        {
            if (lab.LastWeek.TrainingSourceIds == null) return false;
            for (int i = 0; i < lab.LastWeek.TrainingSourceIds.Count; i++)
                if (lab.LastWeek.TrainingSourceIds[i] == sourceId) return true;
            return false;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt || lab.DataLicenses.Count == 0) return;
            DataLicensesConfig cfg = c.DataLicenses;
            if (cfg == null) return;

            for (int i = 0; i < lab.DataLicenses.Count; i++)
            {
                DataLicenseState deal = lab.DataLicenses[i];
                if (deal.Phase == DataLicensePhase.Prospect && deal.ProspectEndsTick > 0 && state.Tick >= deal.ProspectEndsTick)
                    deal.Phase = DataLicensePhase.Revoked;

                if (deal.Phase != DataLicensePhase.Active && deal.Phase != DataLicensePhase.AtRisk) continue;

                if (deal.EndsTick > 0 && state.Tick >= deal.EndsTick)
                {
                    deal.Phase = DataLicensePhase.Completed;
                    Events.Add(state, EventKind.DataLicenseCompleted, lab.Id, -1, 0, deal.SourceId);
                    continue;
                }

                bool used = TrainedOnSourceThisWeek(lab, deal.SourceId);
                if (used)
                {
                    deal.Strikes = 0;
                    if (deal.Phase == DataLicensePhase.AtRisk) deal.Phase = DataLicensePhase.Active;
                    if (deal.WeeklyResearchBonus > 0)
                        lab.ResearchPoints += deal.WeeklyResearchBonus;
                }
                else
                {
                    deal.Strikes++;
                    if (deal.Strikes >= deal.StrikeLimit)
                    {
                        deal.Phase = DataLicensePhase.Revoked;
                        int loss = cfg.RevokedReputationPenalty;
                        if (loss > 0) lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
                        Events.Add(state, EventKind.DataLicenseRevoked, lab.Id, -1, loss, deal.SourceId);
                    }
                    else
                    {
                        deal.Phase = DataLicensePhase.AtRisk;
                        Events.Add(state, EventKind.DataLicenseMissed, lab.Id, -1, deal.Strikes, deal.SourceId);
                    }
                }
            }
        }

        public static void BeginProspect(GameConfig c, GameState state, LabState lab, string sourceId, DataLicenseTierConfig tier)
        {
            var deal = new DataLicenseState
            {
                SourceId = sourceId,
                TierId = tier.Id,
                Phase = DataLicensePhase.Prospect,
                ProspectEndsTick = state.Tick + c.DataLicenses.ProspectWindowWeeks,
            };
            lab.DataLicenses.Add(deal);
            Events.Add(state, EventKind.DataLicensePursued, lab.Id, -1, (int)tier.PursuitCash, sourceId);
        }

        public static void Sign(GameConfig c, GameState state, LabState lab, DataLicenseState deal, DataLicenseTierConfig tier)
        {
            deal.Phase = DataLicensePhase.Active;
            deal.PrepDiscountPermille = tier.PrepDiscountPermille;
            deal.WeeklyResearchBonus = tier.WeeklyResearchBonus;
            deal.StrikeLimit = tier.StrikeLimit;
            deal.Strikes = 0;
            deal.SignedTick = state.Tick;
            deal.EndsTick = state.Tick + tier.TermWeeks;
            deal.ProspectEndsTick = -1;
            Events.Add(state, EventKind.DataLicenseSigned, lab.Id, -1, tier.TermWeeks, deal.SourceId);
        }
    }
}