namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB172PresentationSpec
    {
        public const int LastDocumentedBatch = 172;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB169PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough172PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-172.md";
    }
}