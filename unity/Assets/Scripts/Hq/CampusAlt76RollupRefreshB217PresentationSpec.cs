namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB217PresentationSpec
    {
        public const int LastDocumentedBatch = 217;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB214PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough217PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-217.md";
    }
}