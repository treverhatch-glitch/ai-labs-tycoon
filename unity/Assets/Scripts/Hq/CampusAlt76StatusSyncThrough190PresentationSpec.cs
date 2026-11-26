namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough190PresentationSpec
    {
        public const int LastSyncedBatch = 190;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough187PresentationSpec.LastSyncedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough253PresentationSpec.LastDocumentedPushBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-190.md";
    }
}