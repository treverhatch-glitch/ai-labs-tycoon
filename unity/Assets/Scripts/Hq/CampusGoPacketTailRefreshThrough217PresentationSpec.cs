namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough217PresentationSpec
    {
        public const int LastDocumentedPushBatch = 217;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough214PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough169PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB169PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-217.md";
    }
}