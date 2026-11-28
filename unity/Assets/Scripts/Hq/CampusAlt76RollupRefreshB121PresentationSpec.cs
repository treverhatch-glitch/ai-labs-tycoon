namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 121 (see ALT-76-ROLLUP-REFRESH-121.md).</summary>
    public static class CampusAlt76RollupRefreshB121PresentationSpec
    {
        public const int LastDocumentedBatch = 121;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB118PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough121PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-121.md";
    }
}