namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough175PresentationSpec
    {
        public const int LastSyncedBatch = 175;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough172PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough223PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-175.md";
    }
}