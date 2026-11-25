namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough196PresentationSpec
    {
        public const int LastDocumentedPushBatch = 196;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough193PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough160PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough157PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-196.md";
    }
}