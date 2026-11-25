namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 89 (see ALT-76-ROLLUP-REFRESH-89.md).</summary>
    public static class CampusAlt76RollupRefreshB89PresentationSpec
    {
        public const int LastDocumentedBatch = 89;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB85PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough89PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-89.md";
    }
}