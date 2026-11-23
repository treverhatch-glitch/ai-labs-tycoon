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
        SlaBroken,
        /// <summary>The player applied for a research grant. LabId is the lab, SubjectId the tier, Amount the application fee. Pick is 1.</summary>
        GrantApplied,
        /// <summary>The panel ruled on a research grant. LabId is the lab, SubjectId the tier, Amount the award (0 when declined). Pick is 1 when awarded, 0 when declined.</summary>
        GrantDecided,
        /// <summary>A research grant was settled. LabId is the lab, SubjectId the tier, Amount the cash paid back. Pick is 1 when the papers were delivered, 0 when they were not.</summary>
        GrantSettled,
        /// <summary>The player scrapped a training run. LabId is the lab, SubjectId the model's id, Amount the research points kept, Ref the model's name. Pick is 1 when the run was at least half done, 0 when it was less.</summary>
        RunScrapped,
        /// <summary>A layoff round. Amount is the people let go, SubjectId the share of the staff in permille, Pick 1 when it was a deep cut.</summary>
        LayoffRound,
        /// <summary>The lab paid down technical debt. Amount is the points retired, SubjectId the debt left, Pick 1 when none is left.</summary>
        DebtPaid,
        /// <summary>A person was promoted to senior. SubjectId is the person, Amount the fee, Ref the role id.</summary>
        StaffPromoted,
        /// <summary>The lab committed to a revenue target. SubjectId is the level, Amount the goal.</summary>
        TargetSet,
        /// <summary>A revenue target was judged. SubjectId is the level, Amount the revenue made, Pick 1 when it was met.</summary>
        TargetSettled,
        /// <summary>The lab endowed a university chair. SubjectId is the chairs it now holds, Amount the cost.</summary>
        ChairEndowed,
        /// <summary>A safety person raised a concern that a model shows more alignment than it has. SubjectId is the model, Amount the gap in permille.</summary>
        WhistleRaised,
        /// <summary>The concern was answered. SubjectId is the model, Amount what it cost, Pick 0 when the lab came clean and 1 when it paid for silence.</summary>
        WhistleAnswered,
        /// <summary>The concern leaked. SubjectId is the model, Amount the reputation lost, Pick 1 when a person resigned over it.</summary>
        WhistleLeaked,
        /// <summary>The lab changed its bug bounty. SubjectId is the new level, Amount the fee, Pick 1 when it was raised.</summary>
        BountySet,
        /// <summary>The lab pulled a model out of a sector. SubjectId is the model, Amount the break fee, Pick 1 when it was the last sector and the model was retired. Ref is the sector.</summary>
        SectorPulledOut,
        /// <summary>A loan was restructured. SubjectId is the loan, Amount the fee added to what is owed, Pick 1 when it cleared missed payments.</summary>
        LoanRestructured,
        /// <summary>Cash moved into or out of the treasury reserve by the player. Amount is the sum moved before any fee, Pick 1 when set aside and 0 when drawn.</summary>
        ReserveMoved,
        /// <summary>The reserve paid out to cover a debt. Amount is the sum taken from the reserve before the fee.</summary>
        ReserveCovered,
        /// <summary>The lab changed its safety pledge. SubjectId is the new level, Amount the reputation lost walking it back, Pick 1 when it was raised.</summary>
        PledgeSet,
        /// <summary>A launch broke the pledge. SubjectId is the model, Amount the alignment the pledge asked, Pick the level that was broken.</summary>
        PledgeBroken,
        /// <summary>The lab announced a roadmap. SubjectId is the level, Amount the capability promised, Pick the weeks it has.</summary>
        RoadmapAnnounced,
        /// <summary>A roadmap was settled. SubjectId is the level, Amount the capability promised, Pick 1 when a model delivered it in time.</summary>
        RoadmapSettled,
        /// <summary>A senior started mentoring a junior. SubjectId is the junior, Amount the mentor, Ref the junior's role.</summary>
        MentoringStarted,
        /// <summary>A pair ended. SubjectId is the junior, Amount the weeks it held, Ref the role, Pick 0 when the junior graduated, 1 when it broke up, 2 when the player called it off.</summary>
        MentoringEnded,
        /// <summary>A skunkworks project started. SubjectId is the kind (1 research, 2 safety, 3 engineering), Amount the people, Pick the weeks, Ref the kind.</summary>
        SkunkworksStarted,
        /// <summary>A project paid out. SubjectId is the kind, Amount what it paid (research points, alignment permille or debt points), Pick 1 when it ran its course, 0 when it was wrapped up early.</summary>
        SkunkworksDone,
        /// <summary>The staff started to organise. Amount is the pressure, SubjectId the weeks the player has to answer.</summary>
        LabourDrive,
        /// <summary>A union was recognised. Pick 1 when the player chose it, 0 when a strike forced it. Amount is the pay it adds in permille.</summary>
        LabourRecognised,
        /// <summary>The player resisted a drive. Pick 1 when chosen, 0 when the window ran out. Amount is the legal fees.</summary>
        LabourResisted,
        /// <summary>A resisted drive collapsed because pressure was back under the drive level. Amount is the pressure left.</summary>
        LabourHeld,
        /// <summary>The staff went on strike. Amount is the weeks, SubjectId the morale lost.</summary>
        LabourStrike,
        /// <summary>An end-of-life notice was given. SubjectId is the model, Amount the weeks, Pick 1 when the successor was designed with its help, Ref the model.</summary>
        SunsetAnnounced,
        /// <summary>A model left the market. SubjectId is the successor, Amount the sectors that moved over, Pick 1 when none was dropped, Ref the model.</summary>
        SunsetDone,
        /// <summary>A notice was withdrawn. SubjectId is the model, Amount the reputation it cost, Pick 1 when the player called it off and 0 when the successor was gone, Ref the model.</summary>
        SunsetCalledOff,
        /// <summary>An account team changed. Ref is the sector name, SubjectId the weeks of familiarity left, Amount the people now on it. Pick 1 for a new team, 0 for a changed one, 2 when it was disbanded, 3 when losing salespeople thinned it and 4 when the lab lost the sector.</summary>
        AccountTeamChanged,
        /// <summary>A paper went out for review. SubjectId is the venue (1 workshop, 2 main, 3 flagship), Amount the research points, Pick the effort, Ref the venue name.</summary>
        PaperSubmitted,
        /// <summary>A paper was decided. SubjectId is the venue, Amount the prestige it added (accepted) or the points given back (rejected or withdrawn), Pick 1 accepted, 0 rejected, 2 withdrawn, Ref the venue name.</summary>
        PaperDecided,
        /// <summary>A former employee came back. SubjectId is their old id, Amount the fee, Pick how they had left (1 good terms, 0 laid off, -1 quit), Ref the role id.</summary>
        AlumniRehired,
        /// <summary>A cohort of interns began. Amount is how many, Pick the weeks, Ref the role id.</summary>
        InternshipStarted,
        /// <summary>A cohort ended. SubjectId is how many there were, Amount how many were taken on, Pick 1 when it ran its course and 0 when it was cut short, Ref the role id.</summary>
        InternshipsEnded,
        /// <summary>The support desk was staffed or closed. Amount is the people on it now.</summary>
        SupportDeskSet,
        /// <summary>The backlog crossed its limit. Amount is the backlog, Pick 1 when it went over and trust began to suffer and 0 when it came back under.</summary>
        SupportBacklogChanged,
        /// <summary>A person went on sabbatical. SubjectId is the person, Amount the weeks, Ref the role id.</summary>
        SabbaticalStarted,
        /// <summary>A person came back. SubjectId is the person, Amount their morale now, Pick 2 refreshed, 1 rested, 0 restless, -1 called back early, Ref the role id.</summary>
        SabbaticalEnded,
        /// <summary>An order for people was placed. Amount is the people, Pick the weeks until they arrive, Ref the role id.</summary>
        RequisitionOpened,
        /// <summary>An order ended. SubjectId is the people ordered, Amount how many came, Pick 1 when all came, 0 when some or all were lost and -1 when it was cancelled, Ref the role id.</summary>
        RequisitionClosed,
        /// <summary>A group of visiting scholars arrived. Amount is how many, Pick the weeks they stay.</summary>
        ScholarsArrived,
        /// <summary>The scholars left. SubjectId is how many, Amount the prestige they added, Pick 1 when they left on schedule and 0 when they were sent home.</summary>
        ScholarsLeft,
        /// <summary>A claim was filed against the lab. SubjectId is the claim, Amount what it asks, Pick the kind (0 incident, 1 layoff round, 2 breach, 3 charter), Ref a name.</summary>
        CaseFiled,
        /// <summary>A hearing was lost. SubjectId is the claim, Amount what the lab will pay, Pick 1 when it is the appeal's ruling.</summary>
        CaseJudged,
        /// <summary>A claim ended. SubjectId is the claim, Amount what the lab paid, Pick 0 settled, 1 won, 2 lost and paid, 3 overturned on appeal.</summary>
        CaseClosed,
        /// <summary>A security tier was built. Amount is the tier.</summary>
        SecurityBuilt,
        /// <summary>A breach opened. Amount is the week it leaks if nobody answers.</summary>
        BreachOpened,
        /// <summary>A breach was answered. Amount is the cost, Pick 0 ransom, 1 disclosure, 2 lockdown.</summary>
        BreachAnswered,
        /// <summary>An unanswered breach leaked. Amount is the reputation it cost.</summary>
        BreachLeaked,
        /// <summary>A government offered a contract. Ref is its kind, Amount its pay, Pick the weeks it lasts.</summary>
        ContractOffered,
        /// <summary>A contract was accepted. Ref is its kind, Amount its pay.</summary>
        ContractAccepted,
        /// <summary>An offer ended without a contract. Ref is its kind, Pick 1 when the lab turned it down and 0 when it lapsed.</summary>
        ContractDeclined,
        /// <summary>An instalment fell due. Ref is the kind, Amount what was paid, Pick 1 paid and 0 missed.</summary>
        ContractInstalment,
        /// <summary>A contract ended. Ref is the kind, Amount the penalty paid, Pick 1 completed, 0 ended for misses, -1 abandoned, 2 lost to a leak.</summary>
        ContractEnded,
        /// <summary>A change of charter went under review. Amount is the fee, Pick the form it is heading for.</summary>
        CharterReviewStarted,
        /// <summary>A change of charter was dropped. Pick is the form it was heading for.</summary>
        CharterReviewDropped,
        /// <summary>The lab's charter changed. Pick is the new form, Amount the old one.</summary>
        CharterChanged,
        /// <summary>A trained model reached a tripwire of the lab's policy. SubjectId is the model, Amount the level of the tripwire.</summary>
        TripwireHit,
        /// <summary>The lab is no longer held up by a tripwire. Amount is the level that held it, Pick 1 when the safeguards were put in place and 0 when the lab broke or lowered its policy or the model was gone.</summary>
        TripwireCleared,
        /// <summary>The policy level changed. Amount is the new level, Pick the old one, Ref is "raise", "lower" or "break".</summary>
        PolicyChanged,
        /// <summary>A tax structure opened. Amount is the tier.</summary>
        TaxStructureBuilt,
        /// <summary>The year's tax was paid. Amount is what was paid, Pick 1 when the lab kept a structure that year.</summary>
        TaxPaid,
        /// <summary>The research credit was claimed. Amount is the credit.</summary>
        TaxCreditClaimed,
        /// <summary>A tax audit opened. Amount is what it is about, Pick 1 when the lab kept an offshore structure.</summary>
        TaxAuditOpened,
        /// <summary>A tax audit was answered. Amount is what the lab paid, Pick 0 cooperate, 1 settle, 2 contest, 3 left unanswered.</summary>
        TaxAuditClosed,
        /// <summary>The industry began drafting a standard. Amount is its level.</summary>
        StandardDrafted,
        /// <summary>A standard was ratified. Amount is its level, Pick the part the lab took in the draft.</summary>
        StandardRatified,
        /// <summary>The lab adopted the standards. Amount is the level it now complies with.</summary>
        StandardAdopted,
        /// <summary>The lab released a capable model without complying. SubjectId is the model, Amount the fine and Pick the levels it lacked.</summary>
        StandardLapsed,
        /// <summary>GPUs failed in a week. Amount is the units, Pick 0 lost, 1 swapped for spares, 2 sent for replacement under the warranty.</summary>
        GpusFailed,
        /// <summary>Units came back under the warranty. Amount is the units, Pick 1 when they were rushed.</summary>
        ReplacementsArrived,
        /// <summary>The warranty tier changed. Amount is the new tier.</summary>
        WarrantyChanged,
        /// <summary>Spares were bought. Amount is how many, SubjectId the spares held now.</summary>
        SparesStocked,
        /// <summary>The player's chips no longer cover its loans as the covenant needs. Amount is the cover in permille.</summary>
        CovenantBreached,
        /// <summary>The breach ended. Amount is the cover, Pick 0 when it came right by itself, 1 when the lab paid down.</summary>
        CovenantCured,
        /// <summary>The lender looked away for a fee. Amount is the fee.</summary>
        CovenantWaived,
        /// <summary>The lender called the loans after the cure weeks. Amount is what was owed, Pick 1 when the lab paid it from cash.</summary>
        LoansCalled,
        /// <summary>The capital allowance policy changed. Amount is the new policy, SubjectId the pool written off at once when going back to expensing.</summary>
        CapitalPolicySet,
        /// <summary>The lab listed. Amount is the proceeds after the fee.</summary>
        CompanyListed,
        /// <summary>A quarter was reported. Amount is the share price now, Pick 0 a miss, 1 met guidance, 2 a beat.</summary>
        EarningsReported,
        /// <summary>The lab sold more shares. Amount is the proceeds.</summary>
        FollowOnSold,
        /// <summary>The lab bought shares back. Amount is what it spent.</summary>
        SharesBoughtBack,
        /// <summary>An activist moved closer. Amount is the step.</summary>
        ActivistArrived,
        /// <summary>The lab went private. Amount is what it paid.</summary>
        CompanyDelisted,
        /// <summary>The lab took a revenue advance. Amount is the cash, SubjectId the tier.</summary>
        RevenueAdvanceTaken,
        /// <summary>A revenue advance was cleared. Amount is what was paid at the end, Pick 0 swept out of revenue and 1 bought out.</summary>
        RevenueAdvanceCleared,
        /// <summary>A slump week stretched the advance. Amount is what it added.</summary>
        RevenueAdvanceStretched,
        /// <summary>The lab put money into its foundation. Amount is the gift, SubjectId the focus.</summary>
        FoundationEndowed,
        /// <summary>The foundation reported what it gave. Amount is the giving since the last report, SubjectId the goodwill.</summary>
        FoundationReported,
        /// <summary>The foundation changed its cause. Amount is the new focus.</summary>
        FoundationRefocused,
        /// <summary>The lab wound the foundation up. Amount is what it took back.</summary>
        FoundationWoundUp,
        /// <summary>The foundation gave until nothing was left. Amount is what it gave in all.</summary>
        FoundationRanDry,
        /// <summary>The lab sold GPUs and leased them back. Amount is the advance, SubjectId the units.</summary>
        GpusLeasedBack,
        /// <summary>The lab bought its leased GPUs back. Amount is the price.</summary>
        LeaseBoughtBack,
        /// <summary>A lease ended. Amount is the units that went, Pick 0 the term ran out, 1 handed back early, 2 repossessed.</summary>
        LeaseEnded,
        /// <summary>The lab could not pay the rent this week. Amount is the rent, SubjectId the weeks in a row.</summary>
        LeaseRentMissed,
        /// <summary>The lab extended its lease. Amount is the new rent.</summary>
        LeaseExtended,
        /// <summary>The lab issued a convertible note. Amount is the cash, SubjectId the tier.</summary>
        NoteIssued,
        /// <summary>A note turned into shares. Amount is what was owed, SubjectId the share of the lab it became in permille, Pick 0 at a funding round, 1 at a listing, 2 at maturity.</summary>
        NoteConverted,
        /// <summary>The lab repaid the note. Amount is what it paid.</summary>
        NoteRepaid,
        /// <summary>The lab extended the note. Amount is the new weekly rate.</summary>
        NoteExtended,
        /// <summary>The lab started designing a device. Amount is the cost, SubjectId the line.</summary>
        DeviceDesigned,
        /// <summary>A device launched. Amount is its quality, SubjectId the line, Pick 1 a hit and 0 a flop.</summary>
        DeviceLaunched,
        /// <summary>A poor device was recalled. Amount is what the recall cost, SubjectId the line.</summary>
        DeviceRecalled,
        /// <summary>The lab ended its device line. Amount is the units sold in all, SubjectId the line.</summary>
        DeviceWithdrawn,
        /// <summary>The lab signed a distribution deal. Amount is the platform's cut in permille, SubjectId the platform.</summary>
        PlatformSigned,
        /// <summary>A platform raised its price in the renewal window. Amount is the new cut, SubjectId the platform.</summary>
        PlatformHoldup,
        /// <summary>The lab renewed a deal. Amount is the cut, SubjectId the platform.</summary>
        PlatformRenewed,
        /// <summary>A deal ended. Amount is the reputation lost, SubjectId the platform, Pick 0 the lab walked, 1 the term ran out.</summary>
        PlatformEnded,
        /// <summary>The lab stated its values. Amount is the integrity it starts at, SubjectId is first times ten plus second.</summary>
        CreedSet,
        /// <summary>The lab held a town hall. Amount is the integrity after.</summary>
        TownHallHeld,
        /// <summary>The lab owned up. Amount is the reputation it cost.</summary>
        CreedOwnedUp,
        /// <summary>The lab's integrity fell far enough for the creed to be a joke. Amount is the reputation lost.</summary>
        CreedHollow,
        /// <summary>The lab's integrity rose far enough for it to be held up as an example. Amount is the reputation earned.</summary>
        CreedProud,
        /// <summary>A shock landed on the lab. SubjectId is the kind (1 squeeze, 2 backlash, 3 boom), Ref the world event.</summary>
        ShockStruck,
        /// <summary>A shock was answered. Amount is the reputation it changed, SubjectId the kind, Ref the world event, Pick 1 braced, 2 rode, 3 countered and it worked, 4 countered and it failed, 5 nobody answered.</summary>
        ShockAnswered,
        /// <summary>The lab put a paper's code out. Amount is the research points the clean-up cost, Pick the weeks of clean-up, SubjectId how reproducible it is.</summary>
        CodeReleased,
        /// <summary>The lab kept a paper's code back. Amount is the prestige it cost, Pick 0 the lab chose to, 1 it waited too long.</summary>
        CodeWithheld,
        /// <summary>Another lab tried to reproduce a paper. Amount is the prestige it changed, SubjectId how reproducible the paper was, Pick 1 it replicated and 0 it did not.</summary>
        PaperReplicated,
        /// <summary>The lab made a public bet on a tech. SubjectId is its place in the data, Ref the tech.</summary>
        TechBetMade,
        /// <summary>A bet ended. Amount is the reputation it changed, Ref the tech, Pick 1 kept, 0 the weeks ran out, 2 taken back.</summary>
        TechBetEnded,
        /// <summary>The lab opened a waitlist. SubjectId is the model, Amount the weeks promised.</summary>
        WaitlistOpened,
        /// <summary>A waitlist ended. Amount is the reputation it changed, SubjectId the model, Pick 1 shipped in time, 0 the promise lapsed, 2 closed early.</summary>
        WaitlistEnded,
        /// <summary>The lab set up a careers page. Amount is the cash it cost.</summary>
        CareersOpened,
        /// <summary>A bad review of the lab as an employer appeared. Amount is the score.</summary>
        BadReview,
        /// <summary>A bad review was answered. Amount is the score change, Pick 1 fixed, 2 rebutted, 3 left, 0 nobody answered in time.</summary>
        ReviewAnswered,
        /// <summary>The lab opened a free tier. Amount is the level.</summary>
        FreeTierOpened,
        /// <summary>The free tier closed. Amount is the reputation lost, SubjectId the weeks it ran, Pick 1 shut by the lab, 0 the model went.</summary>
        FreeTierClosed,
        /// <summary>The lab set up a launch watch desk. Amount is the cash it cost.</summary>
        WatchDeskOpened,
        /// <summary>The desk saw a rival launch. SubjectId is the rival, Amount how far ahead of the lab it is in permille (0 when the lab already beats it), Pick 1 it beats the lab.</summary>
        RivalLaunchSeen,
        /// <summary>A rival launch was answered. SubjectId is the rival, Amount the reputation change, Pick 1 counter-programmed, 2 held, 3 owned the safety story, 0 nobody answered in time.</summary>
        LaunchAnswered,
        /// <summary>The lab started a research programme. SubjectId is the tech's place in the data, Ref the tech.</summary>
        ProgrammeStarted,
        /// <summary>A programme stage worked. Amount is the stage number, Pick 1 careful, 2 bold, Ref the tech.</summary>
        ProgrammeStageDone,
        /// <summary>A bold stage failed and waits for a decision. Amount is the stage number, Ref the tech.</summary>
        ProgrammeStageFailed,
        /// <summary>A programme ended. Amount is the research points paid back or refunded, Pick 1 finished, 0 given up, Ref the tech.</summary>
        ProgrammeEnded,
        /// <summary>A training run's priority changed. SubjectId is the model, Amount the compute of progress lost, Pick the new level.</summary>
        RunPriorityChanged,
        /// <summary>The lab formed a project pod. Amount is its chemistry, SubjectId the people in it.</summary>
        PodFormed,
        /// <summary>A pod member was swapped. Amount is the new chemistry.</summary>
        PodSwapped,
        /// <summary>A pod ended. Amount is the research points it added or took over its life, Pick 1 the lab disbanded it, 0 it broke up.</summary>
        PodEnded,
        /// <summary>The lab filed for a licence. Amount is the fee, Pick the regime.</summary>
        LicenceApplied,
        /// <summary>An application was decided. Pick 1 granted, 0 refused, SubjectId the regime.</summary>
        LicenceDecided,
        /// <summary>An incident counted against the licence. Amount is the strikes now.</summary>
        LicenceStrike,
        /// <summary>A licence ended. Pick 0 lapsed, 1 surrendered, 2 suspended, SubjectId the regime.</summary>
        LicenceEnded,
        /// <summary>The lab pledged to support a model. Amount is the weeks.</summary>
        SupportPledged,
        /// <summary>A pledge ended. Pick 1 kept, 0 broken, Amount the fee paid for a breach.</summary>
        SupportPledgeEnded,
        /// <summary>The lab opened a model's API.</summary>
        ApiOpened,
        /// <summary>The lab began retiring a model's API. Amount is the weeks.</summary>
        ApiDeprecated,
        /// <summary>A deprecation ended. Pick 1 when nothing was left behind, 0 when integrations were stranded; Amount is how many.</summary>
        ApiEnded,
        /// <summary>The lab began a conformity assessment. Pick is the scope.</summary>
        ConformityStarted,
        /// <summary>A review ended. Pick 1 certified, 0 findings.</summary>
        ConformityDecided,
        /// <summary>An assessment or certificate ended. Pick 0 withdrawn, 1 expired, 2 suspended.</summary>
        ConformityEnded,
        /// <summary>The lab changed how it is organised. SubjectId is the new form (1 flat, 2 functional, 3 squads); Amount is the weeks of disruption.</summary>
        OrgChanged,
        /// <summary>The structure is costing the lab. Amount is the loss in permille, SubjectId the form. Logged when it first crosses a quarter of the cap and again after a change.</summary>
        OrgStrained,
        /// <summary>The lab ordered a fabric tier. SubjectId is the tier, Amount the deposit.</summary>
        FabricOrdered,
        /// <summary>A fabric tier was delivered. SubjectId is the tier, Amount the balance paid.</summary>
        FabricDelivered,
        /// <summary>A fabric order was cancelled. SubjectId is the tier, Amount the deposit lost.</summary>
        FabricCancelled,
        /// <summary>The lab joined or left the incident register. Pick 1 joined, 0 left.</summary>
        RegisterChanged,
        /// <summary>A report was filed. SubjectId is the model, Pick 1 a summary, 2 a full report; Amount the trust gained.</summary>
        ReportFiled,
        /// <summary>A report was late. SubjectId is the model, Amount the fine.</summary>
        ReportLate,
        /// <summary>An inspection of the register. Pick 1 clean, 0 fined; Amount the fine.</summary>
        InspectionHeld,
        /// <summary>Raw data came in. SubjectId is the source (1 scraped, 2 licensed, 3 synthetic, 4 users), Amount the units.</summary>
        DataAcquired,
        /// <summary>A cleaning job finished. SubjectId is the source, Amount the units that came through.</summary>
        DataCleaned,
        /// <summary>Scraped units were licensed after the fact. Amount is the units.</summary>
        DataLicensed,
        /// <summary>A claim landed over scraped data. SubjectId is the units purged, Amount the fine.</summary>
        DataSuit,
        /// <summary>The lab changed what it does with its users' data. SubjectId is the policy (0 none, 1 opt-in, 2 default).</summary>
        UserDataChanged,
        /// <summary>A privacy complaint landed. Amount is the fine, SubjectId the units collected since the last one.</summary>
        UserDataComplaint,
        /// <summary>Users' data was deleted when the lab moved off a default policy. Amount is the units lost.</summary>
        UserDataDeleted,
        /// <summary>The lab changed how it is controlled. SubjectId is the form (1 dual-class, 2 non-profit parent).</summary>
        GovernanceChanged,
        /// <summary>An independent director joined. SubjectId is how many there are now.</summary>
        DirectorSeated,
        /// <summary>An independent director resigned because the lab could not pay. SubjectId is how many are left.</summary>
        DirectorResigned,
        /// <summary>The lab bought a funding round back. Amount is the price, Text the round.</summary>
        RoundBoughtBack,
        /// <summary>A training run was preempted and lost progress. SubjectId is the model, Amount the compute lost, Ref the model name.</summary>
        SpotPreempted,
        /// <summary>The board voted on buying a round back. Pick is 1 passed or 0 refused, Amount the yes votes in permille of all, Ref the round.</summary>
        BoardVoted,
        /// <summary>The autonomy of a model changed. SubjectId is the model, Amount the level.</summary>
        AutonomySet,
        /// <summary>An agent run went wrong. SubjectId is the model, Amount the revenue refunded, Ref the model name.</summary>
        AgentBlowup,
        /// <summary>A model kept its agents clean for a long streak. SubjectId is the model, Ref the model name.</summary>
        AgentStreak,
        /// <summary>A vendor was contracted. SubjectId is the vendor.</summary>
        AnnotatorsContracted,
        /// <summary>The vendor contract ended. SubjectId is the vendor, Pick 1 ran its term, 0 ended early; a scandal has its own event.</summary>
        AnnotatorsEnded,
        /// <summary>The vendor was audited. Amount is the exposure left.</summary>
        VendorAudited,
        /// <summary>A labour scandal about the vendor broke. SubjectId is the vendor, Amount the fine.</summary>
        VendorScandal,
        /// <summary>A tuning run started. SubjectId is the model, Amount the kind.</summary>
        PostTrainingStarted,
        /// <summary>A tuning run finished. SubjectId is the model, Pick 1 preference tuning, 0 safety tuning, Amount the number of runs of that kind it now has.</summary>
        PostTrainingDone,
        /// <summary>A tuning run was abandoned. SubjectId is the model.</summary>
        PostTrainingAbandoned,
        /// <summary>A person was named as the owner of a model. SubjectId is the model, Amount the staff id.</summary>
        OwnerNamed,
        /// <summary>The owner of a model left. SubjectId is the model, Amount the gap that was left.</summary>
        OwnerLeft,
        /// <summary>Documentation was written for a model. SubjectId is the model, Amount its level.</summary>
        DocsWritten,
        /// <summary>The documentation of a model went stale. SubjectId is the model, Amount its level now.</summary>
        DocsStale,
        /// <summary>The board tabled a motion. SubjectId is the kind (1 calm, 2 runway, 3 safety), Amount its goal.</summary>
        MotionTabled,
        /// <summary>The lab answered a motion. SubjectId is the kind, Pick 1 accepted, 0 declined.</summary>
        MotionAnswered,
        /// <summary>A motion went unanswered. SubjectId is the kind.</summary>
        MotionIgnored,
        /// <summary>A commitment reached its deadline. SubjectId is the kind, Pick 1 kept, 0 broken.</summary>
        MotionJudged,
        /// <summary>The board forced a vote on the founders. Amount is the tension.</summary>
        BoardForcedVote,
        /// <summary>A drill started. Amount is the weeks it will run.</summary>
        DrillStarted,
        /// <summary>A drill finished. Pick 1 passed, 0 failed, Amount is the new preparedness.</summary>
        DrillFinished,
        /// <summary>A model was registered on a public eval. SubjectId is the model, Pick is the suite, Amount the cadence in weeks.</summary>
        EvalRegistered,
        /// <summary>A public eval score was published. SubjectId is the model, Pick 1 beat the bar, 0 below, Amount the score.</summary>
        EvalPublished,
        /// <summary>The tool policy of a model changed. SubjectId is the model, Amount is the mask.</summary>
        ToolPolicySet,
        /// <summary>The human queue grew. Amount is the depth.</summary>
        HumanQueueDeep,
        /// <summary>The desk cleared queue items. Amount cleared, Pick 1 staffed, 0 paid.</summary>
        HumanQueueCleared,
        /// <summary>A release checklist item was signed. SubjectId is the model, Amount the item bit.</summary>
        ChecklistSigned,
        /// <summary>An agent sandbox started. SubjectId is the model, Amount the weeks.</summary>
        SandboxStarted,
        /// <summary>An agent sandbox finished. Pick 1 passed, 0 failed, Amount the score.</summary>
        SandboxFinished,
        /// <summary>The runaway playbook level changed. Amount is the level.</summary>
        PlaybookSet,
        /// <summary>The runaway playbook was armed. Amount is the weeks.</summary>
        PlaybookArmed,
        /// <summary>The orchestration budget level changed. Amount is the level.</summary>
        OrchBudgetSet,
        /// <summary>A model route share changed. SubjectId is the model, Amount ppm.</summary>
        RouteShareSet,
        /// <summary>Public metrics were published. SubjectId is the model, Amount the score.</summary>
        MetricsPublished,
        /// <summary>Public metrics were stopped. SubjectId is the model.</summary>
        MetricsStopped,
        SpecialistBranched,
        DataSourceAcquired,
        DataSourceRenewed,
        DataSourceLapsed,
        WeightsOpened,
        WeightsClosed,
        WeightsCopied,
        SectorPriceSet,
        CampaignDeadlineSet,
        CampaignDeadlineHit,
        LawShaped,
        ScenarioBaggagePicked,
        MarginCall,
        MarginCallSatisfied,
        MarginCallDefaulted,
        HearingShareNudged,
        ScenarioGoalMet,
        ScenarioGoalMissed,
        LawTargetSet,
        ReservedComputeSigned,
        RivalIntentSurfaced,
        GpuTierLocked,
        ScenarioEndlessAck,
        SpecialistEraUnlocked,
        SectorPriceLadderPeak,
        DataSourceSpecialistPaired,
        PublicMetricsEraBoost,
        OpenWeightsEraThrottled,
        EvalHarnessEraBoosted,
        CampaignDeadlineElongated,
        LawShapingCooldownShortened,
        DataSourceRenewEraBonused,
        SectorPricingEraRelieved,
        LawTargetEraAmplified,
        SpecialistBranchEraDiscounted,
        MarginCallEraGraced
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
