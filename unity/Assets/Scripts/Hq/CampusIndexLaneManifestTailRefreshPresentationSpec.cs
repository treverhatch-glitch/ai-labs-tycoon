namespace AiLabs.Unity.Hq
{
    /// <summary>Manifest tail refresh (see INDEX-LANE-MANIFEST-TAIL-REFRESH-104.md).</summary>
    public static class CampusIndexLaneManifestTailRefreshPresentationSpec
    {
        public const int RefreshThroughBatch = 104;

        public static int PriorTailBatchEnd =>
            CampusIndexLaneManifestTailPresentationSpec.TailBatchEnd;

        public static int ExtendedLaneEntryCount =>
            CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneEntryCount;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB104PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-104.md";
    }
}