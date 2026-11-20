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
        BoardDrama
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
