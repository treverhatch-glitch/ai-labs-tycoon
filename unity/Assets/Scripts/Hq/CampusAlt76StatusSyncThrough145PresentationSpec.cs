namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough145PresentationSpec
    {
        public const int LastSyncedBatch = 145;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough142PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough163PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough142PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-145.md";
    }
}