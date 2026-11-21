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
        ModelRolledBack
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
