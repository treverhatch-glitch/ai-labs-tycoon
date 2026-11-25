namespace AiLabs.Unity.Hq
{
    /// <summary>Tail rollup refresh through batch 106 (see PRESENTATION-TAIL-ROLLUP-REFRESH-106.md).</summary>
    public static class CampusPresentationTailRollupRefreshThrough106PresentationSpec
    {
        public const int RefreshThroughBatch = 106;

        public static int TailLaneCount =>
            CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes;

        public static int LastRemoteGroupedBatch => 109;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshPresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-106.md";
    }
}