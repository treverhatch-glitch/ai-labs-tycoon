namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough193PresentationSpec
    {
        public const int LastDocumentedPushBatch = 193;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough190PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough157PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB157PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-193.md";
    }
}