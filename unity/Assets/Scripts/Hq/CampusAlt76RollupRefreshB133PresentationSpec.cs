namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 133 (see ALT-76-ROLLUP-REFRESH-133.md).</summary>
    public static class CampusAlt76RollupRefreshB133PresentationSpec
    {
        public const int LastDocumentedBatch = 133;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB130PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough133PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-133.md";
    }
}