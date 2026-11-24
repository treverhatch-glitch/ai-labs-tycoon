namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 112 (see PRESENTATION-TAIL-ROLLUP-REFRESH-112.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough112PresentationSpec
    {
        public const int RefreshThroughBatch = 112;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough106PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-112.md";
    }
}