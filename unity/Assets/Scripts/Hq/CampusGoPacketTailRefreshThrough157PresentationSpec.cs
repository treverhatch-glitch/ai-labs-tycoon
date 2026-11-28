namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough157PresentationSpec
    {
        public const int LastDocumentedPushBatch = 157;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough154PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough139PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough154PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-157.md";
    }
}