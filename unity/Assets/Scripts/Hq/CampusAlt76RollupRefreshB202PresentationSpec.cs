namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB202PresentationSpec
    {
        public const int LastDocumentedBatch = 202;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB199PresentationSpec.LastDocumentedBatch;

        public static int HudTailRefreshBatch =>
            CampusExplainableHudTailRefreshThrough300PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-202.md";
    }
}