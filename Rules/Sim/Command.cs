using System.Collections.Generic;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Sim
{
    public enum CommandKind
    {
        BuildRoom, DemolishRoom, BuyLot,
        Hire, Fire,
        SetRentedGpus, BuyGpus, SellGpus, SetTrainingShare,
        ResearchTech, StartTraining, ReleaseModel,
        BuyLand, SellLand, LeaseColo, BuildHall, AddPowerDeal, MoveHardware,
        StartChipDesign, CancelChipDesign, TapeOut, OrderChips, SetChipListing, BuyListedChips,
        ResearchFrontier, SellLab, ContinueAfterEnding, StartShadyOp, Launder, RaiseFunding,
        SignReservedCompute, QueueGpuPurchase,
        AcceptChinaDeal, DeclineChinaDeal,
        AcquireLab, SellModel,
        CounterPoach, PoachStaff,
        AnswerPress, LeakStory,
        SetGauntletTuning,
        AnswerComplaint,
        SetReleaseGate,
        CleanUpNaming,
        RefurbishRoom,
        FollowLead,
        FineTuneModel,
        RollBackModel,
        AnswerOfficeEvent,
        SetTrainingObjective,
        DesignWithModel,
        RushRelease,
        Lobby,
        PivotStyle,
        SetRunApproach,
        SetCheckpoints,
        TakeLoan,
        RepayLoan,
        SetResearchShare,
        SetCrunch,
        SetInsurance,
        HoldOffsite,
        RedTeamModel,
        SetInterpretability,
        SetPayLevel,
        SetCompliance,
        PublishPaper,
        SetSublet,
        CommissionAudit,
        SetResearchFocus,
        HoldHackWeek,
        GrantStaffEquity,
        RetrainStaff,
        AppointChief,
        SetEthicsBoard,
        SetOptimisation,
        RunCampaign,
        SetOnCall,
        HireConsultants,
        PublishPostMortem,
        SetSla,
        ApplyForGrant
    }

    /// <summary>
    /// One order from a lab's boss. The player's screens and the rivals' plans both make these, and
    /// they are the only way anything outside the rules changes the game. A command is plain data,
    /// so a recorded list of them replays a whole game.
    /// </summary>
    public sealed class Command : ISync
    {
        public CommandKind Kind;
        public int LabId;
        /// <summary>A config id: the room type, role, tech, model line, site or power deal the order is about.</summary>
        public string Id = "";
        /// <summary>A room, model, holding or hardware batch already in the game, or -1.</summary>
        public int TargetId = -1;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Count;
        public int Permille;
        /// <summary>Sector ids, for a release.</summary>
        public List<string> Ids = new List<string>();

        public void Sync(Syncer s)
        {
            s.Enum("kind", ref Kind);
            s.Int("labId", ref LabId);
            s.Str("id", ref Id);
            s.Int("targetId", ref TargetId);
            s.Int("x", ref X);
            s.Int("y", ref Y);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
            s.Int("count", ref Count);
            s.Int("permille", ref Permille);
            s.StrList("ids", ref Ids);
        }

        static Command Make(CommandKind kind, int labId)
        {
            var c = new Command();
            c.Kind = kind;
            c.LabId = labId;
            return c;
        }

        public static Command BuildRoom(int labId, string roomTypeId, int x, int y, int width, int height)
        {
            Command c = Make(CommandKind.BuildRoom, labId);
            c.Id = roomTypeId;
            c.X = x;
            c.Y = y;
            c.Width = width;
            c.Height = height;
            return c;
        }

        public static Command DemolishRoom(int labId, int roomId)
        {
            Command c = Make(CommandKind.DemolishRoom, labId);
            c.TargetId = roomId;
            return c;
        }

        /// <summary>Buys the next bigger lot. Rooms keep their place.</summary>
        public static Command BuyLot(int labId) { return Make(CommandKind.BuyLot, labId); }

        public static Command Hire(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.Hire, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        public static Command Fire(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.Fire, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        /// <summary>Sets how many GPUs the lab rents, up or down. Rent is paid weekly; there is nothing to pay now.</summary>
        public static Command SetRentedGpus(int labId, int count)
        {
            Command c = Make(CommandKind.SetRentedGpus, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Buys GPUs of the newest generation into the HQ's racks.</summary>
        public static Command BuyGpus(int labId, int count) { return BuyGpus(labId, count, HardwareBatch.Hq); }

        /// <summary>Buys GPUs of the newest generation into a holding at a site, or into the HQ when the holding id is -1.</summary>
        public static Command BuyGpus(int labId, int count, int holdingId)
        {
            Command c = Make(CommandKind.BuyGpus, labId);
            c.Count = count;
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Sells GPUs from the HQ, oldest generation first.</summary>
        public static Command SellGpus(int labId, int count) { return SellGpus(labId, count, HardwareBatch.Hq); }

        public static Command SellGpus(int labId, int count, int holdingId)
        {
            Command c = Make(CommandKind.SellGpus, labId);
            c.Count = count;
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Sets the share of compute that trains models while a run is active. The rest serves customers.</summary>
        public static Command SetTrainingShare(int labId, int permille)
        {
            Command c = Make(CommandKind.SetTrainingShare, labId);
            c.Permille = permille;
            return c;
        }

        public static Command ResearchTech(int labId, string techId)
        {
            Command c = Make(CommandKind.ResearchTech, labId);
            c.Id = techId;
            return c;
        }

        /// <summary>Starts training the next version of a line. The alignment share is the part of the run spent on safety work.</summary>
        public static Command StartTraining(int labId, string lineId, int alignmentSharePermille)
        {
            Command c = Make(CommandKind.StartTraining, labId);
            c.Id = lineId;
            c.Permille = alignmentSharePermille;
            return c;
        }

        /// <summary>Buys a plot of land at a site. The hall comes after.</summary>
        public static Command BuyLand(int labId, string siteId)
        {
            Command c = Make(CommandKind.BuyLand, labId);
            c.Id = siteId;
            return c;
        }

        /// <summary>Sells the land of a holding with no hardware on it. Part of the price comes back; the hall does not.</summary>
        public static Command SellLand(int labId, int holdingId)
        {
            Command c = Make(CommandKind.SellLand, labId);
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Leases (or changes, or with 0 ends the lease of) space at a site: kilowatts in someone else's hall.</summary>
        public static Command LeaseColo(int labId, string siteId, int kw)
        {
            Command c = Make(CommandKind.LeaseColo, labId);
            c.Id = siteId;
            c.Count = kw;
            return c;
        }

        /// <summary>Starts building a bigger hall on owned land: this many more kilowatts.</summary>
        public static Command BuildHall(int labId, int holdingId, int kw)
        {
            Command c = Make(CommandKind.BuildHall, labId);
            c.TargetId = holdingId;
            c.Count = kw;
            return c;
        }

        /// <summary>Buys private power for a holding.</summary>
        public static Command AddPowerDeal(int labId, int holdingId, string dealId)
        {
            Command c = Make(CommandKind.AddPowerDeal, labId);
            c.TargetId = holdingId;
            c.Id = dealId;
            return c;
        }

        /// <summary>Moves units of a hardware batch to another place: a holding's id, or -1 for the HQ.</summary>
        public static Command MoveHardware(int labId, int batchId, int toHoldingId, int count)
        {
            Command c = Make(CommandKind.MoveHardware, labId);
            c.TargetId = batchId;
            c.X = toHoldingId;
            c.Count = count;
            return c;
        }

        /// <summary>Starts designing a chip: its target, then the node, die, number format, memory and cooling.</summary>
        public static Command StartChipDesign(int labId, string target, string node, string die, string format, string memory, string cooling)
        {
            Command c = Make(CommandKind.StartChipDesign, labId);
            c.Id = target;
            c.Ids.AddRange(new[] { node, die, format, memory, cooling });
            return c;
        }

        public static Command CancelChipDesign(int labId, int designId)
        {
            Command c = Make(CommandKind.CancelChipDesign, labId);
            c.TargetId = designId;
            return c;
        }

        /// <summary>Sends a finished design to the foundry.</summary>
        public static Command TapeOut(int labId, int designId)
        {
            Command c = Make(CommandKind.TapeOut, labId);
            c.TargetId = designId;
            return c;
        }

        /// <summary>Books chips of one of the lab's own working designs at the foundry, to arrive in a holding.</summary>
        public static Command OrderChips(int labId, int designId, int holdingId, int count)
        {
            Command c = Make(CommandKind.OrderChips, labId);
            c.TargetId = designId;
            c.X = holdingId;
            c.Count = count;
            return c;
        }

        /// <summary>Offers chips of a working design to other labs at a price per chip, up to a number a month. A price of 0 takes the offer down.</summary>
        public static Command SetChipListing(int labId, int designId, int pricePerChip, int maxPerMonth)
        {
            Command c = Make(CommandKind.SetChipListing, labId);
            c.TargetId = designId;
            c.Count = pricePerChip;
            c.X = maxPerMonth;
            return c;
        }

        /// <summary>Buys chips another lab has listed; they arrive in the buyer's holding.</summary>
        public static Command BuyListedChips(int labId, int sellerLabId, int designId, int holdingId, int count)
        {
            Command c = Make(CommandKind.BuyListedChips, labId);
            c.TargetId = sellerLabId;
            c.Y = designId;
            c.X = holdingId;
            c.Count = count;
            return c;
        }

        /// <summary>Puts a trained model on sale in the named sectors, replacing the lab's older model there.</summary>
        public static Command ReleaseModel(int labId, int modelId, params string[] sectorIds)
        {
            return ReleaseModel(labId, modelId, 0, sectorIds);
        }

        /// <summary><paramref name="claimedAlignmentPermille"/> is the safety reading customers are sold (sandbagging when above the true meter).</summary>
        public static Command ReleaseModel(int labId, int modelId, int claimedAlignmentPermille, params string[] sectorIds)
        {
            Command c = Make(CommandKind.ReleaseModel, labId);
            c.TargetId = modelId;
            c.Permille = claimedAlignmentPermille;
            c.Ids.AddRange(sectorIds);
            return c;
        }

        /// <summary>A first release with <paramref name="weeksSkipped"/> weeks of testing skipped: it sells better for a quarter and is likelier to have an incident for half a year.</summary>
        public static Command RushRelease(int labId, int modelId, int weeksSkipped, params string[] sectorIds)
        {
            Command c = ReleaseModel(labId, modelId, 0, sectorIds);
            c.Kind = CommandKind.RushRelease;
            c.Count = weeksSkipped;
            return c;
        }

        /// <summary>Pays for a push on one government (<paramref name="goal"/> is "quiet" or "subsidy"). It can land, miss or backfire as a scandal.</summary>
        public static Command Lobby(int labId, string blocId, string goal)
        {
            Command c = Make(CommandKind.Lobby, labId);
            c.Id = blocId;
            c.Ids.Add(goal);
            return c;
        }

        /// <summary>Sets aside this share of general compute for research experiments, in permille.</summary>
        public static Command SetResearchShare(int labId, int permille)
        {
            Command c = Make(CommandKind.SetResearchShare, labId);
            c.Permille = permille;
            return c;
        }

        /// <summary>Takes out, changes or cancels incident insurance: the level, from 0 (none) to the data's highest.</summary>
        public static Command SetInsurance(int labId, int level)
        {
            Command c = Make(CommandKind.SetInsurance, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the pay policy: 0 is market pay, above 0 pays better, below 0 pays worse, within the data's limits.</summary>
        public static Command SetPayLevel(int labId, int level)
        {
            Command c = Make(CommandKind.SetPayLevel, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the level of the interpretability programme, from 0 (off) to the data's highest.</summary>
        public static Command SetInterpretability(int labId, int level)
        {
            Command c = Make(CommandKind.SetInterpretability, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the level of the compliance office, from 0 (off) to the data's highest. Each level needs policy staff.</summary>
        public static Command SetCompliance(int labId, int level)
        {
            Command c = Make(CommandKind.SetCompliance, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Spends research points to publish a paper, which earns reputation and academic prestige.</summary>
        public static Command PublishPaper(int labId)
        {
            return Make(CommandKind.PublishPaper, labId);
        }

        /// <summary>Lets this share of the lab's general compute (permille) to outside customers, from 0 to the data's most.</summary>
        public static Command SetSublet(int labId, int permille)
        {
            Command c = Make(CommandKind.SetSublet, labId);
            c.Permille = permille;
            return c;
        }

        /// <summary>Moves one person to another role for a share of that role's hire fee and some of their morale. The new role needs a free seat.</summary>
        public static Command RetrainStaff(int labId, int personId, string roleId)
        {
            Command c = Make(CommandKind.RetrainStaff, labId);
            c.TargetId = personId;
            c.Id = roleId;
            return c;
        }

        /// <summary>Runs a marketing campaign in a sector the lab sells in, at a budget level from 1 to the data's highest.</summary>
        public static Command RunCampaign(int labId, string sectorId, int level)
        {
            Command c = Make(CommandKind.RunCampaign, labId);
            c.Id = sectorId;
            c.Count = level;
            return c;
        }

        /// <summary>Applies for a research grant of a tier (1 to the data's top). A fee, a review of some weeks, and then a payout that owes one paper for each tier.</summary>
        public static Command ApplyForGrant(int labId, int tier)
        {
            Command c = Make(CommandKind.ApplyForGrant, labId);
            c.Count = tier;
            return c;
        }

        /// <summary>Sets the lab's service-level pledge (0 to the data's top level). Revenue premium each week, refunds and reputation loss in a week the promise breaks. A change locks it for some weeks.</summary>
        public static Command SetSla(int labId, int level)
        {
            Command c = Make(CommandKind.SetSla, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Publishes a post-mortem of a model's last incident: depth 1 is a summary, 2 a full write-up that also tells the public the model's true alignment. Within a few weeks of the incident, once for each.</summary>
        public static Command PublishPostMortem(int labId, int modelId, int depth)
        {
            Command c = Make(CommandKind.PublishPostMortem, labId);
            c.TargetId = modelId;
            c.Count = depth;
            return c;
        }

        /// <summary>Hires a firm of policy consultants (level 1 to the data's highest) for a few weeks. Costs a fee and a weekly bill, and the next firm has to wait.</summary>
        public static Command HireConsultants(int labId, int level)
        {
            Command c = Make(CommandKind.HireConsultants, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the level of the on-call rota, from 0 (none) to the data's highest. Raising needs safety people and engineers; any change locks the rota for some weeks.</summary>
        public static Command SetOnCall(int labId, int level)
        {
            Command c = Make(CommandKind.SetOnCall, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the level of inference optimisation, from 0 (none) to the data's highest. Raising pays a setup fee and needs engineers; lowering is free.</summary>
        public static Command SetOptimisation(int labId, int level)
        {
            Command c = Make(CommandKind.SetOptimisation, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sets the level of the ethics board, from 0 (none) to the data's highest. Seating levels costs cash and needs safety staff; stepping down costs reputation and locks the board out for a year.</summary>
        public static Command SetEthicsBoard(int labId, int level)
        {
            Command c = Make(CommandKind.SetEthicsBoard, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Names a person to a chief title ("scientist" or "safety") for a fee, or steps the sitting chief down when the person id is -1.</summary>
        public static Command AppointChief(int labId, string title, int personId)
        {
            Command c = Make(CommandKind.AppointChief, labId);
            c.Id = title;
            c.TargetId = personId;
            return c;
        }

        /// <summary>Gives the team the next level of equity out of the founder's stake. It cannot be taken back.</summary>
        public static Command GrantStaffEquity(int labId)
        {
            return Make(CommandKind.GrantStaffEquity, labId);
        }

        /// <summary>Lets the researchers build what they like for the data's weeks: happier, more research, slower training, then a cooldown.</summary>
        public static Command HoldHackWeek(int labId)
        {
            return Make(CommandKind.HoldHackWeek, labId);
        }

        /// <summary>Picks the lab's research direction by id, or "" to drop it. Costs cash and cannot be done again for a while.</summary>
        public static Command SetResearchFocus(int labId, string focusId)
        {
            Command c = Make(CommandKind.SetResearchFocus, labId);
            c.Id = focusId ?? "";
            return c;
        }

        /// <summary>Pays a red team to attack one model of the lab (trained or on sale) for the data's weeks of fewer incidents.</summary>
        public static Command RedTeamModel(int labId, int modelId)
        {
            Command c = Make(CommandKind.RedTeamModel, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Pays outside auditors to test one model (trained or on sale). They publish its real alignment.</summary>
        public static Command CommissionAudit(int labId, int modelId)
        {
            Command c = Make(CommandKind.CommissionAudit, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Takes the whole team away for a day: morale up for everyone who has room to rise, for about a week of salaries.</summary>
        public static Command HoldOffsite(int labId)
        {
            return Make(CommandKind.HoldOffsite, labId);
        }

        /// <summary>Starts crunch time (on) or ends it (off).</summary>
        public static Command SetCrunch(int labId, bool on)
        {
            Command c = Make(CommandKind.SetCrunch, labId);
            c.Count = on ? 1 : 0;
            return c;
        }

        /// <summary>Borrows against the lab's chips: this share of what it could borrow now, in permille.</summary>
        public static Command TakeLoan(int labId, int permilleOfRoom)
        {
            Command c = Make(CommandKind.TakeLoan, labId);
            c.Permille = permilleOfRoom;
            return c;
        }

        /// <summary>Pays off a loan in one go.</summary>
        public static Command RepayLoan(int labId, int loanId)
        {
            Command c = Make(CommandKind.RepayLoan, labId);
            c.TargetId = loanId;
            return c;
        }

        /// <summary>Changes what the lab is good at, once per era. It costs money and staff, and customers are confused for a year.</summary>
        public static Command PivotStyle(int labId, string styleId)
        {
            Command c = Make(CommandKind.PivotStyle, labId);
            c.Id = styleId;
            return c;
        }

        public static Command ResearchFrontier(int labId)
        {
            return Make(CommandKind.ResearchFrontier, labId);
        }

        /// <summary><paramref name="id"/> is <c>found-again</c> or <c>spectate</c>.</summary>
        public static Command SellLab(int labId, string mode)
        {
            Command c = Make(CommandKind.SellLab, labId);
            c.Id = mode;
            return c;
        }

        /// <summary><paramref name="id"/> is the continue mode; <paramref name="targetId"/> is the index in <see cref="GameState.Endings"/>.</summary>
        public static Command ContinueAfterEnding(int labId, int endingIndex, string mode)
        {
            Command c = Make(CommandKind.ContinueAfterEnding, labId);
            c.Id = mode;
            c.TargetId = endingIndex;
            return c;
        }

        public static Command StartShadyOp(int labId, string cardId, int modelId, int targetLabId, int auxId, string extraRef)
        {
            Command c = Make(CommandKind.StartShadyOp, labId);
            c.Id = cardId;
            c.TargetId = modelId;
            c.X = targetLabId;
            c.Y = auxId;
            if (!string.IsNullOrEmpty(extraRef)) c.Ids.Add(extraRef);
            return c;
        }

        public static Command Launder(int labId, int amount)
        {
            Command c = Make(CommandKind.Launder, labId);
            c.Count = amount;
            return c;
        }

        public static Command RaiseFunding(int labId, string roundId)
        {
            Command c = Make(CommandKind.RaiseFunding, labId);
            c.Id = roundId;
            return c;
        }

        public static Command SignReservedCompute(int labId, int gpuCount)
        {
            Command c = Make(CommandKind.SignReservedCompute, labId);
            c.Count = gpuCount;
            return c;
        }

        public static Command AcceptChinaDeal(int labId) { return Make(CommandKind.AcceptChinaDeal, labId); }

        public static Command DeclineChinaDeal(int labId) { return Make(CommandKind.DeclineChinaDeal, labId); }

        public static Command QueueGpuPurchase(int labId, int count, int holdingId)
        {
            Command c = Make(CommandKind.QueueGpuPurchase, labId);
            c.Count = count;
            c.TargetId = holdingId;
            return c;
        }

        public static Command AcquireLab(int buyerId, int targetLabId)
        {
            Command c = Make(CommandKind.AcquireLab, buyerId);
            c.TargetId = targetLabId;
            return c;
        }

        public static Command SellModel(int labId, int modelId)
        {
            Command c = Make(CommandKind.SellModel, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Answers a rival's offer (<paramref name="offerId"/>) with a counter from <c>talent.counters</c> (<paramref name="counterId"/>).</summary>
        public static Command CounterPoach(int labId, int offerId, string counterId)
        {
            Command c = Make(CommandKind.CounterPoach, labId);
            c.TargetId = offerId;
            c.Id = counterId;
            return c;
        }

        /// <summary>Tries to poach the best person of a role from a rival lab.</summary>
        public static Command PoachStaff(int labId, int rivalLabId, string roleId)
        {
            Command c = Make(CommandKind.PoachStaff, labId);
            c.TargetId = rivalLabId;
            c.Id = roleId;
            return c;
        }

        /// <summary>Answers a journalist's question (<paramref name="inquiryId"/>) with a response from <c>press.responses</c>.</summary>
        public static Command AnswerPress(int labId, int inquiryId, string responseId)
        {
            Command c = Make(CommandKind.AnswerPress, labId);
            c.TargetId = inquiryId;
            c.Id = responseId;
            return c;
        }

        /// <summary>Leaks a rival's recent incident to the press.</summary>
        public static Command LeakStory(int labId, int rivalLabId)
        {
            Command c = Make(CommandKind.LeakStory, labId);
            c.TargetId = rivalLabId;
            return c;
        }

        /// <summary>Puts a finished model to work on the design of a model still in training. <see cref="Command.TargetId"/> is the run and <see cref="Command.Count"/> the helper.</summary>
        public static Command DesignWithModel(int labId, int runId, int helperId)
        {
            Command c = Make(CommandKind.DesignWithModel, labId);
            c.TargetId = runId;
            c.Count = helperId;
            return c;
        }

        /// <summary>Chooses what a model still in training is for. <see cref="Command.Id"/> is the objective.</summary>
        public static Command SetTrainingObjective(int labId, int modelId, string objectiveId)
        {
            Command c = Make(CommandKind.SetTrainingObjective, labId);
            c.TargetId = modelId;
            c.Id = objectiveId;
            return c;
        }

        /// <summary>Chooses how a model still in training is built. <see cref="Command.Id"/> is the approach.</summary>
        public static Command SetRunApproach(int labId, int modelId, string approachId)
        {
            Command c = Make(CommandKind.SetRunApproach, labId);
            c.TargetId = modelId;
            c.Id = approachId;
            return c;
        }

        /// <summary>Chooses how often a model still in training is checkpointed. <see cref="Command.Id"/> is the habit.</summary>
        public static Command SetCheckpoints(int labId, int modelId, string checkpointId)
        {
            Command c = Make(CommandKind.SetCheckpoints, labId);
            c.TargetId = modelId;
            c.Id = checkpointId;
            return c;
        }

        /// <summary>Answers the office event on the desk. <see cref="Command.Id"/> is the choice.</summary>
        public static Command AnswerOfficeEvent(int labId, string choiceId)
        {
            Command c = Make(CommandKind.AnswerOfficeEvent, labId);
            c.Id = choiceId;
            return c;
        }

        /// <summary>Pays to end a glitch on a model that is on sale now. <see cref="Command.TargetId"/> is the model.</summary>
        public static Command FineTuneModel(int labId, int modelId)
        {
            Command c = Make(CommandKind.FineTuneModel, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Withdraws a glitching model and puts the previous version of its line back on sale. <see cref="Command.TargetId"/> is the model.</summary>
        public static Command RollBackModel(int labId, int modelId)
        {
            Command c = Make(CommandKind.RollBackModel, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Starts an investigation into a lucky run. <see cref="Command.TargetId"/> is the lead.</summary>
        public static Command FollowLead(int labId, int leadId)
        {
            Command c = Make(CommandKind.FollowLead, labId);
            c.TargetId = leadId;
            return c;
        }

        /// <summary>Answers a customer's complaint.</summary>
        public static Command AnswerComplaint(int labId, int complaintId, string responseId)
        {
            Command c = Make(CommandKind.AnswerComplaint, labId);
            c.TargetId = complaintId;
            c.Id = responseId;
            return c;
        }

        /// <summary>Sets the lab's own release gate: the least alignment a model may show and still go on sale. 0 removes it.</summary>
        public static Command SetReleaseGate(int labId, int alignment)
        {
            Command c = Make(CommandKind.SetReleaseGate, labId);
            c.Permille = alignment;
            return c;
        }

        /// <summary>Pays the marketing team to tidy the lab's model names, taking some of the naming mess away.</summary>
        public static Command CleanUpNaming(int labId)
        {
            return Make(CommandKind.CleanUpNaming, labId);
        }

        /// <summary>Pays to bring one room up to date, so it is as new as the era (age zero).</summary>
        public static Command RefurbishRoom(int labId, int roomId)
        {
            Command c = Make(CommandKind.RefurbishRoom, labId);
            c.TargetId = roomId;
            return c;
        }

        /// <summary>Sets how hard the lab tunes its entries for the next Gauntlet, in permille. 0 turns it off.</summary>
        public static Command SetGauntletTuning(int labId, int permille)
        {
            Command c = Make(CommandKind.SetGauntletTuning, labId);
            c.Permille = permille;
            return c;
        }
    }

    public enum CommandError
    {
        None,
        UnknownLab, LabBankrupt, UnknownId, InvalidArgument,
        OutOfBounds, Overlap, TooSmall, NoBiggerLot,
        NotEnoughCash, NoSeats, NoGpuSlots, StaffNeedSeats, GpusNeedSlots, NotEnoughStaff, NotEnoughGpus,
        AlreadyResearched, MissingPrerequisite, NotEnoughResearch,
        LineLocked, TooManyRuns, WrongModelStatus, SectorClosed, BelowSectorBar,
        EraLocked,
        SiteClosed, NoFreePlot, GridFull, NotEnoughPower, ExportBlocked, PermitFrozen, BuildRunning, HoldingNotEmpty,
        AllocationExhausted,
        DesignNotReady, FoundryFull,
        ShadyLocked, NoModelForJob, TargetNotEligible, NotEnoughStash, CaseOpen,
        EndingUnavailable, AlreadyChose, SellRefused, Spectating, FrontierUnavailable, BoardLocked,
        BelowEraSafetyBar, OfferGone, InquiryGone, ComplaintGone, BelowReleaseGate, GateLocked, LeadGone, NoOfficeEvent, LobbyCooling, PivotSpent, LoanLimit, TeamTooTired, Uninsurable, InsuranceLocked, PayLocked, SubletLocked, FocusLocked, HackLocked, WouldLoseControl, EthicsLocked, BoardVeto, CampaignLocked, OnCallLocked, ConsultantsLocked, SlaLocked, GrantLocked
    }

    public struct CommandResult
    {
        public bool Ok;
        public CommandError Error;
        /// <summary>Why the order was refused, in plain words. Empty when it went through.</summary>
        public string Message;
        /// <summary>The id of the room or model the order made, or -1.</summary>
        public int CreatedId;

        public static CommandResult Done(int createdId)
        {
            var r = new CommandResult();
            r.Ok = true;
            r.Error = CommandError.None;
            r.Message = "";
            r.CreatedId = createdId;
            return r;
        }

        public static CommandResult Fail(CommandError error, string message)
        {
            var r = new CommandResult();
            r.Ok = false;
            r.Error = error;
            r.Message = message;
            r.CreatedId = -1;
            return r;
        }
    }
}
