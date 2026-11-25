namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough211PresentationSpec
    {
        public const int LastDocumentedPushBatch = 211;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough208PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough166PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB166PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-211.md";
    }
}