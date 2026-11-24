namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 115 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-115.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough115PresentationSpec
    {
        public const int RefreshThroughBatch = 115;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB115PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough115PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-115.md";
    }
}