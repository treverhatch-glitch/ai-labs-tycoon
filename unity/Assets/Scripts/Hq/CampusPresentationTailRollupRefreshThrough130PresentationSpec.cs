namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 130 (see PRESENTATION-TAIL-ROLLUP-REFRESH-130.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough130PresentationSpec
    {
        public const int RefreshThroughBatch = 130;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-130.md";
    }
}