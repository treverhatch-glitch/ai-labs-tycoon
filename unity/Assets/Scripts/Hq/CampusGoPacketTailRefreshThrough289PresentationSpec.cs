namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough289PresentationSpec
    {
        public const int LastDocumentedPushBatch = 289;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough286PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough211PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-289.md";
    }
}