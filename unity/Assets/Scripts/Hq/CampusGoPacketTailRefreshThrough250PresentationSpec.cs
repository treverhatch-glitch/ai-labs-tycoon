namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough250PresentationSpec
    {
        public const int LastDocumentedPushBatch = 250;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough247PresentationSpec.LastDocumentedPushBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough184PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-250.md";
    }
}