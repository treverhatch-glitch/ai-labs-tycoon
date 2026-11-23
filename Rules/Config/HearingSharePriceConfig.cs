using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Public-float labs nudge market bubble permille after hearing answers (Q469, schema 145).</summary>
    public sealed class HearingSharePriceConfig : ISync
    {
        public int Enabled;
        /// <summary>Minimum public float ppm before the ticker moves.</summary>
        public int MinFloatPpm;
        /// <summary>Bubble permille added after a hearing answer with net positive reputation.</summary>
        public int UpPermille;
        /// <summary>Bubble permille removed after a hearing answer with net negative reputation.</summary>
        public int DownPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minFloatPpm", ref MinFloatPpm);
            s.Int("upPermille", ref UpPermille);
            s.Int("downPermille", ref DownPermille);
        }
    }
}