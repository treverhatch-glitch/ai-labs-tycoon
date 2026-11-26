namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough193PresentationSpec
    {
        public const int LastSyncedBatch = 193;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough190PresentationSpec.LastSyncedBatch;

        public static int RivalsHudLastBatch =>
            CampusRivalsScreenHudTailPresentationSpec.PostRollupBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-193.md";
    }
}