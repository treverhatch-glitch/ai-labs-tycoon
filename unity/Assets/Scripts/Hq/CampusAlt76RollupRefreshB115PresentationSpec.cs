namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 115 (see ALT-76-ROLLUP-REFRESH-115.md).</summary>
    public static class CampusAlt76RollupRefreshB115PresentationSpec
    {
        public const int LastDocumentedBatch = 115;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB112PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough115PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-115.md";
    }
}