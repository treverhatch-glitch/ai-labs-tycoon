namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough160PresentationSpec
    {
        public const int RefreshThroughBatch = 160;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough157PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough160PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-160.md";
    }
}