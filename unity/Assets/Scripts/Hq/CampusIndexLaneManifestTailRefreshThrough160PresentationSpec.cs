namespace AiLabs.Unity.Hq
{
    public static class CampusIndexLaneManifestTailRefreshThrough160PresentationSpec
    {
        public const int RefreshThroughBatch = 160;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough157PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB160PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough160PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-160.md";
    }
}