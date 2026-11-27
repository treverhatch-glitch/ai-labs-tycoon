namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB208PresentationSpec
    {
        public const int LastDocumentedBatch = 208;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB205PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough211PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-208.md";
    }
}