namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough151PresentationSpec
    {
        public const int LastSyncedBatch = 151;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough148PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough175PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough148PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-151.md";
    }
}