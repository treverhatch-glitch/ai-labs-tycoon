namespace AiLabs.Unity.Hq
{
    /// <summary>Presentation tail rollup refresh (see PRESENTATION-TAIL-ROLLUP-REFRESH-98.md).</summary>
    public static class CampusPresentationTailRollupRefreshPresentationSpec
    {
        public const int DocumentedTailLanes = 11;

        public static int PriorTailLaneCount =>
            CampusPresentationTailRollupPresentationSpec.DocumentedTailLanes;

        public static int BandTailAnchorCount =>
            CampusStaffPresentationTailIndexPresentationSpec.TailAnchorCount;

        public static int LastGroupedTailBatch => 98;

        public static int RollupRefreshLastBatch => 103;

        public const string RefreshDoc = "PRESENTATION-TAIL-ROLLUP-REFRESH-98.md";
    }
}