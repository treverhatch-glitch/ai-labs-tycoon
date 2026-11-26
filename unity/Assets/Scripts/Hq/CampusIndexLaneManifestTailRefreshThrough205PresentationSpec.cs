namespace AiLabs.Unity.Hq
{
    public static class CampusIndexLaneManifestTailRefreshThrough205PresentationSpec
    {
        public const int RefreshThroughBatch = 205;

        public static int PriorRefreshThroughBatch =>
            CampusIndexLaneManifestTailRefreshThrough202PresentationSpec.RefreshThroughBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough280PresentationSpec.LastDocumentedPushBatch;

        public const string RefreshDoc = "INDEX-LANE-MANIFEST-TAIL-REFRESH-205.md";
    }
}