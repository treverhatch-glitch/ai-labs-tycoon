namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough247PresentationSpec
    {
        public const int LastDocumentedPushBatch = 247;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough244PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough184PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-247.md";
    }
}