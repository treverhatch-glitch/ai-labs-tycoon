namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 98 (see ALT-76-STATUS-SYNC-98.md).</summary>
    public static class CampusAlt76StatusSyncThrough98PresentationSpec
    {
        public const int LastSyncedBatch = 98;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough89PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough98PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-98.md";
    }
}