namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 136 (see ALT-76-STATUS-SYNC-136.md).</summary>
    public static class CampusAlt76StatusSyncThrough136PresentationSpec
    {
        public const int LastSyncedBatch = 136;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough133PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough145PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-136.md";
    }
}