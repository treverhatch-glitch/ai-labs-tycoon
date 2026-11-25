namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough181PresentationSpec
    {
        public const int LastDocumentedPushBatch = 181;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough178PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough151PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB151PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-181.md";
    }
}