namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB214PresentationSpec
    {
        public const int LastDocumentedBatch = 214;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB211PresentationSpec.LastDocumentedBatch;

        public static int PresentationRollupBatch =>
            CampusPresentationTailRollupRefreshThrough211PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-214.md";
    }
}