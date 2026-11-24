namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 127 (see ALT-76-ROLLUP-REFRESH-127.md).</summary>
    public static class CampusAlt76RollupRefreshB127PresentationSpec
    {
        public const int LastDocumentedBatch = 127;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB121PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough127PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-127.md";
    }
}