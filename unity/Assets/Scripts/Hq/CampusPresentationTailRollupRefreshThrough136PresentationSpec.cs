namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough136PresentationSpec
    {
        public const int RefreshThroughBatch = 136;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-136.md";
    }
}