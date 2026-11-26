namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough208PresentationSpec
    {
        public const int RefreshThroughBatch = 208;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough205PresentationSpec.RefreshThroughBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough211PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-208.md";
    }
}