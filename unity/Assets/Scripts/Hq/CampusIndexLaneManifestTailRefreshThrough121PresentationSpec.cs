namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 121 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-121.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough121PresentationSpec
    {
        public const int RefreshThroughBatch = 121;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB121PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough121PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-121.md";
    }
}