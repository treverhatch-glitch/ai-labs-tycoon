namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough217PresentationSpec
    {
        public const int LastSyncedBatch = 217;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough214PresentationSpec.LastSyncedBatch;

        public static int SeasonalChromeLastBatch =>
            CampusSeasonalA11yChromeTailRefreshThrough315PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-217.md";
    }
}