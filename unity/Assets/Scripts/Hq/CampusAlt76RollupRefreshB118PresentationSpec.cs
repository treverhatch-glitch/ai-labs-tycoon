namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 118 (see ALT-76-ROLLUP-REFRESH-118.md).</summary>
    public static class CampusAlt76RollupRefreshB118PresentationSpec
    {
        public const int LastDocumentedBatch = 118;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB115PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough118PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-118.md";
    }
}