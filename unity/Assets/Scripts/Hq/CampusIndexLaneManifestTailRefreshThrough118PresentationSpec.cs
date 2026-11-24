namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 118 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-118.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough118PresentationSpec
    {
        public const int RefreshThroughBatch = 118;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB118PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough118PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-118.md";
    }
}