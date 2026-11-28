namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 127 (see ALT-76-STATUS-SYNC-127.md).</summary>
    public static class CampusAlt76StatusSyncThrough127PresentationSpec
    {
        public const int LastSyncedBatch = 127;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough121PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough127PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough121PresentationSpec.RefreshThroughBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-127.md";
    }
}