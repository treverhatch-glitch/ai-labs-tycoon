namespace AiLabs.Unity.Hq
{
    public static class CampusExplainableHudTailRefreshThrough300PresentationSpec
    {
        public const int TailRefreshThroughPresentationBatch = 300;

        public static int TailEntryCount =>
            CampusExplainableHudTailIndexPresentationSpec.TailEntryCount;

        public static int RollupRefreshLastBatch =>
            CampusPresentationTailRollupRefreshThrough199PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "EXPLAINABLE-HUD-TAIL-REFRESH-300.md";
    }
}