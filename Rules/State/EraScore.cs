using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>How the lab did against the goals of one era, marked when that era ended.</summary>
    public sealed class EraScore : ISync
    {
        public string EraId = "";
        /// <summary>Goals met.</summary>
        public int Met;
        /// <summary>Goals there were.</summary>
        public int Of;
        /// <summary>Points earned, the all-met bonus included.</summary>
        public int Points;

        public void Sync(Syncer s)
        {
            s.Str("eraId", ref EraId);
            s.Int("met", ref Met);
            s.Int("of", ref Of);
            s.Int("points", ref Points);
        }
    }
}
