namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through 133 (see PRESENTATION-TAIL-ROLLUP-REFRESH-133.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough133PresentationSpec
    {
        public const int RefreshThroughBatch = 133;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-133.md";
    }
}