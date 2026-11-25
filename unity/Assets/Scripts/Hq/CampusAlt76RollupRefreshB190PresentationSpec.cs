namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB190PresentationSpec
    {
        public const int LastDocumentedBatch = 190;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB187PresentationSpec.LastDocumentedBatch;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncThrough190PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-190.md";
    }
}