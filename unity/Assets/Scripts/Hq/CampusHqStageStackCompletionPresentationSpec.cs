namespace AiLabs.Unity.Hq
{
    /// <summary>HQ stack index completion (see HQ-STAGE-STACK-COMPLETION.md).</summary>
    public static class CampusHqStageStackCompletionPresentationSpec
    {
        public const int IndexedHqBands = 5;

        public static int TotalHqBands => CampusHqStageStackRollupPresentationSpec.HqBandCount;

        public static int RoomBuildIndexLayers => CampusHqStageStackRollupPresentationSpec.RoomBuildIndexLayers;

        public const string RollupDoc = "HQ-STAGE-STACK-ROLLUP.md";
        public const string CompletionDoc = "HQ-STAGE-STACK-COMPLETION.md";
    }
}