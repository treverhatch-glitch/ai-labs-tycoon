using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Lobby spend to bias the next era's alignment law (Q463, schema 140).</summary>
    public sealed class LawShapingConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        /// <summary>Permille added to release bar when shaping for stricter law.</summary>
        public int StrictDelta;
        /// <summary>Permille taken from release bar when shaping for looser law.</summary>
        public int LooseDelta;
        /// <summary>Attention on home bloc above which stacked nudges are damped, permille scale.</summary>
        public int RegulatorAttentionDampFrom;
        public int RegulatorDampPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("strictDelta", ref StrictDelta);
            s.Int("looseDelta", ref LooseDelta);
            s.Int("regulatorAttentionDampFrom", ref RegulatorAttentionDampFrom);
            s.Int("regulatorDampPermille", ref RegulatorDampPermille);
        }
    }
}