using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PayPeerLeaseArrears(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.WeeksInDebt < 1) return Fail(CommandError.InvalidArgument, "The lab is not behind on bills.");
            int gpus = PeerComputeSystem.LeasedInGpus(lab);
            if (gpus < 1) return Fail(CommandError.InvalidArgument, "There are no peer leases to protect.");
            long each = c.Compute.PeerLeaseArrearsCashPerGpu;
            if (each < 1) return Fail(CommandError.InvalidArgument, "Arrears payments are disabled.");
            long cost = each * gpus;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            int weeks = c.Compute.PeerLeaseShieldWeeks;
            if (weeks < 1) return Fail(CommandError.InvalidArgument, "Arrears shield weeks are not configured.");
            Finance.Spend(lab, LedgerLine.Fines, cost);
            lab.PeerLeaseShieldUntilTick = state.Tick + weeks;
            return CommandResult.Done(-1);
        }

        static CommandResult BuyPowerHedge(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            HeatwaveConfig hw = c.World?.Heatwave;
            if (hw == null || hw.HedgeCashCost < 1 || hw.HedgeWeeks < 1)
                return Fail(CommandError.InvalidArgument, "Power hedges are not offered.");
            if (lab.PowerHedgeUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "A hedge is already active.");
            if (lab.Cash < hw.HedgeCashCost) return NeedCash(lab, hw.HedgeCashCost);
            Finance.Spend(lab, LedgerLine.Power, hw.HedgeCashCost);
            lab.PowerHedgeUntilTick = state.Tick + hw.HedgeWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult BreakReservedContract(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.ReservedContracts.Count == 0)
                return Fail(CommandError.InvalidArgument, "There is no reserved contract to break.");
            int index = cmd.Count < 0 ? 0 : cmd.Count;
            if (index >= lab.ReservedContracts.Count) index = lab.ReservedContracts.Count - 1;
            ReservedContract contract = lab.ReservedContracts[index];
            long penalty = ComputeContractSystem.BreakPenalty(c, state, lab, contract);
            if (penalty > 0 && lab.Cash < penalty) return NeedCash(lab, penalty);
            if (penalty > 0) Finance.Spend(lab, LedgerLine.GpuRent, penalty);
            lab.ReservedContracts.RemoveAt(index);
            return CommandResult.Done(-1);
        }

        static CommandResult DiscloseModelAlignment(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released)
                return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            int gap = SafetySystem.AlignmentGap(c, model);
            if (gap < 1) return Fail(CommandError.InvalidArgument, "That model is not sandbagging.");
            model.DisplayedAlignment = model.Alignment;
            int gain = c.Safety.DiscloseReputationGain;
            if (gain > 0)
                lab.Reputation = (int)MathI.Min(MathI.Permille, lab.Reputation + gain);
            return CommandResult.Done(model.Id);
        }

        static CommandResult PauseChipListing(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            return ChipListingSystem.SetPaused(c, state, lab, cmd.TargetId, cmd.Count != 0);
        }

        static CommandResult AdvocateVendor(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            return VendorSystem.Advocate(c, state, lab);
        }

        static CommandResult AdoptIncidentPlan(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            SafetyConfig s = c.Safety;
            if (s.IncidentPlanWeeks < 1) return Fail(CommandError.InvalidArgument, "Incident plans are not offered.");
            if (IncidentPlanSystem.Active(lab, state.Tick))
                return Fail(CommandError.InvalidArgument, "A plan is already active.");
            if (lab.Cash < s.IncidentPlanAdoptionCash) return NeedCash(lab, s.IncidentPlanAdoptionCash);
            Finance.Spend(lab, LedgerLine.Fines, s.IncidentPlanAdoptionCash);
            lab.IncidentPlanUntilTick = state.Tick + s.IncidentPlanWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult TakeResearchLoan(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ResearchConfig r = c.Research;
            if (r.LoanPrincipal < 1) return Fail(CommandError.InvalidArgument, "Research loans are not offered.");
            if (lab.ResearchLoanBalance > 0) return Fail(CommandError.InvalidArgument, "Pay down the existing loan first.");
            Finance.Earn(lab, LedgerLine.Grants, r.LoanPrincipal);
            if (r.LoanResearchPoints > 0) lab.ResearchPoints += r.LoanResearchPoints;
            lab.ResearchLoanBalance = r.LoanPrincipal;
            return CommandResult.Done(-1);
        }

        static CommandResult GrantPeerForbearance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LabState renter = state.Lab(cmd.TargetId);
            if (renter == null || renter.Id == lab.Id) return Fail(CommandError.UnknownLab, "There is no renter lab " + cmd.TargetId + ".");
            if (PeerComputeSystem.LeaseFrom(renter, lab.Id) == null)
                return Fail(CommandError.InvalidArgument, "That lab is not renting from you.");
            long cost = c.Compute.PeerForbearanceAdoptionCash;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Grants, cost);
            int weeks = c.Compute.PeerForbearanceWeeks;
            var grant = new PeerForbearance { RenterLabId = renter.Id, UntilTick = state.Tick + weeks };
            for (int i = 0; i < lab.PeerForbearances.Count; i++)
                if (lab.PeerForbearances[i].RenterLabId == renter.Id)
                {
                    lab.PeerForbearances[i] = grant;
                    return CommandResult.Done(-1);
                }
            lab.PeerForbearances.Add(grant);
            return CommandResult.Done(-1);
        }

        static CommandResult PledgeSectorPrice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (string.IsNullOrEmpty(cmd.Id)) return Fail(CommandError.InvalidArgument, "Name the sector.");
            SectorConfig sector = c.Sector(cmd.Id);
            if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
            int price = cmd.Permille;
            if (price < c.Market.SectorPriceMinPermille || price > c.Market.SectorPriceMaxPermille)
                return Fail(CommandError.InvalidArgument, "Price must be from " + c.Market.SectorPriceMinPermille + " to " + c.Market.SectorPriceMaxPermille + " permille.");
            SectorEntry entry = state.Sector(sector.Id).Entry(lab.Id);
            if (entry == null) return Fail(CommandError.InvalidArgument, "Release into " + sector.Name + " before pledging a price.");
            if (SectorPricePledgeSystem.PledgeActive(entry, state.Tick))
                return Fail(CommandError.InvalidArgument, "A pledge is already active in that sector.");
            long cost = c.Market.SectorPricePledgeAdoptionCash;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            if (cost > 0) Finance.Spend(lab, LedgerLine.RoomUpkeep, cost);
            entry.PricePermille = price;
            entry.PricePledgePermille = price;
            entry.PricePledgeUntilTick = state.Tick + c.Market.SectorPricePledgeWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult FundLobbyDefense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            FeelingsConfig f = c.Rivalry.Feelings;
            if (f.LobbyDefenseWeeks < 1 || f.LobbyDefenseCash < 1)
                return Fail(CommandError.InvalidArgument, "Lobby defense is not offered.");
            if (lab.LobbyDefenseUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "Lobby defense is already funded.");
            if (lab.Cash < f.LobbyDefenseCash) return NeedCash(lab, f.LobbyDefenseCash);
            Finance.Spend(lab, LedgerLine.RoomUpkeep, f.LobbyDefenseCash);
            lab.LobbyDefenseUntilTick = state.Tick + f.LobbyDefenseWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult ExtendGrantRunway(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            FinanceConfig f = c.Finance;
            if (f.GrantExtensionWeeks < 1 || f.GrantExtensionCash < 1)
                return Fail(CommandError.InvalidArgument, "Grant extensions are not offered.");
            if (lab.GrantPerWeek < 1 || lab.GrantEndsTick < 0)
                return Fail(CommandError.InvalidArgument, "There is no active grant to extend.");
            if (lab.Cash < f.GrantExtensionCash) return NeedCash(lab, f.GrantExtensionCash);
            Finance.Spend(lab, LedgerLine.Grants, f.GrantExtensionCash);
            lab.GrantEndsTick = (int)MathI.Max(lab.GrantEndsTick, state.Tick + 1) + f.GrantExtensionWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult RequestDebtForbearance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            FinanceConfig f = c.Finance;
            if (f.DebtForbearanceWeeks < 1 || f.DebtForbearanceCash < 1)
                return Fail(CommandError.InvalidArgument, "Debt forbearance is not offered.");
            if (lab.DebtForbearanceUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "Debt forbearance is already in effect.");
            if (lab.Cash < f.DebtForbearanceCash) return NeedCash(lab, f.DebtForbearanceCash);
            Finance.Spend(lab, LedgerLine.Fines, f.DebtForbearanceCash);
            lab.DebtForbearanceUntilTick = state.Tick + f.DebtForbearanceWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult ReserveGreyPayout(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ShadyConfig s = c.Shady;
            if (s.GreyReserveCashCost < 1 || s.GreyReserveWeeks < 1)
                return Fail(CommandError.InvalidArgument, "Grey reserves are not offered.");
            bool grey = false;
            for (int i = 0; i < lab.ShadyOps.Count; i++)
            {
                ShadyCardConfig card = c.Shady.Card(lab.ShadyOps[i].CardId);
                if (card != null && card.Special == ShadySpecial.GreyCompute) { grey = true; break; }
            }
            if (!grey) return Fail(CommandError.InvalidArgument, "Run grey compute before reserving its payouts.");
            if (lab.GreyPayoutBoostUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "A grey reserve is already active.");
            if (lab.Cash < s.GreyReserveCashCost) return NeedCash(lab, s.GreyReserveCashCost);
            Finance.Spend(lab, LedgerLine.GreyRevenue, s.GreyReserveCashCost);
            lab.GreyPayoutBoostUntilTick = state.Tick + s.GreyReserveWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult PauseCloudRoyalties(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ChipsConfig k = c.Chips;
            if (k.CloudRoyaltyPauseCash < 1 || k.CloudRoyaltyPauseWeeks < 1)
                return Fail(CommandError.InvalidArgument, "Royalty pauses are not offered.");
            if (k.CloudLandlordSharePermille < 1)
                return Fail(CommandError.InvalidArgument, "Cloud royalties are not configured.");
            bool listed = lab.ChipListings.Count > 0;
            if (!listed)
                return Fail(CommandError.InvalidArgument, "List chips for cloud rent before pausing royalties.");
            if (lab.CloudRoyaltyPauseUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "Royalties are already paused.");
            if (lab.Cash < k.CloudRoyaltyPauseCash) return NeedCash(lab, k.CloudRoyaltyPauseCash);
            Finance.Spend(lab, LedgerLine.ChipSales, k.CloudRoyaltyPauseCash);
            lab.CloudRoyaltyPauseUntilTick = state.Tick + k.CloudRoyaltyPauseWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult FundProtestOutreach(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            AttentionConfig a = c.World.Attention;
            if (a.ProtestOutreachWeeks < 1 || a.ProtestOutreachCash < 1)
                return Fail(CommandError.InvalidArgument, "Protest outreach is not offered.");
            if (lab.ProtestMitigationUntilTick > state.Tick)
                return Fail(CommandError.InvalidArgument, "Protest outreach is already funded.");
            if (lab.Cash < a.ProtestOutreachCash) return NeedCash(lab, a.ProtestOutreachCash);
            Finance.Spend(lab, LedgerLine.RoomUpkeep, a.ProtestOutreachCash);
            lab.ProtestMitigationUntilTick = state.Tick + a.ProtestOutreachWeeks;
            return CommandResult.Done(-1);
        }
    }
}