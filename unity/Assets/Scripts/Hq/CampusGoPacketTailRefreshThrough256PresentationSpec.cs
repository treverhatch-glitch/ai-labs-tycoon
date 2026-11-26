namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough256PresentationSpec
    {
        public const int LastDocumentedPushBatch = 256;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough253PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough187PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-256.md";
    }
}