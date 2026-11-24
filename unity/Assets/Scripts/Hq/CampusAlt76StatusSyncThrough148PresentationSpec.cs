namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough148PresentationSpec
    {
        public const int LastSyncedBatch = 148;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough145PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough169PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough145PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-148.md";
    }
}