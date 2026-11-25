namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough181PresentationSpec
    {
        public const int LastSyncedBatch = 181;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough178PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough235PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough178PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-181.md";
    }
}