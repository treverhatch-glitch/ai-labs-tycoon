namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough172PresentationSpec
    {
        public const int LastDocumentedPushBatch = 172;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough169PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough148PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough145PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-172.md";
    }
}