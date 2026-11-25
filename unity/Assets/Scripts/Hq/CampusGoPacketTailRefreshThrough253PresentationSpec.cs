namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough253PresentationSpec
    {
        public const int LastDocumentedPushBatch = 253;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough250PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough187PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-253.md";
    }
}