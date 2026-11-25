namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough166PresentationSpec
    {
        public const int LastSyncedBatch = 166;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough163PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough205PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough163PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-166.md";
    }
}