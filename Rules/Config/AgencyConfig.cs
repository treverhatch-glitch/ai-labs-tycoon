using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Agent autonomy (bible ch. 6, agency: tools and long tasks, and the agents era). A lab can let a released model run its agents with more freedom: supervised, delegated or autonomous. Each level lifts what the model earns but adds to its weekly incident chance, and from delegated up an agent run can go wrong: a refund of revenue and lost trust, less likely with every containment wing. Delegated needs a containment wing, autonomous needs more of them and a well-aligned model. A clean streak earns trust. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AgencyConfig : ISync
    {
        public int Enabled;
        /// <summary>Index of the first era in which a model can be given autonomy.</summary>
        public int MinEra;
        /// <summary>What each level of autonomy costs to set up, for each level added.</summary>
        public int SetupCost;
        /// <summary>Share of a model revenue that supervised agents add (permille).</summary>
        public int Lift1;
        /// <summary>Share of a model revenue that delegated agents add (permille).</summary>
        public int Lift2;
        /// <summary>Share of a model revenue that autonomous agents add (permille).</summary>
        public int Lift3;
        /// <summary>Weekly incident chance supervised agents add to the model (permille).</summary>
        public int Incident1;
        /// <summary>Weekly incident chance delegated agents add to the model (permille).</summary>
        public int Incident2;
        /// <summary>Weekly incident chance autonomous agents add to the model (permille).</summary>
        public int Incident3;
        /// <summary>Containment wings needed for delegated agents.</summary>
        public int Rooms2;
        /// <summary>Containment wings needed for autonomous agents.</summary>
        public int Rooms3;
        /// <summary>Alignment a model needs for autonomous agents.</summary>
        public int MinAlign3;
        /// <summary>Weekly chance a delegated agent run goes wrong (permille).</summary>
        public int Blowup2;
        /// <summary>Weekly chance an autonomous agent run goes wrong (permille).</summary>
        public int Blowup3;
        /// <summary>Share each containment wing takes off the chance of a blow-up (permille, at most 750 in all).</summary>
        public int RoomCut;
        /// <summary>Weeks of the model revenue refunded to customers after a blow-up.</summary>
        public int RefundWeeks;
        /// <summary>Trust lost in each sector the model is sold in after a blow-up.</summary>
        public int BlowupTrust;
        /// <summary>Weeks without a blow-up that earn the lab trust.</summary>
        public int StreakWeeks;
        /// <summary>Trust won in each sector after a clean streak.</summary>
        public int StreakTrust;
        /// <summary>Weeks before the level of a model can be raised again.</summary>
        public int Cooldown;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("setupCost", ref SetupCost);
            s.Int("lift1", ref Lift1);
            s.Int("lift2", ref Lift2);
            s.Int("lift3", ref Lift3);
            s.Int("incident1", ref Incident1);
            s.Int("incident2", ref Incident2);
            s.Int("incident3", ref Incident3);
            s.Int("rooms2", ref Rooms2);
            s.Int("rooms3", ref Rooms3);
            s.Int("minAlign3", ref MinAlign3);
            s.Int("blowup2", ref Blowup2);
            s.Int("blowup3", ref Blowup3);
            s.Int("roomCut", ref RoomCut);
            s.Int("refundWeeks", ref RefundWeeks);
            s.Int("blowupTrust", ref BlowupTrust);
            s.Int("streakWeeks", ref StreakWeeks);
            s.Int("streakTrust", ref StreakTrust);
            s.Int("cooldown", ref Cooldown);
        }
    }
}
