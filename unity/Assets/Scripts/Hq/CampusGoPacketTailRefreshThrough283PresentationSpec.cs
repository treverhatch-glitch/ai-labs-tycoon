namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough283PresentationSpec
    {
        public const int LastDocumentedPushBatch = 283;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough280PresentationSpec.LastDocumentedPushBatch;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB205PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-283.md";
    }
}