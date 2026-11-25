namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough160PresentationSpec
    {
        public const int LastSyncedBatch = 160;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough157PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough193PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough157PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-160.md";
    }
}