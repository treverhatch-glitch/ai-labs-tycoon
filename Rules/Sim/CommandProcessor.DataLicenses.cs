using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PursueDataLicense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!DataLicenseSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Data licenses are not open yet.");
            string sourceId = cmd.Id;
            if (string.IsNullOrEmpty(sourceId))
                return Fail(CommandError.InvalidArgument, "Name a data source.");
            if (!DataLicenseSystem.SourceRequiresLicense(c, sourceId))
                return Fail(CommandError.InvalidArgument, "That data source does not use a license deal.");
            DataSourceConfig source = c.Training.DataSource(sourceId);
            if (source == null) return Fail(CommandError.UnknownId, "There is no data source \"" + sourceId + "\".");
            if (!TrainingQueries.DataSourceOpen(c, source, state.Tick))
                return Fail(CommandError.EraLocked, source.Name + " opens in " + source.OpensYear + ".");
            if (DataLicenseSystem.ActiveLicenseForSource(lab, sourceId) != null)
                return Fail(CommandError.InvalidArgument, "There is already a license pipeline for that source.");
            if (LabQueries.StaffByFunction(c, lab, RoleFunction.Research) < c.DataLicenses.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "Data licenses need at least " + c.DataLicenses.MinResearchers + " researchers.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : null;
            DataLicenseTierConfig tier = string.IsNullOrEmpty(tierId) ? DefaultDataTier(c) : c.DataLicenses.Tier(tierId);
            if (tier == null) return Fail(CommandError.InvalidArgument, "No data license tiers are configured.");
            if (lab.Bankrupt) return Fail(CommandError.LabBankrupt, lab.Name + " is bankrupt.");
            if (lab.Cash < tier.PursuitCash) return NeedCash(lab, tier.PursuitCash);
            Finance.Spend(lab, LedgerLine.Hiring, tier.PursuitCash);
            DataLicenseSystem.BeginProspect(c, state, lab, sourceId, tier);
            return CommandResult.Done(-1);
        }

        static CommandResult SignDataLicense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!DataLicenseSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Data licenses are not open yet.");
            string sourceId = cmd.Id;
            if (string.IsNullOrEmpty(sourceId)) return Fail(CommandError.InvalidArgument, "Name a data source.");
            DataLicenseState deal = DataLicenseSystem.LicenseForSource(lab, sourceId);
            if (deal == null || deal.Phase != DataLicensePhase.Prospect)
                return Fail(CommandError.InvalidArgument, "Pursue a data license for that source before signing.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : deal.TierId;
            DataLicenseTierConfig tier = c.DataLicenses.Tier(tierId);
            if (tier == null) return Fail(CommandError.UnknownId, "There is no tier \"" + tierId + "\".");
            if (lab.Cash < tier.SignCash) return NeedCash(lab, tier.SignCash);
            Finance.Spend(lab, LedgerLine.Hiring, tier.SignCash);
            DataLicenseSystem.Sign(c, state, lab, deal, tier);
            return CommandResult.Done(-1);
        }

        static CommandResult WalkDataLicense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            string sourceId = cmd.Id;
            DataLicenseState deal = DataLicenseSystem.LicenseForSource(lab, sourceId);
            if (deal == null || deal.Phase != DataLicensePhase.Prospect)
                return Fail(CommandError.InvalidArgument, "There is no prospect to walk away from.");
            deal.Phase = DataLicensePhase.Revoked;
            return CommandResult.Done(-1);
        }

        static CommandResult TerminateDataLicense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            int idx = cmd.Count;
            if (idx < 0 || idx >= lab.DataLicenses.Count)
                return Fail(CommandError.InvalidArgument, "There is no data license at index " + idx + ".");
            DataLicenseState deal = lab.DataLicenses[idx];
            if (deal.Phase != DataLicensePhase.Active && deal.Phase != DataLicensePhase.AtRisk)
                return Fail(CommandError.InvalidArgument, "Only an active license can be terminated early.");
            DataLicenseTierConfig tier = c.DataLicenses.Tier(deal.TierId);
            long penalty = tier?.BreakPenalty ?? 0;
            if (penalty > 0)
            {
                if (lab.Cash < penalty) return NeedCash(lab, penalty);
                Finance.Spend(lab, LedgerLine.Fines, penalty);
            }
            deal.Phase = DataLicensePhase.Revoked;
            Events.Add(state, EventKind.DataLicenseTerminated, lab.Id, -1, penalty, deal.SourceId);
            return CommandResult.Done(-1);
        }

        static CommandResult RenewDataLicense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            string sourceId = cmd.Id;
            DataLicenseState deal = DataLicenseSystem.LicenseForSource(lab, sourceId);
            if (deal == null || deal.Phase != DataLicensePhase.Completed)
                return Fail(CommandError.InvalidArgument, "Renew only after a license term completed.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : deal.TierId;
            DataLicenseTierConfig tier = c.DataLicenses.Tier(tierId);
            if (tier == null) return Fail(CommandError.UnknownId, "There is no tier \"" + tierId + "\".");
            if (lab.Cash < tier.SignCash) return NeedCash(lab, tier.SignCash);
            Finance.Spend(lab, LedgerLine.Hiring, tier.SignCash);
            deal.Phase = DataLicensePhase.Active;
            deal.TierId = tierId;
            deal.PrepDiscountPermille = tier.PrepDiscountPermille;
            deal.WeeklyResearchBonus = tier.WeeklyResearchBonus;
            deal.StrikeLimit = tier.StrikeLimit;
            deal.Strikes = 0;
            deal.SignedTick = state.Tick;
            deal.EndsTick = state.Tick + tier.TermWeeks;
            Events.Add(state, EventKind.DataLicenseRenewed, lab.Id, -1, tier.TermWeeks, sourceId);
            return CommandResult.Done(-1);
        }

        static DataLicenseTierConfig DefaultDataTier(GameConfig c)
        {
            return c.DataLicenses.Tiers.Count > 0 ? c.DataLicenses.Tiers[0] : null;
        }
    }
}