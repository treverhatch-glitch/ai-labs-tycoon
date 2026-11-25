using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Eval harness (bible ch. 8, benchmarks customers see). A released model can be registered for a narrow or broad public suite. Registration costs cash and research points and sets a cadence; each publish compares the score (from capability and alignment) to a bar and shifts trust in every sector the model sells in. Low scores add pressure on conformity reviews already under way. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class EvalHarnessConfig : ISync
    {
        public int Enabled;
        /// <summary>First era in which models can be on a public eval cadence.</summary>
        public int MinEra;
        /// <summary>Cash to register a suite on a model.</summary>
        public int Cost;
        /// <summary>Weeks between published scores for a registered model.</summary>
        public int Cadence;
        /// <summary>Research points to publish once (included in registration).</summary>
        public int Points;
        /// <summary>Trust in each sector when the public score beats the bar.</summary>
        public int TrustGain;
        /// <summary>Trust lost when the score is below the bar.</summary>
        public int TrustLoss;
        /// <summary>Score needed to beat the bar on the narrow suite.</summary>
        public int BarNarrow;
        /// <summary>Score needed to beat the bar on the broad suite.</summary>
        public int BarBroad;
        /// <summary>Extra weeks conformity review needs when scores stay low.</summary>
        public int ConformityPush;
        /// <summary>Tension added to a board that meets when a public score misses the bar.</summary>
        public int FailTension;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("cost", ref Cost);
            s.Int("cadence", ref Cadence);
            s.Int("points", ref Points);
            s.Int("trustGain", ref TrustGain);
            s.Int("trustLoss", ref TrustLoss);
            s.Int("barNarrow", ref BarNarrow);
            s.Int("barBroad", ref BarBroad);
            s.Int("conformityPush", ref ConformityPush);
            s.Int("failTension", ref FailTension);
        }
    }
}
