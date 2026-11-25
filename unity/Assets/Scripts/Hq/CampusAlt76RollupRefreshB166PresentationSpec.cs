namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB166PresentationSpec
    {
        public const int LastDocumentedBatch = 166;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB163PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough166PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-166.md";
    }
}