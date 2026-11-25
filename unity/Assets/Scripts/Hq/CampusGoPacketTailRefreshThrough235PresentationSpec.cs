namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough235PresentationSpec
    {
        public const int LastDocumentedPushBatch = 235;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough232PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough178PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB178PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-235.md";
    }
}