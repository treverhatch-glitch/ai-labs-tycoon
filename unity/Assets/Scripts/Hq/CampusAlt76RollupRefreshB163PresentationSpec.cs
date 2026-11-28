namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB163PresentationSpec
    {
        public const int LastDocumentedBatch = 163;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB160PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough163PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-163.md";
    }
}