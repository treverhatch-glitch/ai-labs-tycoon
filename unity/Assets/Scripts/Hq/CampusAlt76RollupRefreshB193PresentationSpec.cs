namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB193PresentationSpec
    {
        public const int LastDocumentedBatch = 193;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB190PresentationSpec.LastDocumentedBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough193PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-193.md";
    }
}