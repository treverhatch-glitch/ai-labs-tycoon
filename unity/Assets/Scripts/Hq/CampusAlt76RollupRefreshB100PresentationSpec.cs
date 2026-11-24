namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 100 (see ALT-76-ROLLUP-REFRESH-100.md).</summary>
    public static class CampusAlt76RollupRefreshB100PresentationSpec
    {
        public const int LastDocumentedBatch = 100;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB89PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough98PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-100.md";
    }
}