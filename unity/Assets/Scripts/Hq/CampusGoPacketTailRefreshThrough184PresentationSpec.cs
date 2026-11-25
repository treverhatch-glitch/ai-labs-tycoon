namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough184PresentationSpec
    {
        public const int LastDocumentedPushBatch = 184;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough181PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough154PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough151PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-184.md";
    }
}