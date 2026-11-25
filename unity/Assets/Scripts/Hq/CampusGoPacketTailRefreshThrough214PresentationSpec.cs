namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough214PresentationSpec
    {
        public const int LastDocumentedPushBatch = 214;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough211PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough169PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough166PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-214.md";
    }
}