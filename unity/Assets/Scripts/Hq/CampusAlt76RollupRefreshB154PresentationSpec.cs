namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB154PresentationSpec
    {
        public const int LastDocumentedBatch = 154;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB151PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough154PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-154.md";
    }
}