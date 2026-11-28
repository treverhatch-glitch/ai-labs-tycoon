namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 106 (see GO-PACKET-TAIL-REFRESH-106.md).</summary>
    public static class CampusGoPacketTailRefreshThrough106PresentationSpec
    {
        public const int LastDocumentedPushBatch = 106;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough102PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestTailRefreshBatch => 107;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough102PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-106.md";
    }
}