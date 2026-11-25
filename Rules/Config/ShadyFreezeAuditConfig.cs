using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Refuse shady jobs while home-bloc permits are frozen (Q478, schema 153).</summary>
    public sealed class ShadyFreezeAuditConfig : ISync
    {
        public int Enabled;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
        }
    }
}