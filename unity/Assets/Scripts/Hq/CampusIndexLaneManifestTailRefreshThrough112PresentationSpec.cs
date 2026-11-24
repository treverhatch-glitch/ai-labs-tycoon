namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 112 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-112.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough112PresentationSpec
    {
        public const int RefreshThroughBatch = 112;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshPresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB112PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough109PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-112.md";
    }
}