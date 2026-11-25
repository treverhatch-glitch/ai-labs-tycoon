namespace AiLabs.Unity.Hq
{
    public static class CampusIndexLaneManifestTailRefreshThrough175PresentationSpec
    {
        public const int RefreshThroughBatch = 175;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough172PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB175PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough175PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-175.md";
    }
}