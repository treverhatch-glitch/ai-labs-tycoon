namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough169PresentationSpec
    {
        public const int LastDocumentedPushBatch = 169;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough166PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough145PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB145PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-169.md";
    }
}