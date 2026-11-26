using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PursueEnterpriseSeat(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!EnterpriseDealSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Enterprise seat deals are not open yet.");
            if (string.IsNullOrEmpty(cmd.Id))
                return Fail(CommandError.InvalidArgument, "Name a sector.");
            if (!EnterpriseDealSystem.SectorEligible(c, cmd.Id))
                return Fail(CommandError.InvalidArgument, "That sector does not run enterprise seat contracts.");
            if (ActiveOrProspect(c, state, lab, cmd.Id) != null)
                return Fail(CommandError.InvalidArgument, "There is already a deal pipeline for that sector.");
            SectorConfig sector = c.Sector(cmd.Id);
            if (!LabQueries.SectorOpen(c, sector, state.Tick))
                return Fail(CommandError.SectorClosed, sector.Name + " opens in " + sector.OpensYear + ".");
            SectorState ss = state.Sector(cmd.Id);
            if (ss.Entry(lab.Id) == null)
                return Fail(CommandError.InvalidArgument, "Put a model on sale in " + sector.Name + " before pursuing a seat deal.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : null;
            EnterpriseDealTierConfig tier = string.IsNullOrEmpty(tierId) ? DefaultTier(c) : c.EnterpriseDeals.Tier(tierId);
            if (tier == null) return Fail(CommandError.InvalidArgument, "No enterprise deal tiers are configured.");
            if (LabQueries.StaffByFunction(c, lab, RoleFunction.Sales) < c.EnterpriseDeals.MinSalesStaff)
                return Fail(CommandError.NotEnoughStaff, "Enterprise deals need at least " + c.EnterpriseDeals.MinSalesStaff + " sales staff.");
            if (lab.Cash < tier.PursuitCash) return NeedCash(lab, tier.PursuitCash);
            Finance.Spend(lab, LedgerLine.Hiring, tier.PursuitCash);
            EnterpriseDealSystem.BeginProspect(c, state, lab, cmd.Id, tier);
            return CommandResult.Done(-1);
        }

        static CommandResult SignEnterpriseSeat(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!EnterpriseDealSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Enterprise seat deals are not open yet.");
            string sectorId = cmd.Id;
            if (string.IsNullOrEmpty(sectorId)) return Fail(CommandError.InvalidArgument, "Name a sector.");
            EnterpriseDealState deal = ActiveOrProspect(c, state, lab, sectorId);
            if (deal == null || deal.Phase != EnterpriseDealPhase.Prospect)
                return Fail(CommandError.InvalidArgument, "Pursue a seat deal in that sector before signing.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : deal.TierId;
            EnterpriseDealTierConfig tier = c.EnterpriseDeals.Tier(tierId);
            if (tier == null) return Fail(CommandError.UnknownId, "There is no tier \"" + tierId + "\".");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released)
                return Fail(CommandError.WrongModelStatus, model.Name + " must be on sale.");
            if (!EnterpriseDealSystem.ModelInSector(state, lab, model.Id, sectorId))
                return Fail(CommandError.InvalidArgument, model.Name + " is not the model on sale in that sector.");
            long cost = tier.SignCash;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            EnterpriseDealSystem.Sign(c, state, lab, deal, tier, model.Id);
            return CommandResult.Done(model.Id);
        }

        static CommandResult WalkEnterpriseSeat(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            EnterpriseDealState deal = ActiveOrProspect(c, state, lab, cmd.Id);
            if (deal == null || deal.Phase != EnterpriseDealPhase.Prospect)
                return Fail(CommandError.InvalidArgument, "There is no open prospect for that sector.");
            deal.Phase = EnterpriseDealPhase.Churned;
            return CommandResult.Done(-1);
        }

        static CommandResult TerminateEnterpriseSeat(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            int index = cmd.Count;
            if (index < 0 || index >= lab.EnterpriseDeals.Count) return Fail(CommandError.InvalidArgument, "No such deal.");
            EnterpriseDealState deal = lab.EnterpriseDeals[index];
            if (deal.Phase != EnterpriseDealPhase.Active && deal.Phase != EnterpriseDealPhase.AtRisk)
                return Fail(CommandError.InvalidArgument, "Only an active seat deal can be terminated early.");
            EnterpriseDealTierConfig tier = c.EnterpriseDeals.Tier(deal.TierId);
            long penalty = tier?.BreakPenalty ?? 0;
            if (penalty > 0 && lab.Cash < penalty) return NeedCash(lab, penalty);
            if (penalty > 0) Finance.Spend(lab, LedgerLine.Fines, penalty);
            deal.Phase = EnterpriseDealPhase.Churned;
            Events.Add(state, EventKind.EnterpriseSeatTerminated, lab.Id, deal.ModelId, penalty, deal.SectorId);
            return CommandResult.Done(index);
        }

        static CommandResult RenewEnterpriseSeat(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            EnterpriseDealState deal = EnterpriseDealSystem.DealForSector(lab, cmd.Id);
            if (deal == null || deal.Phase != EnterpriseDealPhase.Completed)
                return Fail(CommandError.InvalidArgument, "Finish a term before renewing that seat.");
            string tierId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : deal.TierId;
            EnterpriseDealTierConfig tier = c.EnterpriseDeals.Tier(tierId);
            if (tier == null) return Fail(CommandError.UnknownId, "Unknown tier.");
            ModelState model = lab.Model(deal.ModelId);
            if (model == null || !EnterpriseDealSystem.ModelInSector(state, lab, deal.ModelId, cmd.Id))
                return Fail(CommandError.InvalidArgument, "Bind the same model on sale in that sector before renewing.");
            if (lab.Cash < tier.SignCash) return NeedCash(lab, tier.SignCash);
            Finance.Spend(lab, LedgerLine.Hiring, tier.SignCash);
            deal.Phase = EnterpriseDealPhase.Active;
            deal.TierId = tierId;
            deal.SlaServedPermille = tier.SlaServedPermille;
            deal.RevenueBonusPermille = tier.RevenueBonusPermille;
            deal.StrikeLimit = tier.StrikeLimit;
            deal.Strikes = 0;
            deal.SignedTick = state.Tick;
            deal.EndsTick = state.Tick + tier.TermWeeks;
            Events.Add(state, EventKind.EnterpriseSeatRenewed, lab.Id, deal.ModelId, tier.TermWeeks, cmd.Id);
            return CommandResult.Done(deal.ModelId);
        }

        static EnterpriseDealState ActiveOrProspect(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            return EnterpriseDealSystem.ActiveOrProspect(lab, sectorId);
        }

        static EnterpriseDealTierConfig DefaultTier(GameConfig c)
        {
            return c.EnterpriseDeals.Tiers.Count > 0 ? c.EnterpriseDeals.Tiers[0] : null;
        }
    }
}