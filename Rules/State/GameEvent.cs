using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum EventKind
    {
        RoomBuilt, RoomDemolished, LotBought,
        Hired, Fired,
        GpusRented, GpusBought, GpusSold,
        TechResearched,
        TrainingStarted, TrainingFinished, TrainingFailed, ModelReleased, ModelRetired,
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
        /// <summary>The global chip crunch began. Amount is strength permille.</summary>
        ChipShortageBegan,
        /// <summary>The global chip crunch ended. Amount is the last strength permille.</summary>
        ChipShortageEnded
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

        public void Sync(Syncer s)
        {
            s.Int("tick", ref Tick);
            s.Enum("kind", ref Kind);
            s.Int("labId", ref LabId);
            s.Int("subjectId", ref SubjectId);
            s.Long("amount", ref Amount);
            s.Int("pick", ref Pick);
            s.Str("ref", ref Ref);
        }
    }
}
