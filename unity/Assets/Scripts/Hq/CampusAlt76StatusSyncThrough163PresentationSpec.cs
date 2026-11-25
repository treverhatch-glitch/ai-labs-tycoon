namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough163PresentationSpec
    {
        public const int LastSyncedBatch = 163;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough160PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough199PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough160PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-163.md";
    }
}