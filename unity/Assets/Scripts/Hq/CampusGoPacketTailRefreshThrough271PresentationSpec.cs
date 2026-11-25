namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough271PresentationSpec
    {
        public const int LastDocumentedPushBatch = 271;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough268PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupBatch =>
            CampusPresentationTailRollupRefreshThrough196PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-271.md";
    }
}