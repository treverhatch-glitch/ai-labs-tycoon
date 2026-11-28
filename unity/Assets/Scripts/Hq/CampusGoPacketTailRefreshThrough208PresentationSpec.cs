namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough208PresentationSpec
    {
        public const int LastDocumentedPushBatch = 208;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough205PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough166PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough163PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-208.md";
    }
}