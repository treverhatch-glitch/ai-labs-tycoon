namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 133 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-133.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough133PresentationSpec
    {
        public const int RefreshThroughBatch = 133;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB133PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough133PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-133.md";
    }
}