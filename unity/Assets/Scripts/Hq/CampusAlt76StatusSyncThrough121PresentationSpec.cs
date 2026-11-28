namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 121 (see ALT-76-STATUS-SYNC-121.md).</summary>
    public static class CampusAlt76StatusSyncThrough121PresentationSpec
    {
        public const int LastSyncedBatch = 121;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough118PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough121PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough118PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-121.md";
    }
}