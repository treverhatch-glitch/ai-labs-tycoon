using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Distribution deals with a platform: a phone maker, a browser or an office suite. A deal lifts the lab's revenue every week and the platform takes a cut of the lift. The longer it runs the more of the lab's users come through the platform; when the term nears its end the platform names its price, which goes up if the lab has come to rely on it. The lab renews at the worse price, or walks and loses users. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PlatformsConfig : ISync
    {
        public int Enabled;
        /// <summary>Capability of the best released model a platform wants before it talks.</summary>
        public int MinCapability;
        /// <summary>Reputation a platform wants before it talks.</summary>
        public int MinReputation;
        /// <summary>Index of the first era a phone maker will sign a deal.</summary>
        public int EraA;
        /// <summary>Index of the first era a browser maker will.</summary>
        public int EraB;
        /// <summary>Index of the first era an office suite will.</summary>
        public int EraC;
        /// <summary>Extra weekly revenue a phone deal brings, in permille of what the market paid the lab.</summary>
        public int LiftA;
        /// <summary>Extra revenue a browser deal brings.</summary>
        public int LiftB;
        /// <summary>Extra revenue an office suite deal brings.</summary>
        public int LiftC;
        /// <summary>The phone maker's first cut of the extra revenue, in permille.</summary>
        public int CutA;
        /// <summary>The browser maker's first cut.</summary>
        public int CutB;
        /// <summary>The office suite's first cut.</summary>
        public int CutC;
        /// <summary>Weeks a deal runs.</summary>
        public int TermWeeks;
        /// <summary>Weeks at the end of a term in which the platform names its price and the lab can renew.</summary>
        public int WindowWeeks;
        /// <summary>How much more of the lab's users come through the platform each week, in permille of all of them.</summary>
        public int RelianceStep;
        /// <summary>Reliance, in permille, at which the platform raises its cut in the renewal window.</summary>
        public int HoldupBar;
        /// <summary>What the platform adds to its cut then, in permille.</summary>
        public int HoldupCut;
        /// <summary>What every renewal adds to the cut, in permille.</summary>
        public int RenewCutStep;
        /// <summary>The most a platform will take, in permille.</summary>
        public int MaxCut;
        /// <summary>Reputation lost when a deal ends and all the lab's users came through it; less when fewer did.</summary>
        public int WalkReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minCapability", ref MinCapability);
            s.Int("minReputation", ref MinReputation);
            s.Int("eraA", ref EraA);
            s.Int("eraB", ref EraB);
            s.Int("eraC", ref EraC);
            s.Int("liftA", ref LiftA);
            s.Int("liftB", ref LiftB);
            s.Int("liftC", ref LiftC);
            s.Int("cutA", ref CutA);
            s.Int("cutB", ref CutB);
            s.Int("cutC", ref CutC);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("windowWeeks", ref WindowWeeks);
            s.Int("relianceStep", ref RelianceStep);
            s.Int("holdupBar", ref HoldupBar);
            s.Int("holdupCut", ref HoldupCut);
            s.Int("renewCutStep", ref RenewCutStep);
            s.Int("maxCut", ref MaxCut);
            s.Int("walkReputation", ref WalkReputation);
        }
    }
}
