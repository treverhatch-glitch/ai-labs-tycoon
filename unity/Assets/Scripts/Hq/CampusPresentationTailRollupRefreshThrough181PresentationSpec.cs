namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough181PresentationSpec
    {
        public const int RefreshThroughBatch = 181;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough178PresentationSpec.RefreshThroughBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough181PresentationSpec.RefreshThroughBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough184PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-181.md";
    }
}