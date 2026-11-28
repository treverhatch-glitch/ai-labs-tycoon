namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 106 (see ALT-76-ROLLUP-REFRESH-106.md).</summary>
    public static class CampusAlt76RollupRefreshB106PresentationSpec
    {
        public const int LastDocumentedBatch = 106;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB104PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough104PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-106.md";
    }
}