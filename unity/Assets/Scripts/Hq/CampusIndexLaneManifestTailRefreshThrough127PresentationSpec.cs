namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh through 127 (see INDEX-LANE-MANIFEST-TAIL-REFRESH-127.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshThrough127PresentationSpec
    {
        public const int RefreshThroughBatch = 127;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.RefreshThroughBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB127PresentationSpec.LastDocumentedBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough127PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-127.md";
    }
}