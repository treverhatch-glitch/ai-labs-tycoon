namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough175PresentationSpec
    {
        public const int RefreshThroughBatch = 175;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough175PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-175.md";
    }
}