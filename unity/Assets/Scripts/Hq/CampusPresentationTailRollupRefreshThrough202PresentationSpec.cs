namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough202PresentationSpec
    {
        public const int RefreshThroughBatch = 202;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough199PresentationSpec.RefreshThroughBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough205PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-202.md";
    }
}