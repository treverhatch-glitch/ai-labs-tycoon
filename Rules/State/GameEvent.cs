using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum EventKind
    {
        RoomBuilt, RoomDemolished, LotBought,
        Hired, Fired,
        GpusRented, GpusBought, GpusSold,
        TechResearched,
        TrainingStarted, TrainingFinished, ModelReleased, ModelRetired,
        Incident,
        GauntletCrown, GauntletBracket, GauntletSafest,
        GrantEnded, InDebt, Bankrupt,
        EraBegan, WorldNews,
        LandBought, LandSold, ColoLeased, ColoEnded,
        HallStarted, HallBuilt, TownVoteDelayed, PowerDealStarted, PowerDealReady,
        HardwareMoved, Heatwave,
        AttentionRose, AttentionFell,
        /// <summary>The vendor's yearly keynote. Ref is the generation, Amount the month's supply after it.</summary>
        Keynote,
        /// <summary>A lab joined the race. Ref is the rival's config id, Amount its starting cash.</summary>
        RivalJoined,
        /// <summary>A hall opened within a few weeks of its start.</summary>
        FastBuild,
        /// <summary>A plan-run lab's board had a fit and its plan pauses. Amount is the weeks.</summary>
        BoardDrama,
        /// <summary>A design went to the foundry. Subject is the design, Amount the tape-out cost.</summary>
        ChipTapedOut,
        /// <summary>A tape-out came back with a bug. Amount is what the respin costs.</summary>
        ChipRespin,
        /// <summary>A design works. Subject is the design, Amount its starting yield. Pick is 1 for the lab's first.</summary>
        ChipReady,
        /// <summary>Chips arrived. Subject is the design, Amount the chips that came, Ref the holding's site.</summary>
        ChipsDelivered,
        /// <summary>A delivery lost most of its chips to a yield crash. Amount is the chips that survived.</summary>
        ChipYieldCrash,
        /// <summary>Chips were bought from another lab's list. LabId is the buyer, Subject the seller's lab, Amount the count.</summary>
        ChipsBought,
        EndingReached,
        ShadyOpStarted, ShadyOpResolved, ShadyTraced, ShadyInvestigation, ShadyConvicted, ShadyAcquitted, ShadySideJob, ShadyTemptation,
        StaffPoached,
        /// <summary>The vendor could not fulfil a GPU purchase queue. Amount is GPUs left; Ref holds the reason tag.</summary>
        GpuQueueStalled,
        ChinaDealOffered, ChinaDealAccepted, ChinaDealDeclined,
        /// <summary>Closed a funding round. Ref is the round id; Pick is 1 when terms were a down round.</summary>
        FundingClosed,
        /// <summary>Board update after venture capital. Amount is reputation gained.</summary>
        InvestorBoardVisit,
        LabAcquired,
        ModelSold,
        /// <summary>A rival made an offer to one of the player's people. LabId is the rival, SubjectId the player's lab, Amount the package, Ref the role.</summary>
        PoachOffered,
        /// <summary>The player answered an offer. LabId is the player's lab, SubjectId the rival, Amount what the counter cost, Ref the counter, Pick 1 if the person stayed.</summary>
        PoachCountered,
        /// <summary>The player tried to poach from a rival. LabId is the player's lab, SubjectId the rival, Amount what it cost, Ref the role, Pick 1 if it worked.</summary>
        PoachBackTried,
        /// <summary>A journalist asked the player about a beat. LabId is the player's lab, SubjectId the inquiry, Amount the reputation at play, Ref "beat|journalist|what".</summary>
        PressInquiry,
        /// <summary>The player answered a journalist. LabId is the player's lab, SubjectId the inquiry, Amount the reputation moved (negative if it backfired), Ref the response, Pick 1 if it worked.</summary>
        PressAnswered,
        /// <summary>The player leaked a rival's incident. LabId is the player's lab, SubjectId the rival, Amount the reputation the rival lost (or the leaker lost, if it was traced), Ref the model, Pick 1 if it ran.</summary>
        StoryLeaked,
        /// <summary>A launch earned reputation from the press room. LabId is the lab, SubjectId the model, Amount the reputation points.</summary>
        LaunchBuzz,
        /// <summary>A lab paid to tune its entries for the Gauntlet. LabId is the lab, Amount the bill, Pick 1 if it was paid and the tuning went in, 0 if the lab could not afford it.</summary>
        GauntletTuned,
        /// <summary>A rival exposed a tuned win. LabId is the lab, SubjectId how many entries lost their lift, Amount the reputation lost.</summary>
        GauntletExposed,
        /// <summary>A joke award with a real effect. LabId is the winner, Ref the award id, Amount the effect (reputation, or morale for the newcomers).</summary>
        GauntletAward,
        /// <summary>The live demo of a lab's crown entry. LabId is the lab, SubjectId the model, Amount the reputation moved, Pick 1 for a triumph and 0 for a meme.</summary>
        GauntletDemo,
        /// <summary>An incident cost a lab the trust of one sector's customers. LabId is the lab, Ref the sector, Amount the trust lost. Logged for the player's lab only.</summary>
        TrustLost,
        /// <summary>A customer let down by an incident wrote to the player's desk. LabId is the player's lab, SubjectId the complaint, Amount the trust at stake, Ref "sector|model".</summary>
        CustomerComplaint,
        /// <summary>The player answered a complaint. LabId is the player's lab, SubjectId the complaint, Amount the trust moved (negative if it backfired), Ref "response|sector", Pick 1 if it worked.</summary>
        ComplaintAnswered,
        /// <summary>A room slipped into a worse stage of age as a new era began. LabId is the lab, SubjectId the room, Amount its age in eras, Ref the room type.</summary>
        RoomDated,
        /// <summary>The player refurbished a room. LabId is the lab, SubjectId the room, Amount the cost, Ref the room type.</summary>
        RoomRefurbished,
        /// <summary>A player lab's safety techs lost more ground as a new era began. LabId is the lab, Amount the permille of their bonus now lost, Ref the lab's newest safety tech.</summary>
        ControlStale,
        /// <summary>New safety research won back ground. LabId is the lab, Amount the permille won back, Ref the tech.</summary>
        ControlRefreshed,
        /// <summary>A trained model came out far better than aimed for. LabId is the lab, SubjectId the model, Amount the luck in permille, Ref the model name.</summary>
        LeadFound,
        /// <summary>An investigation ended. LabId is the lab, SubjectId the lead, Amount what it gave (points or permille), Ref the outcome id, Pick 1 when something came of it.</summary>
        LeadResolved,
        /// <summary>A lead nobody followed went cold. LabId is the lab, SubjectId the lead, Ref the model name.</summary>
        LeadWentCold,
        /// <summary>The player paid to fine-tune a glitching model. LabId is the lab, SubjectId the model, Amount the cost, Ref the model name.</summary>
        ModelFineTuned,
        /// <summary>The player withdrew a glitching model and put the older version back on sale. LabId is the lab, SubjectId the older model now on sale, Amount the reputation lost, Ref the withdrawn model's name.</summary>
        ModelRolledBack,
        /// <summary>Something happened at the player's lab that wants an answer. LabId is the lab, Ref the office event id.</summary>
        OfficeEventPosed,
        /// <summary>The player answered. LabId is the lab, Ref "event|choice".</summary>
        OfficeEventAnswered,
        /// <summary>Nobody answered in time and it blew over. LabId is the lab, Ref the office event id.</summary>
        OfficeEventLapsed,
        /// <summary>The player chose what a model in training is for. SubjectId is the model, Ref the objective id (empty for the default).</summary>
        ObjectiveSet,
        /// <summary>A model was put to work on its successor's design. SubjectId is the successor, Amount the helper's id, Ref the successor's name.</summary>
        SuccessorDesigned,
        /// <summary>A model was put on sale with some testing skipped. SubjectId is the model, Amount the weeks skipped, Ref its name.</summary>
        ModelRushed,
        /// <summary>The player lobbied a government. Amount is what it cost, Ref is "bloc|goal|result" (result: landed, missed or backfired).</summary>
        Lobbied,
        /// <summary>The lab changed its style. SubjectId is how many people left, Amount what it cost, Ref "from|to" style ids (from is empty for none).</summary>
        StylePivoted,
        /// <summary>The player's lab was marked against an era's goals as the era ended. SubjectId is the goals met, Amount the points, Ref the era id.</summary>
        EraScored,
        /// <summary>A training run set itself back. SubjectId is the model, Amount the compute thrown away, Ref the model's name.</summary>
        TrainingFailed,
        /// <summary>The player set up a run. SubjectId is the model, Ref "approach|id" or "checkpoints|id" (empty id for the default).</summary>
        RunSetUp,
        /// <summary>Somebody quit by blog post. SubjectId is the person, Amount the reputation it cost, Ref "role|trait".</summary>
        StaffQuit,
        /// <summary>The player borrowed against chips. SubjectId is the loan, Amount the principal.</summary>
        LoanTaken,
        /// <summary>A loan was paid off. SubjectId is the loan, Amount what was paid (early) or the principal (term), Ref "early" or "term".</summary>
        LoanRepaid,
        /// <summary>A payment was missed (the first of a run). SubjectId is the loan, Amount the payment.</summary>
        LoanMissed,
        /// <summary>The lender took chips. SubjectId is how many GPUs, Amount what was still owed after they were sold, Ref the loan id.</summary>
        LoanDefaulted,
        /// <summary>The foundry gave an order's slot to someone else. Subject is the design, Amount the weeks the delivery slipped, Ref the order id.</summary>
        ChipSlotSnubbed,
        /// <summary>Crunch time ended. Amount is how many weeks it lasted, Ref is why: "stopped", "tired" (morale gave out), "spent" (the longest crunch) or "empty" (no one left). Pick is 1 for "tired", 0 for "stopped" and -1 for the other two, for chat lines.</summary>
        CrunchEnded,
        /// <summary>The insurer paid part of an incident's fine. Subject is the model, Amount what it paid, Ref the model's name.</summary>
        InsuranceClaimPaid,
        /// <summary>The insurer ended the policy after too many claims. Amount is how many it remembered.</summary>
        InsuranceDropped,
        /// <summary>The team went on an offsite. Subject is how many people it lifted, Amount what it cost.</summary>
        OffsiteHeld,
        /// <summary>The player paid for a red team to attack a model. LabId is the lab, SubjectId the model, Amount the cost, Ref the model name.</summary>
        RedTeamRan,
        /// <summary>An interpretability programme closed the last of a model's alignment gap. LabId is the lab, SubjectId the model, Amount the points the gap was when the programme started on it this time, Ref the model name.</summary>
        GapFound,
        /// <summary>The player changed the pay policy. LabId is the lab, SubjectId the new level, Amount the weekly salary bill now. Pick is 1 for a raise and 0 for a cut.</summary>
        PayChanged,
        /// <summary>The compliance office lost its policy staff and fell to a lower level. LabId is the lab, SubjectId the level it now runs at, Amount the level it had.</summary>
        ComplianceLapsed,
        /// <summary>The player published a paper. LabId is the lab, SubjectId the prestige now, Amount the research points it cost. Pick is always 1.</summary>
        PaperPublished,
        /// <summary>The player changed the share of compute let to outside customers. LabId is the lab, SubjectId the new share in permille, Amount what it earns in a week at today's fleet.</summary>
        SubletChanged,
        /// <summary>Outside auditors tested a model. LabId is the lab, SubjectId the model, Amount the fee, Ref the model's name. Pick is 1 when the numbers held up and a certificate was given, 0 when the published score embarrassed the lab, -1 for anything in between.</summary>
        AuditDone,
        /// <summary>An incident pulled a model's audit certificate. LabId is the lab, SubjectId the model, Ref its name.</summary>
        AuditRevoked,
        /// <summary>The player changed the lab's research focus. LabId is the lab, Amount the cost, Ref the focus id (empty when dropped). Pick is 1 for a new focus, 0 for none.</summary>
        FocusChanged,
        /// <summary>A hack week started or ended. LabId is the lab, SubjectId its length in weeks, Amount the cost (0 at the end). Pick is 1 when it starts and 0 when it ends.</summary>
        HackWeek,
        /// <summary>The player gave the team another level of equity. LabId is the lab, SubjectId the level now, Amount the share of the company the people own, in permille.</summary>
        EquityGranted,
        /// <summary>The player moved a person to another role. LabId is the lab, SubjectId the person, Amount the fee, Ref "oldRole>newRole". Pick is 1 inside a function and 0 across functions.</summary>
        StaffRetrained,
        /// <summary>The player named or stood down a chief. LabId is the lab, SubjectId the person, Amount the fee, Ref the title ("scientist" or "safety"). Pick is 1 when named, 0 when stood down.</summary>
        ChiefChanged,
        /// <summary>A chief left the payroll or their field and the title fell empty. LabId is the lab, Amount the morale the team lost, Ref the title.</summary>
        ChiefLeft,
        /// <summary>The player seated or stepped down the ethics board. LabId is the lab, SubjectId the level now, Amount the cost. Pick is 1 when the level went up, 0 when it went down.</summary>
        EthicsBoardChanged,
        /// <summary>The ethics board walked out because the safety staff behind it were gone. LabId is the lab, SubjectId the level it fell to, Amount the level it had.</summary>
        EthicsBoardResigned,
        /// <summary>The player set the inference optimisation level. LabId is the lab, SubjectId the level now, Amount the cost. Pick is 1 when the level went up, 0 when it went down.</summary>
        OptimisationSet,
        /// <summary>A marketing campaign started or ended. LabId is the lab, SubjectId the budget level, Amount the cost (0 at the end), Ref the sector. Pick is 1 when it starts, 0 when it ends, 2 when an incident cut it short.</summary>
        CampaignRun,
        /// <summary>The player changed the on-call rota. LabId is the lab, SubjectId the level now. Pick is 1 when the level went up, 0 when it went down.</summary>
        OnCallSet,
        /// <summary>A firm of policy consultants came in or left. LabId is the lab, SubjectId the level, Amount the fee (0 when they leave). Pick is 1 when they start, 0 when they leave.</summary>
        ConsultantsHired,
        /// <summary>The player published a post-mortem of a model's last incident. LabId is the lab, SubjectId the model, Amount the reputation won back, Ref the model's name. Pick is 0 for a summary, 1 for a full write-up.</summary>
        PostMortemPublished,
        /// <summary>The player changed the service-level pledge, or a run of broken weeks dropped it. LabId is the lab, SubjectId the level now. Pick is 1 when the level went up, 0 when it went down.</summary>
        SlaPledged,
        /// <summary>A week broke the service-level pledge. LabId is the lab, SubjectId the level, Amount the refund. Pick is 0 for too little serving compute, 1 for an incident.</summary>
        SlaBroken
    }

    /// <summary>
    /// Something that happened, as data. The rules never write prose into the state: a screen (or
    /// <c>EventText</c>) turns an event into a line, so text can be rewritten or translated freely.
    /// </summary>
    public sealed class GameEvent : ISync
    {
        public int Tick;
        public EventKind Kind;
        public int LabId;
        /// <summary>The model, room or other thing the event is about, or -1.</summary>
        public int SubjectId;
        public long Amount;
        /// <summary>A pick from a config list (which headline an incident got), or 0.</summary>
        public int Pick;
        /// <summary>A config id or a name the event refers to (a tech, a sector, a model name), or empty.</summary>
        public string Ref;
        /// <summary>For an incident: the risk class it was (an id from <c>safety.riskClasses</c>), or empty for the era's plain mishap.</summary>
        public string RiskClass = "";

        public void Sync(Syncer s)
        {
            s.Int("tick", ref Tick);
            s.Enum("kind", ref Kind);
            s.Int("labId", ref LabId);
            s.Int("subjectId", ref SubjectId);
            s.Long("amount", ref Amount);
            s.Int("pick", ref Pick);
            s.Str("ref", ref Ref);
            s.Str("riskClass", ref RiskClass);
        }
    }
}
