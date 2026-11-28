namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough139PresentationSpec
    {
        public const int RefreshThroughBatch = 139;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough136PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough139PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-139.md";
    }
}