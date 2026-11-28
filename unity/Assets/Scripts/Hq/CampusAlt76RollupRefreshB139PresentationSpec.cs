namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB139PresentationSpec
    {
        public const int LastDocumentedBatch = 139;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB136PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough139PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-139.md";
    }
}