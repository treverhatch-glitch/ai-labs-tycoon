namespace AiLabs.Unity.Hq
{
    public static class CampusPresentationTailRollupRefreshThrough214PresentationSpec
    {
        public const int RefreshThroughBatch = 214;

        public static int PriorRefreshThroughBatch =>
            CampusPresentationTailRollupRefreshThrough211PresentationSpec.RefreshThroughBatch;

        public static int GoPacketLastBatch =>
            CampusGoPacketTailRefreshThrough295PresentationSpec.LastDocumentedPushBatch;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-214.md";
    }
}