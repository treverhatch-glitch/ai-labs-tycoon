namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough268PresentationSpec
    {
        public const int LastDocumentedPushBatch = 268;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough265PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough196PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-268.md";
    }
}