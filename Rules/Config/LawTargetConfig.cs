using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Pick strict or loose before lobby spend on law shaping (Q472, schema 148).</summary>
    public sealed class LawTargetConfig : ISync
    {
        public int Enabled;

        public void Sync(Syncer s) { s.Int("enabled", ref Enabled); }
    }
}