namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough223PresentationSpec
    {
        public const int LastDocumentedPushBatch = 223;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough220PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB172PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-223.md";
    }
}