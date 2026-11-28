namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough241PresentationSpec
    {
        public const int LastDocumentedPushBatch = 241;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough238PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough181PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-241.md";
    }
}