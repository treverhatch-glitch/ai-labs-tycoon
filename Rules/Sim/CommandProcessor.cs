using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>
    /// Carries out commands. Each one is checked in full before anything changes, so a refused
    /// command leaves the game exactly as it was and says why.
    /// </summary>
    public static partial class CommandProcessor
    {
        /// <summary>The largest part of a training run that can go to alignment work. Some must be left to train.</summary>
        public const int MaxAlignmentSharePermille = 900;

        public static CommandResult Apply(GameConfig c, GameState state, Command cmd)
        {
            if (cmd == null) return Fail(CommandError.InvalidArgument, "There is no command.");
            LabState lab = state.Lab(cmd.LabId);
            if (lab == null) return Fail(CommandError.UnknownLab, "There is no lab " + cmd.LabId + ".");
            if (cmd.LabId >= 0 && lab.Bankrupt && cmd.Kind != CommandKind.ContinueAfterEnding)
                return Fail(CommandError.LabBankrupt, lab.Name + " is bankrupt.");
            if (cmd.LabId >= 0 && lab.AbsorbedByLabId >= 0 && cmd.Kind != CommandKind.ContinueAfterEnding)
                return Fail(CommandError.LabAbsorbed, lab.Name + " was absorbed into another lab.");
            if (state.PlayerLabId >= 0 && lab != null && lab.Controller == LabController.Player && cmd.LabId != state.PlayerLabId)
                return Fail(CommandError.UnknownLab, "That is not the player's lab.");
            if (cmd.Kind != CommandKind.ContinueAfterEnding && FundingSystem.PlayerCommandLocked(state, lab))
                return Fail(CommandError.BoardLocked, lab.Name + "'s board is in emergency session.");

            switch (cmd.Kind)
            {
                case CommandKind.BuildRoom: return BuildRoom(c, state, lab, cmd);
                case CommandKind.DemolishRoom: return DemolishRoom(c, state, lab, cmd);
                case CommandKind.BuyLot: return BuyLot(c, state, lab);
                case CommandKind.Hire: return Hire(c, state, lab, cmd);
                case CommandKind.Fire: return Fire(c, state, lab, cmd);
                case CommandKind.SetRentedGpus: return SetRentedGpus(state, lab, cmd);
                case CommandKind.BuyGpus: return BuyGpus(c, state, lab, cmd);
                case CommandKind.SellGpus: return SellGpus(c, state, lab, cmd);
                case CommandKind.SetTrainingShare: return SetTrainingShare(lab, cmd);
                case CommandKind.ResearchTech: return ResearchTech(c, state, lab, cmd);
                case CommandKind.StartTraining: return StartTraining(c, state, lab, cmd);
                case CommandKind.ReleaseModel: return ReleaseModel(c, state, lab, cmd);
                case CommandKind.BuyLand: return BuyLand(c, state, lab, cmd);
                case CommandKind.SellLand: return SellLand(c, state, lab, cmd);
                case CommandKind.LeaseColo: return LeaseColo(c, state, lab, cmd);
                case CommandKind.BuildHall: return BuildHall(c, state, lab, cmd);
                case CommandKind.AddPowerDeal: return AddPowerDeal(c, state, lab, cmd);
                case CommandKind.MoveHardware: return MoveHardware(c, state, lab, cmd);
                case CommandKind.StartChipDesign: return StartChipDesign(c, state, lab, cmd);
                case CommandKind.CancelChipDesign: return CancelChipDesign(lab, cmd);
                case CommandKind.TapeOut: return TapeOut(c, state, lab, cmd);
                case CommandKind.OrderChips: return OrderChips(c, state, lab, cmd);
                case CommandKind.SetChipListing: return SetChipListing(c, state, lab, cmd);
                case CommandKind.BuyListedChips: return BuyListedChips(c, state, lab, cmd);
                case CommandKind.ResearchFrontier: return ResearchFrontier(c, state, lab, cmd);
                case CommandKind.SellLab: return SellLab(c, state, lab, cmd);
                case CommandKind.ContinueAfterEnding: return ContinueAfterEnding(c, state, lab, cmd);
                case CommandKind.StartShadyOp: return StartShadyOp(c, state, lab, cmd);
                case CommandKind.Launder: return Launder(c, state, lab, cmd);
                case CommandKind.RaiseFunding: return RaiseFunding(c, state, lab, cmd);
                case CommandKind.SignReservedCompute: return SignReservedCompute(c, state, lab, cmd);
                case CommandKind.QueueGpuPurchase: return QueueGpuPurchase(c, state, lab, cmd);
                case CommandKind.AcceptChinaDeal: return AcceptChinaDeal(c, state, lab, cmd);
                case CommandKind.DeclineChinaDeal: return DeclineChinaDeal(c, state, lab, cmd);
                case CommandKind.AcquireLab: return AcquireLab(c, state, lab, cmd);
                case CommandKind.SellModel: return SellModel(c, state, lab, cmd);
                case CommandKind.CounterPoach: return CounterPoach(c, state, lab, cmd);
                case CommandKind.PoachStaff: return PoachStaff(c, state, lab, cmd);
                case CommandKind.AnswerPress: return AnswerPress(c, state, lab, cmd);
                case CommandKind.LeakStory: return LeakStory(c, state, lab, cmd);
                case CommandKind.AnswerComplaint: return AnswerComplaint(c, state, lab, cmd);
                case CommandKind.SetGauntletTuning: return SetGauntletTuning(c, state, lab, cmd);
                case CommandKind.FollowLead: return FollowLead(c, state, lab, cmd);
                case CommandKind.RushRelease: return RushRelease(c, state, lab, cmd);
                case CommandKind.Lobby: return Lobby(c, state, lab, cmd);
                case CommandKind.PivotStyle: return PivotStyle(c, state, lab, cmd);
                case CommandKind.TakeLoan: return TakeLoan(c, state, lab, cmd);
                case CommandKind.RepayLoan: return RepayLoan(state, lab, cmd);
                case CommandKind.SetResearchShare: return SetResearchShare(c, state, lab, cmd);
                case CommandKind.SetCrunch: return SetCrunch(c, state, lab, cmd);
                case CommandKind.SetInsurance: return SetInsurance(c, state, lab, cmd);
                case CommandKind.HoldOffsite: return HoldOffsite(c, state, lab);
                case CommandKind.RedTeamModel: return RedTeamModel(c, state, lab, cmd);
                case CommandKind.SetInterpretability: return SetInterpretability(c, state, lab, cmd);
                case CommandKind.SetPayLevel: return SetPayLevel(c, state, lab, cmd);
                case CommandKind.SetCompliance: return SetCompliance(c, state, lab, cmd);
                case CommandKind.PublishPaper: return PublishPaper(c, state, lab);
                case CommandKind.SetSublet: return SetSublet(c, state, lab, cmd);
                case CommandKind.CommissionAudit: return CommissionAudit(c, state, lab, cmd);
                case CommandKind.RetrainStaff: return RetrainStaff(c, state, lab, cmd);
                case CommandKind.RunCampaign: return RunCampaign(c, state, lab, cmd);
                case CommandKind.ScrapRun: return ScrapRun(c, state, lab, cmd);
                case CommandKind.Layoff: return Layoff(c, state, lab, cmd);
                case CommandKind.PayDownDebt: return PayDownDebt(c, state, lab, cmd);
                case CommandKind.PromoteStaff: return PromoteStaff(c, state, lab, cmd);
                case CommandKind.SetRevenueTarget: return SetRevenueTarget(c, state, lab, cmd);
                case CommandKind.EndowChair: return EndowChair(c, state, lab, cmd);
                case CommandKind.AnswerWhistle: return AnswerWhistle(c, state, lab, cmd);
                case CommandKind.SetBugBounty: return SetBugBounty(c, state, lab, cmd);
                case CommandKind.PullOutOfSector: return PullOutOfSector(c, state, lab, cmd);
                case CommandKind.RestructureLoan: return RestructureLoan(c, state, lab, cmd);
                case CommandKind.AnnounceRoadmap: return AnnounceRoadmap(c, state, lab, cmd);
                case CommandKind.AssignMentor: return AssignMentor(c, state, lab, cmd);
                case CommandKind.EndMentoring: return EndMentoring(c, state, lab, cmd);
                case CommandKind.StartSkunkworks: return StartSkunkworks(c, state, lab, cmd);
                case CommandKind.WrapUpSkunkworks: return WrapUpSkunkworks(c, state, lab, cmd);
                case CommandKind.SetPledge: return SetPledge(c, state, lab, cmd);
                case CommandKind.SetAsideReserve: return SetAsideReserve(c, state, lab, cmd);
                case CommandKind.DrawReserve: return DrawReserve(c, state, lab, cmd);
                case CommandKind.ApplyForGrant: return ApplyForGrant(c, state, lab, cmd);
                case CommandKind.SetSla: return SetSla(c, state, lab, cmd);
                case CommandKind.PublishPostMortem: return PublishPostMortem(c, state, lab, cmd);
                case CommandKind.HireConsultants: return HireConsultants(c, state, lab, cmd);
                case CommandKind.SetOnCall: return SetOnCall(c, state, lab, cmd);
                case CommandKind.SetOptimisation: return SetOptimisation(c, state, lab, cmd);
                case CommandKind.SetEthicsBoard: return SetEthicsBoard(c, state, lab, cmd);
                case CommandKind.AppointChief: return AppointChief(c, state, lab, cmd);
                case CommandKind.GrantStaffEquity: return GrantStaffEquity(c, state, lab);
                case CommandKind.HoldHackWeek: return HoldHackWeek(c, state, lab);
                case CommandKind.SetResearchFocus: return SetResearchFocus(c, state, lab, cmd);
                case CommandKind.SetRunApproach: return SetRunApproach(c, state, lab, cmd);
                case CommandKind.SetCheckpoints: return SetCheckpoints(c, state, lab, cmd);
                case CommandKind.DesignWithModel: return DesignWithModel(c, state, lab, cmd);
                case CommandKind.SetTrainingObjective: return SetTrainingObjective(c, state, lab, cmd);
                case CommandKind.AnswerOfficeEvent: return AnswerOfficeEvent(c, state, lab, cmd);
                case CommandKind.FineTuneModel: return FineTuneModel(c, state, lab, cmd);
                case CommandKind.RollBackModel: return RollBackModel(c, state, lab, cmd);
                case CommandKind.SetReleaseGate: return SetReleaseGate(c, state, lab, cmd);
                case CommandKind.CleanUpNaming: return CleanUpNaming(c, state, lab, cmd);
                case CommandKind.RefurbishRoom: return RefurbishRoom(c, state, lab, cmd);
                default: return Fail(CommandError.InvalidArgument, "Unknown kind of command.");
            }
        }

        static CommandResult Fail(CommandError error, string message) { return CommandResult.Fail(error, message); }

        static CommandResult NeedCash(LabState lab, long cost)
        {
            return Fail(CommandError.NotEnoughCash, "That costs $" + cost + " and the lab has $" + lab.Cash + ".");
        }

        static CommandResult BuildRoom(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomTypeConfig type = c.RoomType(cmd.Id);
            if (type == null) return Fail(CommandError.UnknownId, "There is no room type \"" + cmd.Id + "\".");
            if (!EraSystem.RoomOpen(c, type, state.Tick))
                return Fail(CommandError.EraLocked, type.Name + " is not around yet: it arrives with " + c.Era(type.Era).Name + " (" + c.Era(type.Era).StartYear + ").");
            if (!LabQueries.RoomUnlocked(lab, type))
                return Fail(CommandError.MissingPrerequisite, type.Name + " needs " + c.Tech(type.RequiresTech).Name + " first.");
            if (cmd.Width < type.MinWidth || cmd.Height < type.MinHeight)
                return Fail(CommandError.TooSmall, "A " + type.Name + " must be at least " + type.MinWidth + " by " + type.MinHeight + " tiles.");
            LotConfig lot = c.Lot(lab.LotId);
            if (!LotGrid.InBounds(lot, cmd.X, cmd.Y, cmd.Width, cmd.Height))
                return Fail(CommandError.OutOfBounds, "That does not fit on the lot (" + lot.Width + " by " + lot.Height + " tiles).");
            if (!LotGrid.IsFree(lab.Rooms, cmd.X, cmd.Y, cmd.Width, cmd.Height))
                return Fail(CommandError.Overlap, "Another room is in the way.");
            long cost = type.BuildCostPerTile * cmd.Width * cmd.Height;
            if (lab.Cash < cost) return NeedCash(lab, cost);

            var room = new RoomState();
            room.Id = state.TakeId();
            room.TypeId = type.Id;
            room.X = cmd.X;
            room.Y = cmd.Y;
            room.Width = cmd.Width;
            room.Height = cmd.Height;
            lab.Rooms.Add(room);
            Finance.Spend(lab, LedgerLine.Construction, cost);
            Events.Add(state, EventKind.RoomBuilt, lab.Id, room.Id, cost, type.Id);
            return CommandResult.Done(room.Id);
        }

        static CommandResult DemolishRoom(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomState room = lab.Room(cmd.TargetId);
            if (room == null) return Fail(CommandError.UnknownId, "There is no room " + cmd.TargetId + ".");
            RoomTypeConfig type = c.RoomType(room.TypeId);

            if (!string.IsNullOrEmpty(type.SeatsRole))
            {
                int seatsLeft = LabQueries.Seats(c, lab, type.SeatsRole) - LabQueries.SeatsInRoom(c, room);
                if (LabQueries.StaffCount(lab, type.SeatsRole) > seatsLeft)
                    return Fail(CommandError.StaffNeedSeats, "People still work in that room. Let them go or build them seats elsewhere first.");
            }
            int slotsLeft = LabQueries.GpuSlots(c, lab) - LabQueries.GpuSlotsInRoom(c, room);
            if (LabQueries.HqGpus(lab) > slotsLeft)
                return Fail(CommandError.GpusNeedSlots, "The lab's own GPUs are racked in that room. Sell them or build racks elsewhere first.");

            lab.Rooms.Remove(room);
            Events.Add(state, EventKind.RoomDemolished, lab.Id, room.Id, 0, type.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult BuyLot(GameConfig c, GameState state, LabState lab)
        {
            int at = c.Lots.IndexOf(c.Lot(lab.LotId));
            if (at + 1 >= c.Lots.Count) return Fail(CommandError.NoBiggerLot, "The lab already has the biggest lot.");
            LotConfig next = c.Lots[at + 1];
            if (lab.Cash < next.Price) return NeedCash(lab, next.Price);

            lab.LotId = next.Id;
            Finance.Spend(lab, LedgerLine.Construction, next.Price);
            Events.Add(state, EventKind.LotBought, lab.Id, -1, next.Price, next.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult Hire(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoleConfig role = c.Role(cmd.Id);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Hire at least one person.");
            int free = LabQueries.Seats(c, lab, role.Id) - LabQueries.StaffCount(lab, role.Id);
            if (cmd.Count > free)
                return Fail(CommandError.NoSeats, "There are " + free + " free seats for a " + role.Name + ". Build a room for them first.");
            long cost = RolePerks.HireCost(c, lab, role, cmd.Count, state.Tick);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            StaffSystem.Hire(c, state, lab, role, cmd.Count);
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            Events.Add(state, EventKind.Hired, lab.Id, -1, cmd.Count, role.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult Fire(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoleConfig role = c.Role(cmd.Id);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Let go of at least one person.");
            if (cmd.Count > LabQueries.StaffCount(lab, role.Id))
                return Fail(CommandError.NotEnoughStaff, "The lab does not have that many people in that role.");

            StaffSystem.Fire(lab, role.Id, cmd.Count);
            ControlStale.OnFired(c, lab, role.Id, cmd.Count);
            TalentSystem.DropGoneOffers(lab);
            Finance.Spend(lab, LedgerLine.Hiring, role.Severance * cmd.Count);
            Events.Add(state, EventKind.Fired, lab.Id, -1, cmd.Count, role.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult SetRentedGpus(GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 0) return Fail(CommandError.InvalidArgument, "A lab cannot rent fewer than zero GPUs.");
            if (cmd.Count != lab.RentedGpus)
            {
                lab.RentedGpus = cmd.Count;
                Events.Add(state, EventKind.GpusRented, lab.Id, -1, cmd.Count, "");
            }
            return CommandResult.Done(-1);
        }

        static CommandResult BuyGpus(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Buy at least one GPU.");
            int holdingId = cmd.TargetId;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            CommandResult place = CheckPlaceForHardware(c, state, lab, holdingId, gen, cmd.Count);
            if (!place.Ok) return place;
            int allocation = VendorSystem.AllocationLeft(state, lab);
            if (cmd.Count > allocation)
                return Fail(CommandError.AllocationExhausted, c.Compute.VendorName + " has " + allocation + " GPUs left to sell " + lab.Name + " this month.");
            long cost = VendorSystem.PricePerGpu(c, state, lab, gen) * cmd.Count;
            if (lab.Cash < cost) return NeedCash(lab, cost);

            Fleet.AddGpus(state, lab, holdingId, gen.Id, cmd.Count);
            Finance.Spend(lab, LedgerLine.Hardware, cost);
            VendorSystem.OnBought(c, state, lab, cmd.Count);
            Events.Add(state, EventKind.GpusBought, lab.Id, holdingId, cmd.Count, gen.Id);
            return CommandResult.Done(-1);
        }

        /// <summary>
        /// Everything that must hold before hardware of a generation can go to a place: the place exists, its government has not
        /// frozen the lab out, no export rule stops the tier, and there is room (rack space at the HQ, free kilowatts at a site).
        /// </summary>
        static CommandResult CheckPlaceForHardware(GameConfig c, GameState state, LabState lab, int holdingId, GpuGenerationConfig gen, int count)
        {
            Holding holding = null;
            if (holdingId != HardwareBatch.Hq)
            {
                holding = lab.Holding(holdingId);
                if (holding == null) return Fail(CommandError.UnknownId, "The lab has no holding " + holdingId + ".");
            }
            string blocId = WorldQueries.BlocOfPlace(c, lab, holdingId);
            BlocConfig bloc = c.World.Bloc(blocId);
            if (WorldQueries.IsFrozen(lab, blocId))
                return Fail(CommandError.PermitFrozen, bloc.Agency + " has frozen " + lab.Name + "'s permits in " + bloc.Name + ".");
            if (!LabQueries.ExportAllowed(c, state, lab, gen.Tier, blocId))
                return Fail(CommandError.ExportBlocked, "Export rules keep " + gen.Name + " out of " + bloc.Name + " for a lab based in " + c.World.Bloc(lab.HomeBlocId).Name + ".");

            if (holding == null)
            {
                int free = LabQueries.GpuSlots(c, lab) - LabQueries.HqGpus(lab);
                if (count > free)
                    return Fail(CommandError.NoGpuSlots, "There is rack space for " + free + " more GPUs. Build a server room first.");
            }
            else
            {
                int freeKw = WorldQueries.FreeKw(c, state, lab, holding);
                if (count * gen.KwPerGpu > freeKw)
                    return Fail(CommandError.NotEnoughPower, "That needs " + (count * gen.KwPerGpu) + " kW and the hall has " + (freeKw < 0 ? 0 : freeKw) + " kW free.");
            }
            return CommandResult.Done(-1);
        }

        static CommandResult SellGpus(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Sell at least one GPU.");
            int holdingId = cmd.TargetId;
            if (holdingId != HardwareBatch.Hq && lab.Holding(holdingId) == null) return Fail(CommandError.UnknownId, "The lab has no holding " + holdingId + ".");
            int owned = LabQueries.OwnedGpuUnits(lab, holdingId);
            if (cmd.Count > owned) return Fail(CommandError.NotEnoughGpus, "The lab owns " + owned + " GPUs there.");

            long proceeds = 0;
            int left = cmd.Count;
            foreach (HardwareBatch batch in Fleet.OldestFirst(c, lab, holdingId))
            {
                if (left == 0) break;
                if (batch.ResaleLockedUntilTick >= 0 && state.Tick < batch.ResaleLockedUntilTick)
                    return Fail(CommandError.TargetNotEligible, "These GPUs are still tied up in post-acquisition integration.");
                int take = left < batch.Count ? left : batch.Count;
                proceeds += take * LabQueries.ResalePerUnit(c, c.Compute.Generation(batch.GpuGenId), state.Tick);
                Fleet.Take(lab, batch, take);
                left -= take;
            }
            Finance.Earn(lab, LedgerLine.Hardware, proceeds);
            Events.Add(state, EventKind.GpusSold, lab.Id, holdingId, cmd.Count, "");
            return CommandResult.Done(-1);
        }

        static CommandResult SetResearchShare(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sets compute aside for research.");
            if (cmd.Permille < 0 || cmd.Permille > c.Research.MaxComputeSharePermille)
                return Fail(CommandError.InvalidArgument, "The research share is from 0 to " + c.Research.MaxComputeSharePermille + ".");
            lab.ResearchSharePermille = cmd.Permille;
            return CommandResult.Done(-1);
        }

        static CommandResult SetCrunch(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can ask its people to work late.");
            if (cmd.Count != 0 && cmd.Count != 1) return Fail(CommandError.InvalidArgument, "Crunch time is on (1) or off (0).");
            if (cmd.Count == 0)
            {
                if (!Crunch.Active(lab)) return Fail(CommandError.InvalidArgument, "The team is not in crunch time.");
                Crunch.End(state, lab, "stopped", lab.CrunchWeeks - 1);
                return CommandResult.Done(-1);
            }
            if (Crunch.Active(lab)) return Fail(CommandError.InvalidArgument, "The team is already in crunch time.");
            if (c.StaffPeople.CrunchMaxWeeks <= 0) return Fail(CommandError.InvalidArgument, "There is no crunch time in this game.");
            if (HackWeek.Active(lab)) return Fail(CommandError.InvalidArgument, "The team is on a hack week; crunch time can start when it is over.");
            if (lab.Staff.Count == 0) return Fail(CommandError.NotEnoughStaff, "There is no one to work late.");
            if (Crunch.AverageMorale(lab) < c.StaffPeople.CrunchMinMorale)
                return Fail(CommandError.TeamTooTired, "The team is too tired for crunch time (morale " + Crunch.AverageMorale(lab) + ", needs " + c.StaffPeople.CrunchMinMorale + ").");
            lab.CrunchWeeks = 1;
            return CommandResult.Done(-1);
        }

        static CommandResult HoldOffsite(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab takes its team on an offsite.");
            if (c.StaffPeople.OffsiteMoraleLift <= 0) return Fail(CommandError.InvalidArgument, "There are no offsites in this game.");
            if (lab.Staff.Count == 0) return Fail(CommandError.NotEnoughStaff, "There is no one to take away.");
            if (Offsite.Lifted(c, lab) == 0) return Fail(CommandError.InvalidArgument, "No one has room left to rise: everyone is as high as an offsite can take them.");
            long cost = Offsite.Cost(c, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Offsite.Hold(c, state, lab, cost);
            return CommandResult.Done(-1);
        }

        static CommandResult SetInsurance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab buys incident insurance.");
            FinanceConfig f = c.Finance;
            if (cmd.Count < 0 || cmd.Count > f.InsuranceMaxLevel) return Fail(CommandError.InvalidArgument, "Insurance is from level 0 to " + f.InsuranceMaxLevel + ".");
            if (cmd.Count == lab.InsuranceLevel) return Fail(CommandError.InvalidArgument, "The lab already has that cover.");
            if (state.Tick < lab.InsuranceLockedUntil) return Fail(CommandError.InsuranceLocked, "The policy was changed recently and cannot be changed before week " + lab.InsuranceLockedUntil + ".");
            if (cmd.Count > lab.InsuranceLevel && lab.InsuranceClaims >= f.InsuranceDropClaims)
                return Fail(CommandError.Uninsurable, "No insurer will sell cover to a lab with " + lab.InsuranceClaims + " recent claims.");
            lab.InsuranceLevel = cmd.Count;
            lab.InsuranceLockedUntil = state.Tick + f.InsuranceLockWeeks;
            return CommandResult.Done(-1);
        }

        static CommandResult SetTrainingShare(LabState lab, Command cmd)
        {
            if (cmd.Permille < 0 || cmd.Permille > MathI.Permille)
                return Fail(CommandError.InvalidArgument, "The training share is from 0 to 1000.");
            lab.TrainingSharePermille = cmd.Permille;
            return CommandResult.Done(-1);
        }

        static CommandResult ResearchTech(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            if (LabQueries.HasTech(lab, tech.Id)) return Fail(CommandError.AlreadyResearched, tech.Name + " is already researched.");
            if (!EraSystem.TechOpen(c, tech, state.Tick))
                return Fail(CommandError.EraLocked, tech.Name + " is not invented yet: it arrives with " + c.Era(tech.Era).Name + " (" + c.Era(tech.Era).StartYear + ").");
            for (int i = 0; i < tech.Requires.Count; i++)
                if (!LabQueries.HasTech(lab, tech.Requires[i]))
                    return Fail(CommandError.MissingPrerequisite, tech.Name + " needs " + c.Tech(tech.Requires[i]).Name + " first.");
            long cost = LabQueries.TechCost(c, state, lab, tech);
            if (lab.ResearchPoints < cost)
                return Fail(CommandError.NotEnoughResearch, tech.Name + " costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");

            int lossBefore = ControlStale.LossNow(c, lab, state.Tick);
            lab.ResearchPoints -= cost;
            lab.Techs.Add(tech.Id);
            Events.Add(state, EventKind.TechResearched, lab.Id, -1, cost, tech.Id);
            ControlStale.OnResearched(c, state, lab, lossBefore, tech);
            return CommandResult.Done(-1);
        }

        static CommandResult StartTraining(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelLineConfig line = c.Line(cmd.Id);
            if (line == null) return Fail(CommandError.UnknownId, "There is no model line \"" + cmd.Id + "\".");
            if (!LabQueries.LineUnlocked(c, lab, line.Id)) return Fail(CommandError.LineLocked, "The lab has not researched how to build a " + line.Name + " yet.");
            if (cmd.Permille < 0 || cmd.Permille > MaxAlignmentSharePermille)
                return Fail(CommandError.InvalidArgument, "The alignment share is from 0 to " + MaxAlignmentSharePermille + ": some of the run has to train the model.");
            if (LabQueries.ActiveRuns(lab) >= c.Training.MaxConcurrentRuns)
                return Fail(CommandError.TooManyRuns, "The lab can train " + c.Training.MaxConcurrentRuns + " models at once.");
            long prepCost = EraSystem.RunPrepCost(c, line, state.Tick);
            if (lab.Cash < prepCost) return NeedCash(lab, prepCost);

            var model = new ModelState();
            model.Id = state.TakeId();
            model.LineId = line.Id;
            model.Version = LabQueries.NextVersion(lab, line.Id);
            model.Name = VersionNaming.NameFor(c, lab, line, model.Version);
            VersionNaming.OnVersionNamed(c, lab);
            model.Status = ModelStatus.Training;
            model.RequiredCompute = EraSystem.RunCompute(c, line, state.Tick);
            model.AlignmentSharePermille = cmd.Permille;
            model.StartedTick = state.Tick;
            model.TrainedTick = -1;
            model.ReleasedTick = -1;
            model.LastIncidentTick = -1;
            lab.Models.Add(model);
            Finance.Spend(lab, LedgerLine.Training, prepCost);
            Events.Add(state, EventKind.TrainingStarted, lab.Id, model.Id, prepCost, model.Name);
            return CommandResult.Done(model.Id);
        }

        static CommandResult ReleaseModel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released)
                return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a trained model can go on sale.");
            if (cmd.Ids == null || cmd.Ids.Count == 0) return Fail(CommandError.InvalidArgument, "Name at least one sector to sell into.");
            if (SafetyCulture.GateBlocks(lab, model))
                return Fail(CommandError.BelowReleaseGate, model.Name + " shows alignment " + SafetySystem.DisplayedAlignment(model) + " and your own release gate asks for " + lab.ReleaseGate + ".");
            if (model.Status == ModelStatus.Trained && EthicsBoard.Vetoes(c, lab, model))
                return Fail(CommandError.BoardVeto, "The ethics board will not let " + model.Name + " go on sale: its true alignment is " + model.Alignment + " and the board wants " + c.Board.VetoAlignment + ".");
            if (!EraRules.MeetsReleaseBar(c, model, state.Tick))
                return Fail(CommandError.BelowEraSafetyBar, model.Name + " shows alignment " + SafetySystem.DisplayedAlignment(model) + " and the law in " + EraSystem.AtTick(c, state.Tick).Name + " asks for " + EraRules.ReleaseBar(c, state.Tick) + " before a model goes on sale.");

            for (int i = 0; i < cmd.Ids.Count; i++)
            {
                SectorConfig sector = c.Sector(cmd.Ids[i]);
                if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Ids[i] + "\".");
                if (cmd.Ids.IndexOf(cmd.Ids[i]) != i) return Fail(CommandError.InvalidArgument, sector.Name + " is named twice.");
                if (!LabQueries.SectorOpen(c, sector, state.Tick))
                    return Fail(CommandError.SectorClosed, sector.Name + " opens in " + sector.OpensYear + ".");
                if (model.Capability < sector.MinCapability)
                    return Fail(CommandError.BelowSectorBar, sector.Name + " needs capability " + sector.MinCapability + " and " + model.Name + " has " + model.Capability + ".");
            }

            var replaced = new List<ModelState>();
            for (int i = 0; i < cmd.Ids.Count; i++)
            {
                SectorState sectorState = state.Sector(cmd.Ids[i]);
                SectorEntry entry = sectorState.Entry(lab.Id);
                if (entry == null)
                {
                    entry = new SectorEntry();
                    entry.LabId = lab.Id;
                    entry.ModelId = model.Id;
                    sectorState.Entries.Add(entry);
                }
                else if (entry.ModelId != model.Id)
                {
                    // An upgrade: customers stay with the lab, so the share carries over to the new model.
                    ModelState old = lab.Model(entry.ModelId);
                    old.Sectors.Remove(sectorState.Id);
                    if (!replaced.Contains(old)) replaced.Add(old);
                    entry.ModelId = model.Id;
                }
                if (!model.Sectors.Contains(sectorState.Id)) model.Sectors.Add(sectorState.Id);
            }

            bool firstRelease = model.Status != ModelStatus.Released;
            if (model.Status != ModelStatus.Released)
            {
                model.Status = ModelStatus.Released;
                model.ReleasedTick = state.Tick;
                PressSystem.OnLaunch(c, state, lab, model);
                SafetyCulture.OnFirstRelease(c, lab, model);
                if (cmd.Permille > 0)
                    model.DisplayedAlignment = (int)MathI.Min(MathI.Permille, MathI.Max(model.Alignment, cmd.Permille));
                else if (model.DisplayedAlignment < model.Alignment)
                    model.DisplayedAlignment = model.Alignment;
                Pledge.OnFirstRelease(c, state, lab, model);
            }
            Events.Add(state, EventKind.ModelReleased, lab.Id, model.Id, cmd.Ids.Count, model.Name);
            if (firstRelease) OfficeEvents.OnRelease(c, state, lab);

            for (int i = 0; i < replaced.Count; i++)
            {
                if (replaced[i].Sectors.Count > 0) continue;
                replaced[i].Status = ModelStatus.Retired;
                Events.Add(state, EventKind.ModelRetired, lab.Id, replaced[i].Id, 0, replaced[i].Name);
            }
            return CommandResult.Done(-1);
        }
    }
}
