namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB181PresentationSpec
    {
        public const int LastDocumentedBatch = 181;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB178PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough181PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-181.md";
    }
}