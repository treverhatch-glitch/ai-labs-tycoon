namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough259PresentationSpec
    {
        public const int LastDocumentedPushBatch = 259;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough256PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough190PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-259.md";
    }
}