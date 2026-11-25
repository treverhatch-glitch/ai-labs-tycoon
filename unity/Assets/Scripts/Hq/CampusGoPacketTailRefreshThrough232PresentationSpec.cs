namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough232PresentationSpec
    {
        public const int LastDocumentedPushBatch = 232;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough229PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough178PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough175PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-232.md";
    }
}