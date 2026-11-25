namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough280PresentationSpec
    {
        public const int LastDocumentedPushBatch = 280;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough277PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB202PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-280.md";
    }
}