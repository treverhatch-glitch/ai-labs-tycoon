namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 115 (see PRESENTATION-TAIL-ROLLUP-REFRESH-115.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough115PresentationSpec
    {
        public const int RefreshThroughBatch = 115;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough112PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-115.md";
    }
}