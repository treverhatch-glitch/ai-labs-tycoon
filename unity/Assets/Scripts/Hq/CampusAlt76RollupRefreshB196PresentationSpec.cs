namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB196PresentationSpec
    {
        public const int LastDocumentedBatch = 196;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB193PresentationSpec.LastDocumentedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough265PresentationSpec.LastDocumentedPushBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-196.md";
    }
}