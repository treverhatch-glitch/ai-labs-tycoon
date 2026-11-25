namespace AiLabs.Unity.Hq
{
    /// <summary>HQ stage stack rollup (see HQ-STAGE-STACK-ROLLUP.md).</summary>
    public static class CampusHqStageStackRollupPresentationSpec
    {
        public static int HqBandCount => CampusHqStageStackPresentationSpec.HqPresentationBandCount;

        public static int RoomBuildIndexLayers => CampusRoomBuildIndexPresentationSpec.DocumentedRoomBuildLayers;

        public static float LotViewportWidthFraction =>
            CampusHqStageStackPresentationSpec.LotViewportWidthFraction;

        public const string StackDoc = "HQ-STAGE-STACK.md";
        public const string RollupDoc = "HQ-STAGE-STACK-ROLLUP.md";
    }
}