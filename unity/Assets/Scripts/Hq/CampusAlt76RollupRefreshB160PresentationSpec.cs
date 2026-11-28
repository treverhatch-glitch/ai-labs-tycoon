namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB160PresentationSpec
    {
        public const int LastDocumentedBatch = 160;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB157PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough160PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-160.md";
    }
}