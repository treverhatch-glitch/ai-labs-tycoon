namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough172PresentationSpec
    {
        public const int RefreshThroughBatch = 172;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough169PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-172.md";
    }
}