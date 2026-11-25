namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough205PresentationSpec
    {
        public const int LastDocumentedPushBatch = 205;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough202PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough163PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB163PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-205.md";
    }
}