namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough205PresentationSpec
    {
        public const int RefreshThroughBatch = 205;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough202PresentationSpec.RefreshThroughBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough208PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-205.md";
    }
}