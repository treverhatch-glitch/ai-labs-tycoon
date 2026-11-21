using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Endings when a released model crosses into superintelligence band.</summary>
    public sealed class SuperintelligenceConfig : ISync
    {
        public int ReleasedCapabilityMin;
        public int PaperclipMaxAlignment;
        public int LongSummerMinAlignment;
        public int LongSummerMinFrontier;
        public bool LooseRequiresZeroContainment;

        public void Sync(Syncer s)
        {
            s.Int("releasedCapabilityMin", ref ReleasedCapabilityMin);
            s.Int("paperclipMaxAlignment", ref PaperclipMaxAlignment);
            s.Int("longSummerMinAlignment", ref LongSummerMinAlignment);
            s.Int("longSummerMinFrontier", ref LongSummerMinFrontier);
            s.Bool("looseRequiresZeroContainment", ref LooseRequiresZeroContainment);
        }
    }
}