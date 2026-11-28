namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB175PresentationSpec
    {
        public const int LastDocumentedBatch = 175;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB172PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough175PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-175.md";
    }
}