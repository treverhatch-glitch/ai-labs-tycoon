namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 102 (see ALT-76-STATUS-SYNC-102.md).</summary>
    public static class CampusAlt76StatusSyncThrough102PresentationSpec
    {
        public const int LastSyncedBatch = 102;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough98PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough102PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-102.md";
    }
}