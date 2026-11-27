namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76RollupRefreshB211PresentationSpec
    {
        public const int LastDocumentedBatch = 211;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshB208PresentationSpec.LastDocumentedBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough289PresentationSpec.LastDocumentedPushBatch;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-211.md";
    }
}