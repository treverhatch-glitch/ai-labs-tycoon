namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough202PresentationSpec
    {
        public const int LastSyncedBatch = 202;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough199PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough274PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-202.md";
    }
}