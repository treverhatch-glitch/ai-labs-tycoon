namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough178PresentationSpec
    {
        public const int LastSyncedBatch = 178;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough175PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough229PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough175PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-178.md";
    }
}