namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB169PresentationSpec
    {
        public const int LastDocumentedBatch = 169;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB166PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough169PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-169.md";
    }
}