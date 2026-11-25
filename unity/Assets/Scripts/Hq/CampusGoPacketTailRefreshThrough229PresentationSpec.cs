namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough229PresentationSpec
    {
        public const int LastDocumentedPushBatch = 229;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough226PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough175PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB175PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-229.md";
    }
}