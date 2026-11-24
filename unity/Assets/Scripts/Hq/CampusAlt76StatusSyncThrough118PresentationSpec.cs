namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 118 (see ALT-76-STATUS-SYNC-118.md).</summary>
    public static class CampusAlt76StatusSyncThrough118PresentationSpec
    {
        public const int LastSyncedBatch = 118;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough115PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough118PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough115PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-118.md";
    }
}