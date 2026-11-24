namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 102 (see GO-PACKET-TAIL-REFRESH-102.md).</summary>
    public static class CampusGoPacketTailRefreshThrough102PresentationSpec
    {
        public const int LastDocumentedPushBatch = 102;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough98PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusPresentationTailRollupRefreshPresentationSpec.RollupRefreshLastBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough98PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-102.md";
    }
}