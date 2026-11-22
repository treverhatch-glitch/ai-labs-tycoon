using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Licensing regimes that open with the eras. The lab can file under any regime open now: the registry, the licence, the audit regime, each dearer and stricter than the last. An application is reviewed for some weeks and is granted only if the lab's best released model is aligned enough; a granted licence cuts incident fines but each incident while licensed is a strike, and enough strikes suspend it. A licence can be renewed late in its term, surrendered, or left to lapse. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LicenceConfig : ISync
    {
        public int Enabled;
        /// <summary>Era (by place in the list) in which regime 1, the registry, opens.</summary>
        public int EraA;
        /// <summary>Era in which regime 2, the licence, opens.</summary>
        public int EraB;
        /// <summary>Era in which regime 3, the audit regime, opens.</summary>
        public int EraC;
        /// <summary>Fee to apply to regime 1.</summary>
        public int FeeA;
        /// <summary>Fee to apply to regime 2.</summary>
        public int FeeB;
        /// <summary>Fee to apply to regime 3.</summary>
        public int FeeC;
        /// <summary>Weeks a regime 1 licence lasts.</summary>
        public int TermA;
        /// <summary>Weeks a regime 2 licence lasts.</summary>
        public int TermB;
        /// <summary>Weeks a regime 3 licence lasts.</summary>
        public int TermC;
        /// <summary>Alignment the lab best released model needs for regime 1 (0 to 1000).</summary>
        public int BarA;
        /// <summary>Same for regime 2.</summary>
        public int BarB;
        /// <summary>Same for regime 3.</summary>
        public int BarC;
        /// <summary>Share a regime 1 licence takes off incident fines (permille).</summary>
        public int CutA;
        /// <summary>Same for regime 2.</summary>
        public int CutB;
        /// <summary>Same for regime 3.</summary>
        public int CutC;
        /// <summary>Weeks an application is reviewed.</summary>
        public int ReviewWeeks;
        /// <summary>Weeks before another application after a refusal or a suspension.</summary>
        public int CooldownWeeks;
        /// <summary>Incidents while licensed that suspend the licence.</summary>
        public int StrikeLimit;
        /// <summary>Reputation lost when a licence is suspended (damped).</summary>
        public int SuspendReputation;
        /// <summary>Weeks before the end of a licence in which it can be renewed.</summary>
        public int RenewWindow;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("eraA", ref EraA);
            s.Int("eraB", ref EraB);
            s.Int("eraC", ref EraC);
            s.Int("feeA", ref FeeA);
            s.Int("feeB", ref FeeB);
            s.Int("feeC", ref FeeC);
            s.Int("termA", ref TermA);
            s.Int("termB", ref TermB);
            s.Int("termC", ref TermC);
            s.Int("barA", ref BarA);
            s.Int("barB", ref BarB);
            s.Int("barC", ref BarC);
            s.Int("cutA", ref CutA);
            s.Int("cutB", ref CutB);
            s.Int("cutC", ref CutC);
            s.Int("reviewWeeks", ref ReviewWeeks);
            s.Int("cooldownWeeks", ref CooldownWeeks);
            s.Int("strikeLimit", ref StrikeLimit);
            s.Int("suspendReputation", ref SuspendReputation);
            s.Int("renewWindow", ref RenewWindow);
        }
    }
}
