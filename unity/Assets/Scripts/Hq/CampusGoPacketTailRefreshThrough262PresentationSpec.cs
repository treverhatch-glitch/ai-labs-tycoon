namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough262PresentationSpec
    {
        public const int LastDocumentedPushBatch = 262;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough259PresentationSpec.LastDocumentedPushBatch;

        public static int PresentationTailBatch =>
            CampusPresentationTailRollupRefreshThrough190PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-262.md";
    }
}