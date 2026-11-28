namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 118 (see GO-PACKET-TAIL-REFRESH-118.md).</summary>
    public static class CampusGoPacketTailRefreshThrough118PresentationSpec
    {
        public const int LastDocumentedPushBatch = 118;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough115PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB115PresentationSpec.LastDocumentedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough115PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-118.md";
    }
}