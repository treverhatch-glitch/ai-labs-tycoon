namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 145 (see GO-PACKET-TAIL-REFRESH-145.md).</summary>
    public static class CampusGoPacketTailRefreshThrough145PresentationSpec
    {
        public const int LastDocumentedPushBatch = 145;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough142PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough142PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-145.md";
    }
}