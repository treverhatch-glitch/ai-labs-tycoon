namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 130 (see ALT-76-STATUS-SYNC-130.md).</summary>
    public static class CampusAlt76StatusSyncThrough130PresentationSpec
    {
        public const int LastSyncedBatch = 130;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough127PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough133PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-130.md";
    }
}