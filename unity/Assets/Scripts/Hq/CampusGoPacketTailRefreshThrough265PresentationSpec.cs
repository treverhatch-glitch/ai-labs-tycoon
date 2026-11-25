namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough265PresentationSpec
    {
        public const int LastDocumentedPushBatch = 265;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough262PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupBatch =>
            CampusPresentationTailRollupRefreshThrough193PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-265.md";
    }
}