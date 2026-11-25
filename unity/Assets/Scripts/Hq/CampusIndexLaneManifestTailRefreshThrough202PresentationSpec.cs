namespace AiLabs.Unity.Hq
{
    public static class CampusIndexLaneManifestTailRefreshThrough202PresentationSpec
    {
        public const int RefreshThroughBatch = 202;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough199PresentationSpec.RefreshThroughBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough274PresentationSpec.LastDocumentedPushBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-202.md";
    }
}