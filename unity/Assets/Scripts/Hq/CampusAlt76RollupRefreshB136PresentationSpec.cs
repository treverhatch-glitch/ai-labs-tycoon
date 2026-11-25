namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 136 (see ALT-76-ROLLUP-REFRESH-136.md).</summary>
    public static class CampusAlt76RollupRefreshB136PresentationSpec
    {
        public const int LastDocumentedBatch = 136;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB133PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough136PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-136.md";
    }
}