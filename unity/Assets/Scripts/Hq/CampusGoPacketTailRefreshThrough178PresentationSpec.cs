namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough178PresentationSpec
    {
        public const int LastDocumentedPushBatch = 178;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough175PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough151PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough148PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-178.md";
    }
}