namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough199PresentationSpec
    {
        public const int LastDocumentedPushBatch = 199;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough196PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough160PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB160PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-199.md";
    }
}