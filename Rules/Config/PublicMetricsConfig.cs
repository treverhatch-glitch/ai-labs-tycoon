using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Public metrics (bible ch. 6): customer-visible uptime board per released model. Publishing moves trust; incidents mark misses and eval conformity feels weak scores. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PublicMetricsConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for public metrics.</summary>
        public int MinEra;
        /// <summary>Cash to publish metrics for a model.</summary>
        public int PublishCost;
        /// <summary>Trust gained in each sector when the score beats the bar.</summary>
        public int TrustGain;
        /// <summary>Trust lost in each sector when the score is below the bar.</summary>
        public int TrustLoss;
        /// <summary>Trust lost in each sector when an incident marks a published miss.</summary>
        public int MissTrust;
        /// <summary>Bar for a good public score (out of 1000).</summary>
        public int Bar;
        /// <summary>Weeks added to conformity review when the score is bad.</summary>
        public int ConformityPush;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("publishCost", ref PublishCost);
            s.Int("trustGain", ref TrustGain);
            s.Int("trustLoss", ref TrustLoss);
            s.Int("missTrust", ref MissTrust);
            s.Int("bar", ref Bar);
            s.Int("conformityPush", ref ConformityPush);
        }
    }
}