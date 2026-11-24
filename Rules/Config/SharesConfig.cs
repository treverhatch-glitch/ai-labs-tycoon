using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Public float for plan-run rivals — the circular financing hook with the bubble meter.</summary>
    public sealed class SharesConfig : ISync
    {
        /// <summary>Rivals that list shares start with this much of the lab on the market, in ppm.</summary>
        public int RivalStartPublicFloatPpm;
        public int RivalMaxPublicFloatPpm;
        /// <summary>Each month the bubble is at or above this, rivals can sell more stock.</summary>
        public int IpoWhenBubbleAtPermille;
        /// <summary>How much public float rises per hot month.</summary>
        public int IpoStepPpm;
        /// <summary>Each IPO step also pushes the shared bubble meter up by this much.</summary>
        public int IpoBubbleBoostPermille;

        public void Sync(Syncer s)
        {
            s.Int("rivalStartPublicFloatPpm", ref RivalStartPublicFloatPpm);
            s.Int("rivalMaxPublicFloatPpm", ref RivalMaxPublicFloatPpm);
            s.Int("ipoWhenBubbleAtPermille", ref IpoWhenBubbleAtPermille);
            s.Int("ipoStepPpm", ref IpoStepPpm);
            s.Int("ipoBubbleBoostPermille", ref IpoBubbleBoostPermille);
        }
    }
}