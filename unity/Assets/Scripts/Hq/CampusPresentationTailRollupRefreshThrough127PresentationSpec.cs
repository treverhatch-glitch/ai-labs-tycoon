namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 127 (see PRESENTATION-TAIL-ROLLUP-REFRESH-127.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough127PresentationSpec
    {
        public const int RefreshThroughBatch = 127;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough121PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-127.md";
    }
}