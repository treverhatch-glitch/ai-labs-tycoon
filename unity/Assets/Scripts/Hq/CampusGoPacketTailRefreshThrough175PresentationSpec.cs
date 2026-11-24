namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough175PresentationSpec
    {
        public const int LastDocumentedPushBatch = 175;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough172PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough148PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB148PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-175.md";
    }
}