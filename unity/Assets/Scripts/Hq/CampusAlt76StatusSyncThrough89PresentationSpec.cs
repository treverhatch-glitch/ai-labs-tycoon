namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 89 (see ALT-76-STATUS-SYNC-89.md).</summary>
    public static class CampusAlt76StatusSyncThrough89PresentationSpec
    {
        public const int LastSyncedBatch = 89;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough86PresentationSpec.LastSyncedBatch;

        public static int CaptureTailEntries =>
            CampusCaptureHudTailIndexPresentationSpec.TailEntryCount;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-89.md";
    }
}