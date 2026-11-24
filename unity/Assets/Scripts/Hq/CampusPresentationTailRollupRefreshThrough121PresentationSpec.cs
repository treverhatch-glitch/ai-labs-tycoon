namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 121 (see PRESENTATION-TAIL-ROLLUP-REFRESH-121.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough121PresentationSpec
    {
        public const int RefreshThroughBatch = 121;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough118PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-121.md";
    }
}