namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB145PresentationSpec
    {
        public const int LastDocumentedBatch = 145;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB142PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough145PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-145.md";
    }
}