namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough166PresentationSpec
    {
        public const int LastDocumentedPushBatch = 166;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough163PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough145PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough142PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-166.md";
    }
}