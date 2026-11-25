using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// An endowed foundation. The lab puts money into a fund that earns a steady return and gives a share of it away each week, to safety, education or open science. Giving builds goodwill; goodwill fades; and what it is worth depends on the focus: it damps scandals, it feeds reputation, or it weakens the claims in court. The fund can be topped up, re-pointed at a cost, or wound up for most of its money back. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class FoundationConfig : ISync
    {
        public int Enabled;
        /// <summary>The least a first endowment can be.</summary>
        public int MinGift;
        /// <summary>What the fund earns each week, in permille of the fund.</summary>
        public int ReturnPermille;
        /// <summary>Level 1 weekly giving, in permille of the fund.</summary>
        public int PayoutA;
        /// <summary>Level 2 weekly giving.</summary>
        public int PayoutB;
        /// <summary>Level 3 weekly giving.</summary>
        public int PayoutC;
        /// <summary>Giving needed for one point of goodwill.</summary>
        public int GoodwillUnit;
        /// <summary>Share of goodwill lost each week, in permille.</summary>
        public int GoodwillDecay;
        /// <summary>The most goodwill there can be.</summary>
        public int MaxGoodwill;
        /// <summary>Scandal damping a full 1000 points of goodwill give a safety foundation, in permille.</summary>
        public int SafetyDamp;
        /// <summary>The most damping a foundation gives.</summary>
        public int MaxDamp;
        /// <summary>Weeks between reputation points for an education foundation.</summary>
        public int EducationEvery;
        /// <summary>Goodwill an education foundation needs to give reputation.</summary>
        public int EducationAt;
        /// <summary>Claim strength a full 1000 points of goodwill take off for an open science foundation.</summary>
        public int ScienceStrength;
        /// <summary>Share of goodwill lost when the focus changes, in permille.</summary>
        public int SwitchLoss;
        /// <summary>Weeks the focus is locked after a change.</summary>
        public int LockWeeks;
        /// <summary>Share of the fund the lab gets back when it winds the foundation up.</summary>
        public int WindUpPermille;
        /// <summary>Reputation lost when it does.</summary>
        public int WindUpReputation;
        /// <summary>Weeks between reports of what the foundation gave.</summary>
        public int ReportWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minGift", ref MinGift);
            s.Int("returnPermille", ref ReturnPermille);
            s.Int("payoutA", ref PayoutA);
            s.Int("payoutB", ref PayoutB);
            s.Int("payoutC", ref PayoutC);
            s.Int("goodwillUnit", ref GoodwillUnit);
            s.Int("goodwillDecay", ref GoodwillDecay);
            s.Int("maxGoodwill", ref MaxGoodwill);
            s.Int("safetyDamp", ref SafetyDamp);
            s.Int("maxDamp", ref MaxDamp);
            s.Int("educationEvery", ref EducationEvery);
            s.Int("educationAt", ref EducationAt);
            s.Int("scienceStrength", ref ScienceStrength);
            s.Int("switchLoss", ref SwitchLoss);
            s.Int("lockWeeks", ref LockWeeks);
            s.Int("windUpPermille", ref WindUpPermille);
            s.Int("windUpReputation", ref WindUpReputation);
            s.Int("reportWeeks", ref ReportWeeks);
        }
    }
}
