namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through batch 104 (see ALT-76-STATUS-SYNC-104.md).</summary>
    public static class CampusAlt76StatusSyncThrough104PresentationSpec
    {
        public const int LastSyncedBatch = 104;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough102PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough106PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-104.md";
    }
}