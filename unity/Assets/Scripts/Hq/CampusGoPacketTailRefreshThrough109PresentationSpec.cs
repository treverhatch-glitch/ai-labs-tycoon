namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 109 (see GO-PACKET-TAIL-REFRESH-109.md).</summary>
    public static class CampusGoPacketTailRefreshThrough109PresentationSpec
    {
        public const int LastDocumentedPushBatch = 109;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough106PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB106PresentationSpec.LastDocumentedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough106PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-109.md";
    }
}