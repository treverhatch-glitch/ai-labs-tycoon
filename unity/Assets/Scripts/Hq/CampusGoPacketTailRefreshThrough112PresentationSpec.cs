namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 112 (see GO-PACKET-TAIL-REFRESH-112.md).</summary>
    public static class CampusGoPacketTailRefreshThrough112PresentationSpec
    {
        public const int LastDocumentedPushBatch = 112;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough109PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB112PresentationSpec.LastDocumentedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough109PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-112.md";
    }
}