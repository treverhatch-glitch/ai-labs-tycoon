namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 115 (see ALT-76-STATUS-SYNC-115.md).</summary>
    public static class CampusAlt76StatusSyncThrough115PresentationSpec
    {
        public const int LastSyncedBatch = 115;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough109PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough115PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-115.md";
    }
}