namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 86 (see ALT-76-STATUS-SYNC-86.md).</summary>
    public static class CampusAlt76StatusSyncThrough86PresentationSpec
    {
        public const int LastSyncedBatch = 86;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncPresentationSpec.LastSyncedBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB85PresentationSpec.LastDocumentedBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-86.md";
    }
}