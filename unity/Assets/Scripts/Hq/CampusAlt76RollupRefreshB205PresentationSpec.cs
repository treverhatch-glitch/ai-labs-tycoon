namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB205PresentationSpec
    {
        public const int LastDocumentedBatch = 205;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB202PresentationSpec.LastDocumentedBatch;

        public static int RivalWorldTailBatch =>
            CampusRivalWorldTailIndexRefreshThrough300PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-205.md";
    }
}