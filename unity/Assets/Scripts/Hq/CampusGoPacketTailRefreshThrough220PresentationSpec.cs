namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough220PresentationSpec
    {
        public const int LastDocumentedPushBatch = 220;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough217PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough172PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough169PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-220.md";
    }
}