using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Antitrust scrutiny when the bubble runs hot or a monopoly ending fires.</summary>
    public sealed class RegulatorsConfig : ISync
    {
        /// <summary>When the bubble is at or above this, dominant labs gain attention each month.</summary>
        public int BubbleScrutinyAtPermille;
        /// <summary>Only labs with at least this revenue share get scrutiny bumps.</summary>
        public int ScrutinyRevenuePpm;
        /// <summary>Attention added to the home bloc each scrutiny month.</summary>
        public int ScrutinyAttentionPerMonth;
        /// <summary>Extra antitrust attention on every bloc when a monopoly ending fires.</summary>
        public int MonopolyRegulatorAttention;

        public void Sync(Syncer s)
        {
            s.Int("bubbleScrutinyAtPermille", ref BubbleScrutinyAtPermille);
            s.Int("scrutinyRevenuePpm", ref ScrutinyRevenuePpm);
            s.Int("scrutinyAttentionPerMonth", ref ScrutinyAttentionPerMonth);
            s.Int("monopolyRegulatorAttention", ref MonopolyRegulatorAttention);
        }
    }
}