namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 121 (see GO-PACKET-TAIL-REFRESH-121.md).</summary>
    public static class CampusGoPacketTailRefreshThrough121PresentationSpec
    {
        public const int LastDocumentedPushBatch = 121;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough118PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB118PresentationSpec.LastDocumentedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough118PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-121.md";
    }
}