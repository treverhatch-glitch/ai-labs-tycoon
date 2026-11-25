namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough169PresentationSpec
    {
        public const int LastSyncedBatch = 169;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough166PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough211PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough166PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-169.md";
    }
}