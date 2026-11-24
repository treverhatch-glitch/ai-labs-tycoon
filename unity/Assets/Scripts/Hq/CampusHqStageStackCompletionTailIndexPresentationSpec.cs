namespace AiLabs.Unity.Hq
{
    /// <summary>HQ stack completion tail (see HQ-STAGE-STACK-COMPLETION-TAIL-INDEX.md).</summary>
    public static class CampusHqStageStackCompletionTailIndexPresentationSpec
    {
        public const int TailAnchorCount = 3;

        public static int IndexedHqBands =>
            CampusHqStageStackCompletionPresentationSpec.IndexedHqBands;

        public static int TotalHqBands =>
            CampusHqStageStackCompletionPresentationSpec.TotalHqBands;

        public static int ComputeSplitTailAnchors =>
            CampusComputeSplitTailIndexPresentationSpec.TailAnchorCount;

        public const string TailIndexDoc = "HQ-STAGE-STACK-COMPLETION-TAIL-INDEX.md";
    }
}