namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough211PresentationSpec
    {
        public const int LastSyncedBatch = 211;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough208PresentationSpec.LastSyncedBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough208PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-211.md";
    }
}