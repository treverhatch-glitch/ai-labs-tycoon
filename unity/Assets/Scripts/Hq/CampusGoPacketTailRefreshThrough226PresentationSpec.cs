namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough226PresentationSpec
    {
        public const int LastDocumentedPushBatch = 226;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough223PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough175PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-226.md";
    }
}