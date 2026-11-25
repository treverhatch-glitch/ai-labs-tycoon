namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough139PresentationSpec
    {
        public const int LastSyncedBatch = 139;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough136PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough151PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-139.md";
    }
}