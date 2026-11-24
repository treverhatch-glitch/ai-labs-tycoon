namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 112 (see ALT-76-ROLLUP-REFRESH-112.md).</summary>
    public static class CampusAlt76RollupRefreshB112PresentationSpec
    {
        public const int LastDocumentedBatch = 112;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB106PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough109PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-112.md";
    }
}