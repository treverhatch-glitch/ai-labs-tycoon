namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough154PresentationSpec
    {
        public const int LastSyncedBatch = 154;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough151PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough181PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough151PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-154.md";
    }
}