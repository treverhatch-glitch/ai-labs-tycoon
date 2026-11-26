namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough208PresentationSpec
    {
        public const int LastSyncedBatch = 208;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough205PresentationSpec.LastSyncedBatch;

        public static int RivalWorldTailBatch =>
            CampusRivalWorldTailIndexRefreshThrough300PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-208.md";
    }
}