using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Model end-of-life notice. The player announces a date (4 to 26 weeks away) on which a released model leaves the market and its customers move to its successor: the model that was designed with its help (see successor design), still on sale and at least as capable. During the notice the lab pays to keep the old model running. On the day each sector the successor can serve moves over, trust falls by more for a short notice than a long one and less for a successor the old model helped design, and a sector the successor cannot serve is dropped. If the successor is gone by then the notice is withdrawn and the lab loses reputation. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SunsetConfig : ISync
    {
        public int Enabled;
        /// <summary>The shortest notice, in weeks.</summary>
        public int MinWeeks;
        /// <summary>The longest notice, in weeks.</summary>
        public int MaxWeeks;
        /// <summary>While the notice runs the lab keeps the old model going next to its successor: this share of what the model earns in a week, paid every week.</summary>
        public int SupportPermille;
        /// <summary>The trust a sector loses at the end of a notice of <c>MinWeeks</c>; it falls in a straight line to <c>TrustDropLong</c> at <c>MaxWeeks</c>.</summary>
        public int TrustDropShort;
        public int TrustDropLong;
        /// <summary>Customers move more easily to a successor the old model helped design (see successor design): this share of the trust loss is spared.</summary>
        public int AssistedCutPermille;
        /// <summary>A sector the successor cannot serve (its capability is under the sector's bar) is dropped, and loses this multiple of the trust, in permille.</summary>
        public int DroppedPermille;
        /// <summary>Reputation lost by calling a notice off.</summary>
        public int CancelReputation;
        /// <summary>Reputation lost when the successor is gone on the day, so the lab breaks its word.</summary>
        public int BrokenReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("supportPermille", ref SupportPermille);
            s.Int("trustDropShort", ref TrustDropShort);
            s.Int("trustDropLong", ref TrustDropLong);
            s.Int("assistedCutPermille", ref AssistedCutPermille);
            s.Int("droppedPermille", ref DroppedPermille);
            s.Int("cancelReputation", ref CancelReputation);
            s.Int("brokenReputation", ref BrokenReputation);
        }
    }
}
