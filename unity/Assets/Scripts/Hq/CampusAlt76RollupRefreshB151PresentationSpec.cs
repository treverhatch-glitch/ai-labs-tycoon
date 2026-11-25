namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB151PresentationSpec
    {
        public const int LastDocumentedBatch = 151;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB148PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough151PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-151.md";
    }
}