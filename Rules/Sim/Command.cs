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
        ApplyForGrant,
        ScrapRun,
        Layoff,
        PayDownDebt,
        PromoteStaff,
        SetRevenueTarget,
        EndowChair,
        AnswerWhistle,
        SetBugBounty,
        PullOutOfSector,
        RestructureLoan,
        SetAsideReserve,
        DrawReserve,
        SetPledge,
        AnnounceRoadmap,
        AssignMentor,
        EndMentoring,
        StartSkunkworks,
        WrapUpSkunkworks,
        RecogniseUnion,
        ResistUnion,
        AnnounceSunset,
        CancelSunset,
        AssignAccountTeam,
        SubmitPaper,
        WithdrawPaper,
        RehireAlumnus,
        StartInternships,
        EndInternships,
        StaffSupportDesk,
        SendOnSabbatical,
        RecallFromSabbatical,
        OpenRequisition,
        CancelRequisition,
        HostScholars,
        SendScholarsHome,
        SettleCase,
        RetainFirm,
        AppealCase,
        BuildSecurity,
        AnswerBreach,
        AcceptContract,
        DeclineContract,
        AbandonContract,
        ConvertCharter,
        CancelCharterChange,
        AdoptPolicy,
        BreakPolicy,
        BuildTaxStructure,
        ClaimTaxCredit,
        AnswerTaxAudit,
        TakeStandardsRole,
        AdoptStandards,
        DropStandards,
        SetWarranty,
        StockSpares,
        ExpediteReplacements,
        WaiveCovenant,
        CureCovenant,
        SetCapitalPolicy,
        ListCompany,
        SetGuidance,
        FollowOn,
        BuyBackShares,
        TakePrivate,
        TakeRevenueAdvance,
        BuyOutAdvance,
        EndowFoundation,
        SetFoundationFocus,
        SetFoundationPayout,
        WindUpFoundation,
        SaleLeaseback,
        BuyBackLease,
        ExtendLease,
        ReturnLease,
        IssueNote,
        RepayNote,
        ExtendNote,
        DesignDevice,
        SetDevicePrice,
        WithdrawDevice,
        SignPlatform,
        RenewPlatform,
        WalkFromPlatform,
        SetCreed,
        HoldTownHall,
        OwnUp,
        AnswerShock,
        ReleaseCode,
        WithholdCode,
        BetOnTech,
        WithdrawTechBet,
        OpenWaitlist,
        CloseWaitlist,
        OpenCareersPage,
        AnswerReview,
        OpenFreeTier,
        CloseFreeTier,
        OpenWatchDesk,
        CloseWatchDesk,
        AnswerLaunch,
        StartProgramme,
        ChooseApproach,
        RecoverStage,
        AbandonProgramme,
        SetRunPriority,
        FormPod,
        SwapPodMember,
        DisbandPod,
        ApplyLicence,
        RenewLicence,
        SurrenderLicence,
        PledgeSupport,
        BreakSupportPledge,
        OpenApi,
        DeprecateApi,
        CancelDeprecation,
        StartConformity,
        SubmitEvidence,
        WithdrawConformity,
        SetOrg,
        OrderFabric,
        CancelFabric,
        JoinRegister,
        LeaveRegister,
        FileReport,
        AcquireData,
        CleanData,
        LicenseScraped,
        SetUserData,
        SetGovernance,
        SeatDirector,
        BuyBackRound,
        SetSpotShare,
        SetAutonomy,
        ContractAnnotators,
        EndAnnotators,
        AuditVendor,
        PostTrain,
        AbandonPostTraining,
        AssignOwner,
        WriteDocs,
        HoldBoardMeetings,
        AnswerMotion,
        StartDrill,
        RegisterEval,
        SetToolPolicy,
        StaffHumanQueue,
        ClearHumanQueue,
        SignChecklistItem,
        StartSandbox,
        SetPlaybook,
        ArmPlaybook,
        SetOrchBudget,
        SetRouteShare,
        PublishMetrics,
        StopMetrics,
        BranchSpecialist,
        AcquireDataSource,
        RenewDataSource,
        SetWeightsPolicy,
        SetSectorPrice,
        SetCampaignDeadline,
        ShapeLaw,
        RespondTakeoff,
        PickScenarioBaggage,
        SatisfyMarginCall,
        RespondRungSix,
        SetLawTarget
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

        /// <summary>Scraps a training run that is still going: its compute is gone, part of it comes back as research points, the press and the team mind in proportion to how far along it was.</summary>
        public static Command ScrapRun(int labId, int modelId)
        {
            Command c = Make(CommandKind.ScrapRun, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Lets go of a share (permille, up to the data's cap) of every function at once, on a reduced package. The people who stay and the press mind it.</summary>
        public static Command Layoff(int labId, int permille)
        {
            Command c = Make(CommandKind.Layoff, labId);
            c.Permille = permille;
            return c;
        }

        /// <summary>Pays down technical debt by a number of points (up to the data's cap per command) for cash. Needs engineers.</summary>
        public static Command PayDownDebt(int labId, int points)
        {
            Command c = Make(CommandKind.PayDownDebt, labId);
            c.Count = points;
            return c;
        }

        /// <summary>Promotes one person to senior: a fee and a raise now, and they count for more in their function. Needs the person's id.</summary>
        public static Command PromoteStaff(int labId, int staffId)
        {
            Command c = Make(CommandKind.PromoteStaff, labId);
            c.TargetId = staffId;
            return c;
        }

        /// <summary>Commits the lab to a revenue target for the next quarter, at a level from 1 to the data's top. A harder target pays more reputation if met and costs more if missed.</summary>
        public static Command SetRevenueTarget(int labId, int level)
        {
            Command c = Make(CommandKind.SetRevenueTarget, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Endows a university chair: a big one-off cost that pays for good in research points and a floor under academic prestige, and a stipend every week.</summary>
        public static Command EndowChair(int labId)
        {
            return Make(CommandKind.EndowChair, labId);
        }

        /// <summary>Answers a safety person's concern about a model's shown alignment: 0 comes clean (the shown number drops to the real one), 1 pays for silence (the gap stays and the next silence costs more).</summary>
        public static Command AnswerWhistle(int labId, int choice)
        {
            Command c = Make(CommandKind.AnswerWhistle, labId);
            c.Count = choice;
            return c;
        }

        /// <summary>Sets the bug bounty level, 0 to the data's top. Raising it costs a fee per level, every level costs a bill every week for each released model, and it takes a share off the weekly incident chance; a change locks the level for some weeks.</summary>
        public static Command SetBugBounty(int labId, int level)
        {
            Command c = Make(CommandKind.SetBugBounty, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Pulls a model on sale out of one sector: a break fee of some weeks of what it earned there, lost trust and reputation, and the share it had built. A model with no sector left is retired.</summary>
        public static Command PullOutOfSector(int labId, int modelId, string sectorId)
        {
            Command c = Make(CommandKind.PullOutOfSector, labId);
            c.TargetId = modelId;
            c.Ids.Add(sectorId);
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

        /// <summary>Asks the lender to stretch a loan over a longer term: a lower weekly payment for a fee added to what is owed, a dent in the lab's name and a clean sheet of missed payments.</summary>
        public static Command RestructureLoan(int labId, int loanId)
        {
            Command c = Make(CommandKind.RestructureLoan, labId);
            c.TargetId = loanId;
            return c;
        }

        /// <summary>Moves cash into the treasury reserve. It earns a small yield every week and covers a debt the weekly bill would otherwise leave. Count is dollars.</summary>
        public static Command SetAsideReserve(int labId, int dollars)
        {
            Command c = Make(CommandKind.SetAsideReserve, labId);
            c.Count = dollars;
            return c;
        }

        /// <summary>Takes dollars back out of the reserve, minus a fee. Count is dollars taken from the reserve.</summary>
        public static Command DrawReserve(int labId, int dollars)
        {
            Command c = Make(CommandKind.DrawReserve, labId);
            c.Count = dollars;
            return c;
        }

        /// <summary>Makes a public safety pledge, level 1 to the data's top, or 0 to walk it back. While it holds, customers trust the lab a little more every week; a launch that shows less alignment than the pledge asks breaks it. Lowering it costs reputation, and a change locks it for weeks.</summary>
        public static Command SetPledge(int labId, int level)
        {
            Command c = Make(CommandKind.SetPledge, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Announces a public roadmap: a model at least so much more capable than the lab's best (level 1 to the data's top, a bolder promise at each) inside a number of weeks (TargetId). The announcement earns reputation at once; delivering earns more and the deadline passing costs far more.</summary>
        public static Command AnnounceRoadmap(int labId, int level, int weeks)
        {
            Command c = Make(CommandKind.AnnounceRoadmap, labId);
            c.Count = level;
            c.TargetId = weeks;
            return c;
        }

        /// <summary>Pairs a senior (Count, the person's id) with a junior in the same function (TargetId). The junior's morale climbs and, if the pair holds long enough and a senior seat is free, they are promoted for nothing; meanwhile both count for less.</summary>
        public static Command AssignMentor(int labId, int mentorId, int menteeId)
        {
            Command c = Make(CommandKind.AssignMentor, labId);
            c.Count = mentorId;
            c.TargetId = menteeId;
            return c;
        }

        /// <summary>Calls off the mentoring of the junior with this id (TargetId). The weeks so far are lost.</summary>
        public static Command EndMentoring(int labId, int menteeId)
        {
            Command c = Make(CommandKind.EndMentoring, labId);
            c.TargetId = menteeId;
            return c;
        }

        /// <summary>Pulls a team (Count people, at least the data's minimum) out of a function for a project of TargetId weeks: Id is "research", "safety" or "engineering". The team is away the whole time and the project costs a budget up front; it pays research points, alignment on the lab's finished models or cleared technical debt, by the person-weeks worked.</summary>
        public static Command StartSkunkworks(int labId, string kind, int people, int weeks)
        {
            Command c = Make(CommandKind.StartSkunkworks, labId);
            c.Id = kind;
            c.Count = people;
            c.TargetId = weeks;
            return c;
        }

        /// <summary>Ends the running project now and pays the share of the work done so far that the data allows for stopping early.</summary>
        public static Command WrapUpSkunkworks(int labId)
        {
            return Make(CommandKind.WrapUpSkunkworks, labId);
        }

        /// <summary>Answers a staff drive to organise by recognising the union: a contract with better pay, dearer layoffs, no crunch time and a little more research.</summary>
        public static Command RecogniseUnion(int labId)
        {
            return Make(CommandKind.RecogniseUnion, labId);
        }

        /// <summary>Answers a staff drive by resisting it: legal fees now, and the drive collapses only if pressure is back under the drive level at the end of the resisting period; a strike comes first if pressure tops out.</summary>
        public static Command ResistUnion(int labId)
        {
            return Make(CommandKind.ResistUnion, labId);
        }

        /// <summary>Gives a released model an end-of-life notice of Count weeks (TargetId is the model). On the day its customers move to the successor it helped design, and the model is retired. Needs such a successor on sale, and costs the lab support while the notice runs.</summary>
        public static Command AnnounceSunset(int labId, int modelId, int weeks)
        {
            Command c = Make(CommandKind.AnnounceSunset, labId);
            c.TargetId = modelId;
            c.Count = weeks;
            return c;
        }

        /// <summary>Calls a model's end-of-life notice off. It costs the lab some reputation.</summary>
        public static Command CancelSunset(int labId, int modelId)
        {
            Command c = Make(CommandKind.CancelSunset, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Sets the salespeople on a sector's account team (Id is the sector, Count the people, 0 to disband). The sector needs a model of the lab on sale, and the people leave the lab-wide sales force. A new team starts from nothing and a changed one loses part of what it built.</summary>
        public static Command AssignAccountTeam(int labId, string sectorId, int people)
        {
            Command c = Make(CommandKind.AssignAccountTeam, labId);
            c.Id = sectorId;
            c.Count = people;
            return c;
        }

        /// <summary>Sends a paper to a venue (TargetId 1 workshop, 2 main conference, 3 flagship) with an effort of Count (1 to 3). The research points are paid now; the decision comes after the venue's review weeks and depends only on the lab's prestige that day plus the effort.</summary>
        public static Command SubmitPaper(int labId, int venue, int effort)
        {
            Command c = Make(CommandKind.SubmitPaper, labId);
            c.TargetId = venue;
            c.Count = effort;
            return c;
        }

        /// <summary>Takes a paper out of review (TargetId is its place in the lab's list, from 0) for part of the points back.</summary>
        public static Command WithdrawPaper(int labId, int index)
        {
            Command c = Make(CommandKind.WithdrawPaper, labId);
            c.TargetId = index;
            return c;
        }

        /// <summary>Brings a former employee back (TargetId is the person's old id from the lab's alumni list). It costs a share of the hiring fee that is smaller for a better parting, needs a free seat, and the person returns with their old trait and a morale that depends on how they left.</summary>
        public static Command RehireAlumnus(int labId, int personId)
        {
            Command c = Make(CommandKind.RehireAlumnus, labId);
            c.TargetId = personId;
            return c;
        }

        /// <summary>Takes on a cohort of interns in a role (Id the role, Count the interns). The stipend is paid weekly and must be in the bank now; interns hold no seat and do no work. At the end each is offered the job for a share of the hiring fee, if a seat and the cash are there.</summary>
        public static Command StartInternships(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.StartInternships, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        /// <summary>Sends the cohort home early. Nobody is offered a job; they join the alumni pool on good terms.</summary>
        public static Command EndInternships(int labId)
        {
            return Make(CommandKind.EndInternships, labId);
        }

        /// <summary>Sets the engineers on the support desk (Count; 0 closes it). They leave engineering and training work, clear a set number of tickets each a week, and what is not cleared piles up as a backlog that costs trust once it passes a limit.</summary>
        public static Command StaffSupportDesk(int labId, int people)
        {
            Command c = Make(CommandKind.StaffSupportDesk, labId);
            c.Count = people;
            return c;
        }

        /// <summary>Sends a person away for the sabbatical weeks on full pay (TargetId is the person). They do no work and cannot quit while away. A worn-down person comes back refreshed, a happy one restless, the rest a little better than settled.</summary>
        public static Command SendOnSabbatical(int labId, int personId)
        {
            Command c = Make(CommandKind.SendOnSabbatical, labId);
            c.TargetId = personId;
            return c;
        }

        /// <summary>Calls a person back early (TargetId is the person). They return with the morale they left with.</summary>
        public static Command RecallFromSabbatical(int labId, int personId)
        {
            Command c = Make(CommandKind.RecallFromSabbatical, labId);
            c.TargetId = personId;
            return c;
        }

        /// <summary>Orders people in a role for a future date (Id the role, Count the people). A deposit is paid now; the price is locked below the hiring fee of today. On the day each needs a free seat and the rest of the price, and anyone who cannot be seated or paid for is lost with their deposit.</summary>
        public static Command OpenRequisition(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.OpenRequisition, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        /// <summary>Cancels an open order (TargetId is its place in the lab's list, from 0). The deposit is lost.</summary>
        public static Command CancelRequisition(int labId, int index)
        {
            Command c = Make(CommandKind.CancelRequisition, labId);
            c.TargetId = index;
            return c;
        }

        /// <summary>Hosts a group of visiting scholars (Count) at free researcher seats. They are paid a stipend, add research points each week and, when they leave on schedule, add prestige.</summary>
        public static Command HostScholars(int labId, int count)
        {
            Command c = Make(CommandKind.HostScholars, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Sends the scholars home early. No prestige is earned.</summary>
        public static Command SendScholarsHome(int labId)
        {
            return Make(CommandKind.SendScholarsHome, labId);
        }

        /// <summary>Settles a claim before its hearing (TargetId is the claim) for a share of what it asks. It ends there.</summary>
        public static Command SettleCase(int labId, int caseId)
        {
            Command c = Make(CommandKind.SettleCase, labId);
            c.TargetId = caseId;
            return c;
        }

        /// <summary>Retains an outside firm for a claim before its hearing (TargetId is the claim): a fee now, and the firm takes more off the claim's strength than the lab's own counsel.</summary>
        public static Command RetainFirm(int labId, int caseId)
        {
            Command c = Make(CommandKind.RetainFirm, labId);
            c.TargetId = caseId;
            return c;
        }

        /// <summary>Appeals a lost claim once (TargetId is the claim): a fee now, a new ruling after the appeal weeks, and the appeal takes something more off the claim's strength.</summary>
        public static Command AppealCase(int labId, int caseId)
        {
            Command c = Make(CommandKind.AppealCase, labId);
            c.TargetId = caseId;
            return c;
        }

        /// <summary>Starts building the next security tier. It is paid for now and done after the build weeks; the lab pays upkeep for every tier it holds.</summary>
        public static Command BuildSecurity(int labId)
        {
            return Make(CommandKind.BuildSecurity, labId);
        }

        /// <summary>Answers an open breach: 0 pays the ransom, 1 discloses it, 2 locks everything down at once (the next tier now, at double the price).</summary>
        public static Command AnswerBreach(int labId, int choice)
        {
            Command c = Make(CommandKind.AnswerBreach, labId);
            c.Count = choice;
            return c;
        }

        /// <summary>Accepts the contract on offer. It needs the security tier, free engineers and no other contract; accepting can cost reputation and the safety staff's morale.</summary>
        public static Command AcceptContract(int labId)
        {
            return Make(CommandKind.AcceptContract, labId);
        }

        /// <summary>Turns the contract on offer down. The next offer comes after the usual wait.</summary>
        public static Command DeclineContract(int labId)
        {
            return Make(CommandKind.DeclineContract, labId);
        }

        /// <summary>Walks away from the contract in force: the penalty is paid and reputation is lost.</summary>
        public static Command AbandonContract(int labId)
        {
            return Make(CommandKind.AbandonContract, labId);
        }

        /// <summary>Starts a change of charter (Count is the form: 0 for-profit, 1 capped-profit, 2 nonprofit). The fee is paid now and the change takes effect after the review weeks.</summary>
        public static Command ConvertCharter(int labId, int form)
        {
            Command c = Make(CommandKind.ConvertCharter, labId);
            c.Count = form;
            return c;
        }

        /// <summary>Drops the change of charter under review. The fee is not given back.</summary>
        public static Command CancelCharterChange(int labId)
        {
            return Make(CommandKind.CancelCharterChange, labId);
        }

        /// <summary>Sets the scaling policy level, 0 to the data's top. Raising it costs a fee for each level up; lowering it costs reputation and is refused while the level is locked; either change locks lowering for some weeks. Level k has a tripwire at a capability: a trained model that reaches a tripwire of the policy is held back, and so is every new run, until the safeguards for that level are in place.</summary>
        public static Command AdoptPolicy(int labId, int level)
        {
            Command c = Make(CommandKind.AdoptPolicy, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Breaks the policy while a tripwire holds the lab up: the level drops below the tripwire, the lab loses reputation and the safety staff lose morale.</summary>
        public static Command BreakPolicy(int labId)
        {
            return Make(CommandKind.BreakPolicy, labId);
        }

        /// <summary>Builds a tax structure of this tier (1 a holding company, 2 an offshore one) over some weeks for a fee, or unwinds the structure at once when the tier is 0. A structure takes a share off the tax and adds a weekly fee and an exposure that builds toward an audit.</summary>
        public static Command BuildTaxStructure(int labId, int tier)
        {
            Command c = Make(CommandKind.BuildTaxStructure, labId);
            c.Count = tier;
            return c;
        }

        /// <summary>Claims the research credit: a share of the year's training spend comes off the tax bill. It adds exposure to an audit.</summary>
        public static Command ClaimTaxCredit(int labId)
        {
            return Make(CommandKind.ClaimTaxCredit, labId);
        }

        /// <summary>Answers an open tax audit: 0 cooperate (pay back what was saved with interest and unwind the structure), 1 settle (pay part and keep the structure), 2 contest (the claim goes on the litigation docket).</summary>
        public static Command AnswerTaxAudit(int labId, int choice)
        {
            Command c = Make(CommandKind.AnswerTaxAudit, labId);
            c.Count = choice;
            return c;
        }

        /// <summary>Takes a part in the draft of the next standard: 0 observer, 1 member, 2 sponsor, 3 obstructor. A member and a sponsor speed the draft up for dues each week, a sponsor also pays a fee to become one, and an obstructor holds it back for dues and reputation.</summary>
        public static Command TakeStandardsRole(int labId, int role)
        {
            Command c = Make(CommandKind.TakeStandardsRole, labId);
            c.Count = role;
            return c;
        }

        /// <summary>Adopts every standard ratified so far, for a cost on each new level and an upkeep each week.</summary>
        public static Command AdoptStandards(int labId)
        {
            return Make(CommandKind.AdoptStandards, labId);
        }

        /// <summary>Stops complying with the standards: the upkeep ends, and a capable release is fined again.</summary>
        public static Command DropStandards(int labId)
        {
            return Make(CommandKind.DropStandards, labId);
        }

        /// <summary>Sets the GPU warranty tier: 0 none, 1 or 2. A failed unit under a warranty is replaced with the current generation after some weeks (fewer at tier 2) instead of being lost. A fee for each unit every week; raising the tier costs a switch fee.</summary>
        public static Command SetWarranty(int labId, int tier)
        {
            Command c = Make(CommandKind.SetWarranty, labId);
            c.Count = tier;
            return c;
        }

        /// <summary>Buys spare GPUs of the current generation to hold. A spare takes the place of a failed unit at once, for upkeep every week.</summary>
        public static Command StockSpares(int labId, int count)
        {
            Command c = Make(CommandKind.StockSpares, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Pays to have the units waiting on a warranty replacement delivered now.</summary>
        public static Command ExpediteReplacements(int labId)
        {
            return Make(CommandKind.ExpediteReplacements, labId);
        }

        /// <summary>Pays the lender a fee, a share of what is owed, to look away from a breach of the loan covenant for a spell. Only a few times in all.</summary>
        public static Command WaiveCovenant(int labId)
        {
            return Make(CommandKind.WaiveCovenant, labId);
        }

        /// <summary>Pays the loans down, oldest first, until the chips cover what is left as the covenant needs.</summary>
        public static Command CureCovenant(int labId)
        {
            return Make(CommandKind.CureCovenant, labId);
        }

        /// <summary>Sets how hardware and building spend is deducted for tax: 0 expense it now, 1 capitalise and write it off over time, 2 an enhanced allowance that deducts more than was spent for audit exposure. Locked for a time after a change.</summary>
        public static Command SetCapitalPolicy(int labId, int policy)
        {
            Command c = Make(CommandKind.SetCapitalPolicy, labId);
            c.Count = policy;
            return c;
        }

        /// <summary>Lists the company: sells a slice of it to the public for cash, pays a fee, and from then on reports every quarter. Needs reputation and revenue, and not a nonprofit.</summary>
        public static Command ListCompany(int labId)
        {
            return Make(CommandKind.ListCompany, labId);
        }

        /// <summary>Sets the guidance for the quarter (1 conservative, 2 base, 3 stretch) in its first weeks. The quarter is judged against it.</summary>
        public static Command SetGuidance(int labId, int level)
        {
            Command c = Make(CommandKind.SetGuidance, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Sells more shares at the current price for cash, which dilutes the founders again. Not often.</summary>
        public static Command FollowOn(int labId)
        {
            return Make(CommandKind.FollowOn, labId);
        }

        /// <summary>Spends cash, in thousands, to buy shares back: the price rises and the founders own more of the lab again.</summary>
        public static Command BuyBackShares(int labId, int thousands)
        {
            Command c = Make(CommandKind.BuyBackShares, labId);
            c.Count = thousands;
            return c;
        }

        /// <summary>Buys all the public's shares back and ends the listing.</summary>
        public static Command TakePrivate(int labId)
        {
            return Make(CommandKind.TakePrivate, labId);
        }

        /// <summary>Takes a revenue advance: tier 1, 2 or 3 (a bigger share of each week's revenue swept for a lower total), for a share of the largest the lab's revenue allows, in permille. One at a time.</summary>
        public static Command TakeRevenueAdvance(int labId, int tier, int permille)
        {
            Command c = Make(CommandKind.TakeRevenueAdvance, labId);
            c.Count = tier;
            c.Permille = permille;
            return c;
        }

        /// <summary>Buys the advance out: pays most of what is still owed, once it has run a while.</summary>
        public static Command BuyOutAdvance(int labId)
        {
            return Make(CommandKind.BuyOutAdvance, labId);
        }

        /// <summary>Puts cash, in thousands, into the foundation's fund. The first gift sets its focus (1 safety, 2 education, 3 open science).</summary>
        public static Command EndowFoundation(int labId, int thousands, int focus)
        {
            Command c = Make(CommandKind.EndowFoundation, labId);
            c.Count = thousands;
            c.TargetId = focus;
            return c;
        }

        /// <summary>Points the foundation at another cause. It loses part of its goodwill and cannot change again for a while.</summary>
        public static Command SetFoundationFocus(int labId, int focus)
        {
            Command c = Make(CommandKind.SetFoundationFocus, labId);
            c.TargetId = focus;
            return c;
        }

        /// <summary>Sets how much of the fund the foundation gives away each week (0 none, 1, 2 or 3).</summary>
        public static Command SetFoundationPayout(int labId, int level)
        {
            Command c = Make(CommandKind.SetFoundationPayout, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Winds the foundation up and takes most of the fund back; the goodwill goes with it.</summary>
        public static Command WindUpFoundation(int labId)
        {
            return Make(CommandKind.WindUpFoundation, labId);
        }

        /// <summary>Sells some of the head office's own GPUs for most of their value and leases them straight back. One lease at a time.</summary>
        public static Command SaleLeaseback(int labId, int count)
        {
            Command c = Make(CommandKind.SaleLeaseback, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Buys the leased GPUs back at the price set at the sale.</summary>
        public static Command BuyBackLease(int labId)
        {
            return Make(CommandKind.BuyBackLease, labId);
        }

        /// <summary>Extends the lease once, at a higher rent.</summary>
        public static Command ExtendLease(int labId)
        {
            return Make(CommandKind.ExtendLease, labId);
        }

        /// <summary>Hands the leased GPUs back now and stops the rent.</summary>
        public static Command ReturnLease(int labId)
        {
            return Make(CommandKind.ReturnLease, labId);
        }

        /// <summary>Issues a convertible note of tier 1, 2 or 3 (a bigger discount and more interest for less time), for an amount in thousands. One at a time.</summary>
        public static Command IssueNote(int labId, int tier, int thousands)
        {
            Command c = Make(CommandKind.IssueNote, labId);
            c.Count = thousands;
            c.TargetId = tier;
            return c;
        }

        /// <summary>Repays the note with its interest.</summary>
        public static Command RepayNote(int labId)
        {
            return Make(CommandKind.RepayNote, labId);
        }

        /// <summary>Extends the note once: more time for a higher rate and a deeper discount.</summary>
        public static Command ExtendNote(int labId)
        {
            return Make(CommandKind.ExtendNote, labId);
        }

        /// <summary>Starts designing a device (1 speaker, 2 glasses, 3 home robot) with some weeks of testing before the launch, which buys quality. One line at a time.</summary>
        public static Command DesignDevice(int labId, int line, int testWeeks)
        {
            Command c = Make(CommandKind.DesignDevice, labId);
            c.TargetId = line;
            c.Count = testWeeks;
            return c;
        }

        /// <summary>Sets the device's price tier: 1 low (more units, thin margin), 2 middle, 3 high.</summary>
        public static Command SetDevicePrice(int labId, int tier)
        {
            Command c = Make(CommandKind.SetDevicePrice, labId);
            c.Count = tier;
            return c;
        }

        /// <summary>Ends the device line: stops selling it or drops the design.</summary>
        public static Command WithdrawDevice(int labId)
        {
            return Make(CommandKind.WithdrawDevice, labId);
        }

        /// <summary>Signs a distribution deal with a platform (1 phone maker, 2 browser, 3 office suite). One at a time.</summary>
        public static Command SignPlatform(int labId, int platform)
        {
            Command c = Make(CommandKind.SignPlatform, labId);
            c.TargetId = platform;
            return c;
        }

        /// <summary>Renews the deal in its last weeks, at the price the platform has named.</summary>
        public static Command RenewPlatform(int labId)
        {
            return Make(CommandKind.RenewPlatform, labId);
        }

        /// <summary>Ends the deal now. The users who came through the platform go with it.</summary>
        public static Command WalkFromPlatform(int labId)
        {
            return Make(CommandKind.WalkFromPlatform, labId);
        }

        /// <summary>States the lab's values: two of 1 safety, 2 speed, 3 openness, 4 people, 5 profit. Changing them later starts from distrust.</summary>
        public static Command SetCreed(int labId, int first, int second)
        {
            Command c = Make(CommandKind.SetCreed, labId);
            c.TargetId = first;
            c.Count = second;
            return c;
        }

        /// <summary>Holds a town hall on the values, which lifts integrity. Not while the creed is a joke.</summary>
        public static Command HoldTownHall(int labId)
        {
            return Make(CommandKind.HoldTownHall, labId);
        }

        /// <summary>Admits to the staff that the lab has not lived up to its values. Costs reputation, brings integrity back.</summary>
        public static Command OwnUp(int labId)
        {
            return Make(CommandKind.OwnUp, labId);
        }

        /// <summary>Answers the shock waiting at the lab: 1 brace for it, 2 ride it out, 3 counter it with the lab's best model.</summary>
        public static Command AnswerShock(int labId, int stance)
        {
            Command c = Make(CommandKind.AnswerShock, labId);
            c.Count = stance;
            return c;
        }

        /// <summary>Puts the code and data of the oldest waiting paper out after some weeks of cleaning it up, which costs research points and makes it likelier to replicate.</summary>
        public static Command ReleaseCode(int labId, int effort)
        {
            Command c = Make(CommandKind.ReleaseCode, labId);
            c.Count = effort;
            return c;
        }

        /// <summary>Keeps the code and data of the oldest waiting paper back. It costs some standing.</summary>
        public static Command WithholdCode(int labId)
        {
            return Make(CommandKind.WithholdCode, labId);
        }

        /// <summary>Publicly bets that the lab will research a tech (by id) within the data's weeks. One bet at a time.</summary>
        public static Command BetOnTech(int labId, string techId)
        {
            Command c = Make(CommandKind.BetOnTech, labId);
            c.Id = techId;
            return c;
        }

        /// <summary>Takes the public bet back. It costs a little reputation.</summary>
        public static Command WithdrawTechBet(int labId)
        {
            return Make(CommandKind.WithdrawTechBet, labId);
        }

        /// <summary>Opens a public waitlist for a model that is training or trained but not out, promising it in the given weeks.</summary>
        public static Command OpenWaitlist(int labId, int modelId, int weeks)
        {
            Command c = Make(CommandKind.OpenWaitlist, labId);
            c.TargetId = modelId;
            c.Count = weeks;
            return c;
        }

        /// <summary>Closes the waitlist early. It costs reputation by the hype it built.</summary>
        public static Command CloseWaitlist(int labId)
        {
            return Make(CommandKind.CloseWaitlist, labId);
        }

        /// <summary>Sets up a careers page: the lab starts being scored as an employer. It costs the data's cash.</summary>
        public static Command OpenCareersPage(int labId)
        {
            return Make(CommandKind.OpenCareersPage, labId);
        }

        /// <summary>Answers a bad review: 1 own up and fix it (costs cash for each person), 2 publicly rebut it, 3 leave it.</summary>
        public static Command AnswerReview(int labId, int stance)
        {
            Command c = Make(CommandKind.AnswerReview, labId);
            c.Count = stance;
            return c;
        }

        /// <summary>Opens a free tier of the lab's best released model at a level, 1 to 3. A higher level reaches more and costs more to serve.</summary>
        public static Command OpenFreeTier(int labId, int level)
        {
            Command c = Make(CommandKind.OpenFreeTier, labId);
            c.Count = level;
            return c;
        }

        /// <summary>Shuts the free tier. The people who built on it are upset by its reach.</summary>
        public static Command CloseFreeTier(int labId)
        {
            return Make(CommandKind.CloseFreeTier, labId);
        }

        /// <summary>Sets up a launch watch desk. It costs the data's cash and then a weekly bill.</summary>
        public static Command OpenWatchDesk(int labId)
        {
            return Make(CommandKind.OpenWatchDesk, labId);
        }

        /// <summary>Shuts the launch watch desk. A launch waiting for an answer is taken as held.</summary>
        public static Command CloseWatchDesk(int labId)
        {
            return Make(CommandKind.CloseWatchDesk, labId);
        }

        /// <summary>Answers a rival launch that beats the lab: 1 counter-programme (cash for each person), 2 hold, 3 own the safety story (needs a safe enough model).</summary>
        public static Command AnswerLaunch(int labId, int answer)
        {
            Command c = Make(CommandKind.AnswerLaunch, labId);
            c.Count = answer;
            return c;
        }

        /// <summary>Starts a multi-stage research programme aimed at a costly tech (by id). Nothing is paid until the first approach is chosen.</summary>
        public static Command StartProgramme(int labId, string techId)
        {
            Command c = Make(CommandKind.StartProgramme, labId);
            c.Id = techId;
            return c;
        }

        /// <summary>Picks how the next stage is run: 1 carefully, 2 boldly. The stage's research points are paid now.</summary>
        public static Command ChooseApproach(int labId, int approach)
        {
            Command c = Make(CommandKind.ChooseApproach, labId);
            c.Count = approach;
            return c;
        }

        /// <summary>Answers a failed bold stage: 1 retry it boldly, 2 fall back to the careful way, 3 give the programme up.</summary>
        public static Command RecoverStage(int labId, int choice)
        {
            Command c = Make(CommandKind.RecoverStage, labId);
            c.Count = choice;
            return c;
        }

        /// <summary>Gives the programme up at any point and takes part of the points back.</summary>
        public static Command AbandonProgramme(int labId)
        {
            return Make(CommandKind.AbandonProgramme, labId);
        }

        /// <summary>Sets the priority of a training run: 1 background, 2 normal, 3 rush. Changing it costs the run part of its progress.</summary>
        public static Command SetRunPriority(int labId, int modelId, int level)
        {
            Command c = Make(CommandKind.SetRunPriority, labId);
            c.TargetId = modelId;
            c.Count = level;
            return c;
        }

        /// <summary>Forms a project pod of the lab's own people (by staff id). It costs the data's cash for each person.</summary>
        public static Command FormPod(int labId, params int[] staffIds)
        {
            Command c = Make(CommandKind.FormPod, labId);
            c.Ids = new System.Collections.Generic.List<string>();
            for (int i = 0; i < staffIds.Length; i++) c.Ids.Add(staffIds[i].ToString());
            return c;
        }

        /// <summary>Swaps one member of the pod (TargetId) for another person (Count). The pod's weeks together are halved.</summary>
        public static Command SwapPodMember(int labId, int outId, int inId)
        {
            Command c = Make(CommandKind.SwapPodMember, labId);
            c.TargetId = outId;
            c.Count = inId;
            return c;
        }

        /// <summary>Disbands the pod.</summary>
        public static Command DisbandPod(int labId)
        {
            return Make(CommandKind.DisbandPod, labId);
        }

        /// <summary>Files for a licence under a regime that is open this era (1 registry, 2 licence, 3 audit regime). The fee is paid now and is lost if the application is refused.</summary>
        public static Command ApplyLicence(int labId, int regime)
        {
            Command c = Make(CommandKind.ApplyLicence, labId);
            c.Count = regime;
            return c;
        }

        /// <summary>Renews the licence in the last weeks of its term: the fee again and the same alignment bar. Clears the strikes.</summary>
        public static Command RenewLicence(int labId)
        {
            return Make(CommandKind.RenewLicence, labId);
        }

        /// <summary>Gives the licence (or the application) up. Nothing is refunded.</summary>
        public static Command SurrenderLicence(int labId)
        {
            return Make(CommandKind.SurrenderLicence, labId);
        }

        /// <summary>Promises to keep supporting a released model for this many weeks. It then cannot be replaced in a sector or given notice until the pledge ends or is broken.</summary>
        public static Command PledgeSupport(int labId, int modelId, int weeks)
        {
            Command c = Make(CommandKind.PledgeSupport, labId);
            c.TargetId = modelId;
            c.Count = weeks;
            return c;
        }

        /// <summary>Breaks a support pledge now, for a fee, trust in the model's sectors and some reputation.</summary>
        public static Command BreakSupportPledge(int labId, int modelId)
        {
            Command c = Make(CommandKind.BreakSupportPledge, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Opens a released model's API: customers start building on it. Costs upkeep every week and lifts the model's revenue as integrations grow.</summary>
        public static Command OpenApi(int labId, int modelId)
        {
            Command c = Make(CommandKind.OpenApi, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Starts to retire a model's API with a deadline in weeks. The successor is a model of the lab with an open API, or 0 for none.</summary>
        public static Command DeprecateApi(int labId, int modelId, int successorId, int weeks)
        {
            Command c = Make(CommandKind.DeprecateApi, labId);
            c.TargetId = modelId;
            c.Count = weeks;
            c.Id = successorId.ToString();
            return c;
        }

        /// <summary>Calls off a deprecation. What was already lost stays lost.</summary>
        public static Command CancelDeprecation(int labId, int modelId)
        {
            Command c = Make(CommandKind.CancelDeprecation, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Starts a conformity assessment of a released model. Scope 1 is narrow, 2 is broad. Compiles a dossier first, for research points.</summary>
        public static Command StartConformity(int labId, int modelId, int scope)
        {
            Command c = Make(CommandKind.StartConformity, labId);
            c.TargetId = modelId;
            c.Count = scope;
            return c;
        }

        /// <summary>Hands the assessor the evidence. The model needs a live red-team result, and for a broad scope a live external audit too.</summary>
        public static Command SubmitEvidence(int labId, int modelId)
        {
            Command c = Make(CommandKind.SubmitEvidence, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Drops the assessment, or gives up the certificate. Nothing is refunded.</summary>
        public static Command WithdrawConformity(int labId, int modelId)
        {
            Command c = Make(CommandKind.WithdrawConformity, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Organises the lab: 1 flat, 2 by function, 3 in cross-functional squads. Changing it disrupts the lab for a while and cannot be repeated soon.</summary>
        public static Command SetOrg(int labId, int form)
        {
            Command c = Make(CommandKind.SetOrg, labId);
            c.Count = form;
            return c;
        }

        /// <summary>Orders the next tier of the training-cluster fabric. A deposit now, the rest at delivery; the cluster is down for the last weeks.</summary>
        public static Command OrderFabric(int labId)
        {
            return Make(CommandKind.OrderFabric, labId);
        }

        /// <summary>Cancels the tier being built. The deposit is lost.</summary>
        public static Command CancelFabric(int labId)
        {
            return Make(CommandKind.CancelFabric, labId);
        }

        /// <summary>Joins the incident register: every incident must be reported within weeks, and inspections look at the findings.</summary>
        public static Command JoinRegister(int labId)
        {
            return Make(CommandKind.JoinRegister, labId);
        }

        /// <summary>Leaves the register. Not while a report is waiting or being prepared.</summary>
        public static Command LeaveRegister(int labId)
        {
            return Make(CommandKind.LeaveRegister, labId);
        }

        /// <summary>Files the oldest waiting report: 1 a summary now (leaves a finding), 2 a full report (weeks of safety people's time; clears a finding).</summary>
        public static Command FileReport(int labId, int depth)
        {
            Command c = Make(CommandKind.FileReport, labId);
            c.Count = depth;
            return c;
        }

        /// <summary>Brings in raw data: 1 scraped from the web, 2 licensed archives, 3 synthetic (needs a model on sale). Units are in <paramref name="units"/>.</summary>
        public static Command AcquireData(int labId, int kind, int units)
        {
            Command c = Make(CommandKind.AcquireData, labId);
            c.TargetId = kind;
            c.Count = units;
            return c;
        }

        /// <summary>Starts a cleaning job on raw data of one source. One job at a time.</summary>
        public static Command CleanData(int labId, int kind, int units)
        {
            Command c = Make(CommandKind.CleanData, labId);
            c.TargetId = kind;
            c.Count = units;
            return c;
        }

        /// <summary>Licenses scraped units after the fact, so a claim can no longer take them.</summary>
        public static Command LicenseScraped(int labId, int units)
        {
            Command c = Make(CommandKind.LicenseScraped, labId);
            c.Count = units;
            return c;
        }

        /// <summary>Sets what the lab does with its users' data: 0 none, 1 opt-in, 2 on by default. Moving off a default policy deletes most of what was collected.</summary>
        public static Command SetUserData(int labId, int policy)
        {
            Command c = Make(CommandKind.SetUserData, labId);
            c.Count = policy;
            return c;
        }

        /// <summary>Changes how the company is controlled: 1 dual-class shares, 2 a non-profit parent. Each is permanent, only early, and costs cash from later rounds.</summary>
        public static Command SetGovernance(int labId, int form)
        {
            Command c = Make(CommandKind.SetGovernance, labId);
            c.Count = form;
            return c;
        }

        /// <summary>Starts the search for an independent director. One search at a time.</summary>
        public static Command SeatDirector(int labId)
        {
            return Make(CommandKind.SeatDirector, labId);
        }

        /// <summary>Pays the investors of a funding round the lab has raised to leave, giving the founders back that round's dilution. Id is the round.</summary>
        public static Command BuyBackRound(int labId, string roundId)
        {
            Command c = Make(CommandKind.BuyBackRound, labId);
            c.Id = roundId;
            return c;
        }

        /// <summary>Runs a share of the rented GPUs as spot capacity (permille): cheaper rent, and training runs may be preempted.</summary>
        public static Command SetSpotShare(int labId, int permille)
        {
            Command c = Make(CommandKind.SetSpotShare, labId);
            c.Permille = permille;
            return c;
        }

        /// <summary>Sets how much freedom a released model gives its agents: 0 none, 1 supervised, 2 delegated, 3 autonomous. TargetId is the model.</summary>
        public static Command SetAutonomy(int labId, int modelId, int level)
        {
            Command c = Make(CommandKind.SetAutonomy, labId);
            c.TargetId = modelId;
            c.Count = level;
            return c;
        }

        /// <summary>Contracts an annotation vendor for a term: 1 volume, 2 licensed, 3 expert. Count is the vendor.</summary>
        public static Command ContractAnnotators(int labId, int vendor)
        {
            Command c = Make(CommandKind.ContractAnnotators, labId);
            c.Count = vendor;
            return c;
        }

        /// <summary>Ends the vendor contract early, paying most of the fees still to run.</summary>
        public static Command EndAnnotators(int labId)
        {
            return Make(CommandKind.EndAnnotators, labId);
        }

        /// <summary>Audits the vendor, taking most of the hidden exposure away.</summary>
        public static Command AuditVendor(int labId)
        {
            return Make(CommandKind.AuditVendor, labId);
        }

        /// <summary>Starts tuning a trained model: Count 1 preference tuning, 2 safety tuning. TargetId is the model.</summary>
        public static Command PostTrain(int labId, int modelId, int kind)
        {
            Command c = Make(CommandKind.PostTrain, labId);
            c.TargetId = modelId;
            c.Count = kind;
            return c;
        }

        /// <summary>Stops the tuning run of a model. The data it used is gone.</summary>
        public static Command AbandonPostTraining(int labId, int modelId)
        {
            Command c = Make(CommandKind.AbandonPostTraining, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Names the person who owns a model. TargetId is the model, Count the staff id; Count 0 takes the owner away.</summary>
        public static Command AssignOwner(int labId, int modelId, int staffId)
        {
            Command c = Make(CommandKind.AssignOwner, labId);
            c.TargetId = modelId;
            c.Count = staffId;
            return c;
        }

        /// <summary>Writes the next level of documentation for a model. TargetId is the model.</summary>
        public static Command WriteDocs(int labId, int modelId)
        {
            Command c = Make(CommandKind.WriteDocs, labId);
            c.TargetId = modelId;
            return c;
        }

        /// <summary>Starts (Count 1) or stops (Count 0) holding board meetings.</summary>
        public static Command HoldBoardMeetings(int labId, int on)
        {
            Command c = Make(CommandKind.HoldBoardMeetings, labId);
            c.Count = on;
            return c;
        }

        /// <summary>Answers the motion on the table: Count 1 accepts it as a commitment, 0 declines it.</summary>
        public static Command AnswerMotion(int labId, int accept)
        {
            Command c = Make(CommandKind.AnswerMotion, labId);
            c.Count = accept;
            return c;
        }

        /// <summary>Starts an incident drill at the lab.</summary>
        public static Command StartDrill(int labId)
        {
            return Make(CommandKind.StartDrill, labId);
        }

        /// <summary>Registers a released model on a public eval suite: 1 narrow, 2 broad. TargetId is the model.</summary>
        public static Command RegisterEval(int labId, int modelId, int suite)
        {
            Command c = Make(CommandKind.RegisterEval, labId);
            c.TargetId = modelId;
            c.Count = suite;
            return c;
        }

        /// <summary>Sets the agent tool mask on a released model: Count 0 to 7 (bits 1 web, 2 code, 4 data). TargetId is the model.</summary>
        public static Command SetToolPolicy(int labId, int modelId, int mask)
        {
            Command c = Make(CommandKind.SetToolPolicy, labId);
            c.TargetId = modelId;
            c.Count = mask;
            return c;
        }

        /// <summary>Staffs the desk to drain up to Count items this week.</summary>
        public static Command StaffHumanQueue(int labId, int count)
        {
            Command c = Make(CommandKind.StaffHumanQueue, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Pays to clear Count items from the queue now.</summary>
        public static Command ClearHumanQueue(int labId, int count)
        {
            Command c = Make(CommandKind.ClearHumanQueue, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Signs a checklist item on a trained model: 1 red-team, 2 docs, 4 eval, 8 policy. TargetId is the model.</summary>
        public static Command SignChecklistItem(int labId, int modelId, int item)
        {
            Command c = Make(CommandKind.SignChecklistItem, labId);
            c.TargetId = modelId;
            c.Count = item;
            return c;
        }

        public static Command StartSandbox(int labId, int modelId)
        {
            Command c = Make(CommandKind.StartSandbox, labId);
            c.TargetId = modelId;
            return c;
        }

        public static Command SetPlaybook(int labId, int level)
        {
            Command c = Make(CommandKind.SetPlaybook, labId);
            c.Count = level;
            return c;
        }

        public static Command ArmPlaybook(int labId)
        {
            return Make(CommandKind.ArmPlaybook, labId);
        }

        public static Command SetOrchBudget(int labId, int level)
        {
            Command c = Make(CommandKind.SetOrchBudget, labId);
            c.Count = level;
            return c;
        }

        public static Command SetRouteShare(int labId, int modelId, int ppm)
        {
            Command c = Make(CommandKind.SetRouteShare, labId);
            c.TargetId = modelId;
            c.Count = ppm;
            return c;
        }

        public static Command PublishMetrics(int labId, int modelId)
        {
            Command c = Make(CommandKind.PublishMetrics, labId);
            c.TargetId = modelId;
            return c;
        }

        public static Command StopMetrics(int labId, int modelId)
        {
            Command c = Make(CommandKind.StopMetrics, labId);
            c.TargetId = modelId;
            return c;
        }

        public static Command BranchSpecialist(int labId, int modelId, string kindId)
        {
            Command c = Make(CommandKind.BranchSpecialist, labId);
            c.TargetId = modelId;
            c.Id = kindId;
            return c;
        }

        public static Command AcquireDataSource(int labId, string sourceId)
        {
            Command c = Make(CommandKind.AcquireDataSource, labId);
            c.Id = sourceId;
            return c;
        }

        public static Command RenewDataSource(int labId, string sourceId)
        {
            Command c = Make(CommandKind.RenewDataSource, labId);
            c.Id = sourceId;
            return c;
        }

        public static Command SetWeightsPolicy(int labId, int modelId, int policy)
        {
            Command c = Make(CommandKind.SetWeightsPolicy, labId);
            c.TargetId = modelId;
            c.Permille = policy;
            return c;
        }

        public static Command SetSectorPrice(int labId, string sectorId, int tier)
        {
            Command c = Make(CommandKind.SetSectorPrice, labId);
            c.Id = sectorId;
            c.Permille = tier;
            return c;
        }

        public static Command SetCampaignDeadline(int labId, int deadlineTick)
        {
            Command c = Make(CommandKind.SetCampaignDeadline, labId);
            c.Permille = deadlineTick;
            return c;
        }

        public static Command ShapeLaw(int labId, string blocId, string goal)
        {
            Command c = Make(CommandKind.ShapeLaw, labId);
            c.Id = blocId;
            c.Ids = new List<string> { goal };
            return c;
        }

        public static Command RespondTakeoff(int labId, string choiceId)
        {
            Command c = Make(CommandKind.RespondTakeoff, labId);
            c.Id = choiceId;
            return c;
        }

        public static Command PickScenarioBaggage(int labId, string baggageId)
        {
            Command c = Make(CommandKind.PickScenarioBaggage, labId);
            c.Id = baggageId;
            return c;
        }

        /// <summary>Pays cash or sells GPUs to clear a margin call. Count sells that many units; pay uses Permille as cash when set.</summary>
        public static Command SatisfyMarginCall(int labId, int gpuCount, int cash = 0)
        {
            Command c = Make(CommandKind.SatisfyMarginCall, labId);
            c.Count = gpuCount;
            c.Permille = cash;
            return c;
        }

        public static Command RespondRungSix(int labId, string choiceId)
        {
            Command c = Make(CommandKind.RespondRungSix, labId);
            c.Id = choiceId;
            return c;
        }

        public static Command SetLawTarget(int labId, string goal)
        {
            Command c = Make(CommandKind.SetLawTarget, labId);
            c.Id = goal;
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
        UnknownLab, LabBankrupt, LabAbsorbed, UnknownId, InvalidArgument,
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
        BelowEraSafetyBar, OfferGone, InquiryGone, ComplaintGone, BelowReleaseGate, GateLocked, LeadGone, NoOfficeEvent, LobbyCooling, PivotSpent, LoanLimit, TeamTooTired, Uninsurable, InsuranceLocked, PayLocked, SubletLocked, FocusLocked, HackLocked, WouldLoseControl, EthicsLocked, BoardVeto, CampaignLocked, OnCallLocked, ConsultantsLocked, SlaLocked, GrantLocked, BountyLocked, PledgeLocked
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
