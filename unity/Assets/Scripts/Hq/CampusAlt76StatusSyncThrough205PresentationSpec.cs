namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough205PresentationSpec
    {
        public const int LastSyncedBatch = 205;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough202PresentationSpec.LastSyncedBatch;

        public static int SeasonalChromeLastBatch =>
            CampusSeasonalA11yChromeTailRefreshThrough300PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-205.md";
    }
}