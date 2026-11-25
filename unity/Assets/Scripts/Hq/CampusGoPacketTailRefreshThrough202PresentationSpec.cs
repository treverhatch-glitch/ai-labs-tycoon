namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough202PresentationSpec
    {
        public const int LastDocumentedPushBatch = 202;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough199PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough163PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough160PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-202.md";
    }
}