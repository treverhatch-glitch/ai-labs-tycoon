namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough172PresentationSpec
    {
        public const int LastSyncedBatch = 172;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough169PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough217PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough169PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-172.md";
    }
}