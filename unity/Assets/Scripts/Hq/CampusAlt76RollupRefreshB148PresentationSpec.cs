namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB148PresentationSpec
    {
        public const int LastDocumentedBatch = 148;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB145PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough148PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-148.md";
    }
}