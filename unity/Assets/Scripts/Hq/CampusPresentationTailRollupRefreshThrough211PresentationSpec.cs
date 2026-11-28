namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough211PresentationSpec
    {
        public const int RefreshThroughBatch = 211;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough208PresentationSpec.RefreshThroughBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough214PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-211.md";
    }
}