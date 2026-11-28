namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough244PresentationSpec
    {
        public const int LastDocumentedPushBatch = 244;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough241PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough181PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-244.md";
    }
}