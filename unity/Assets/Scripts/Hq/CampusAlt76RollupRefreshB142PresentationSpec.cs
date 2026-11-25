namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB142PresentationSpec
    {
        public const int LastDocumentedBatch = 142;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB139PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough142PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-142.md";
    }
}