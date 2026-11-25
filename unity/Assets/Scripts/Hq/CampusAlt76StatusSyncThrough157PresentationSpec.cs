namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough157PresentationSpec
    {
        public const int LastSyncedBatch = 157;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough154PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough187PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough154PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-157.md";
    }
}