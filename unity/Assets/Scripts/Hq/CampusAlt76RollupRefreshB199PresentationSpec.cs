namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB199PresentationSpec
    {
        public const int LastDocumentedBatch = 199;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB196PresentationSpec.LastDocumentedBatch;

        public static int RivalWorldTailBatch =>
            CampusRivalWorldTailIndexRefreshThrough288PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-199.md";
    }
}