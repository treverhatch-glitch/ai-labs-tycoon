namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 118 (see PRESENTATION-TAIL-ROLLUP-REFRESH-118.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough118PresentationSpec
    {
        public const int RefreshThroughBatch = 118;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough115PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-118.md";
    }
}