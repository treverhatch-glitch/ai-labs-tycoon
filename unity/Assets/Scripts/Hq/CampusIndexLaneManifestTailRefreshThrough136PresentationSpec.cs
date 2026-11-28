namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 136 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-136.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough136PresentationSpec
    {
        public const int RefreshThroughBatch = 136;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB136PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough136PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-136.md";
    }
}