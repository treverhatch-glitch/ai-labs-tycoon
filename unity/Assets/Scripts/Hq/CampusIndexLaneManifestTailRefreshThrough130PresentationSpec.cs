namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 130 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-130.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough130PresentationSpec
    {
        public const int RefreshThroughBatch = 130;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB130PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough130PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-130.md";
    }
}