namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough187PresentationSpec
    {
        public const int LastDocumentedPushBatch = 187;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough184PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough154PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB154PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-187.md";
    }
}