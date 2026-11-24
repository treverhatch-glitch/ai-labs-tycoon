namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 133 (see ALT-76-STATUS-SYNC-133.md).</summary>
    public static class CampusAlt76StatusSyncThrough133PresentationSpec
    {
        public const int LastSyncedBatch = 133;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough130PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough139PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-133.md";
    }
}