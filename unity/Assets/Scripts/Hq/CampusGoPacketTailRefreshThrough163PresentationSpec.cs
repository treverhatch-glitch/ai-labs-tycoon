namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough163PresentationSpec
    {
        public const int LastDocumentedPushBatch = 163;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough160PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough142PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough160PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-163.md";
    }
}