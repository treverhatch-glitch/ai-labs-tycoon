using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Reputation and cooldown gates on reserved GPU contracts (Q474, schema 149).</summary>
    public sealed class ReservedComputeGateConfig : ISync
    {
        public int Enabled;
        public int MinReputationPermille;
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minReputationPermille", ref MinReputationPermille);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}