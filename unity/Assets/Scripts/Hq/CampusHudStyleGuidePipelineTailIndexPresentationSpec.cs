namespace AiLabs.Unity.Hq
{
    /// <summary>Tail index for STYLE-GUIDE + NEXT.md art pipeline HUD rows (batch 218 LOCAL).</summary>
    public static class CampusHudStyleGuidePipelineTailIndexPresentationSpec
    {
        public const int DocumentedHudPipelineRows = 4;

        public static int ExplainableHudDrawers =>
            CampusExplainableHudIndexPresentationSpec.DocumentedBreakdownDrawers;

        public static float SharedPartRowHeightPixels =>
            CampusExplainableHudIndexPresentationSpec.SharedPartRowHeightPixels;

        public const string StyleGuideDoc = "STYLE-GUIDE.md";
        public const string PipelineTailDoc = "HUD-STYLE-GUIDE-PIPELINE-TAIL.md";
    }
}