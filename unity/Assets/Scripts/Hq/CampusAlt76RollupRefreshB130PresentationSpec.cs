namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 130 (see ALT-76-ROLLUP-REFRESH-130.md).</summary>
    public static class CampusAlt76RollupRefreshB130PresentationSpec
    {
        public const int LastDocumentedBatch = 130;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB127PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough130PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-130.md";
    }
}