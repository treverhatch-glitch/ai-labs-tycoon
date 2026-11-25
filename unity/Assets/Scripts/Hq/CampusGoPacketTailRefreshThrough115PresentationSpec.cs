namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 115 (see GO-PACKET-TAIL-REFRESH-115.md).</summary>
    public static class CampusGoPacketTailRefreshThrough115PresentationSpec
    {
        public const int LastDocumentedPushBatch = 115;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough112PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough112PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough112PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-115.md";
    }
}