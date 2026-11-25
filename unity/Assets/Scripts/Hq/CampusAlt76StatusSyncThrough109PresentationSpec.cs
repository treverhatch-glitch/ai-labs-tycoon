namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 109 (see ALT-76-STATUS-SYNC-109.md).</summary>
    public static class CampusAlt76StatusSyncThrough109PresentationSpec
    {
        public const int LastSyncedBatch = 109;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough104PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough109PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-109.md";
    }
}