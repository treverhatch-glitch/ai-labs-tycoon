namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 104 (see ALT-76-ROLLUP-REFRESH-104.md).</summary>
    public static class CampusAlt76RollupRefreshB104PresentationSpec
    {
        public const int LastDocumentedBatch = 104;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB100PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough102PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-104.md";
    }
}