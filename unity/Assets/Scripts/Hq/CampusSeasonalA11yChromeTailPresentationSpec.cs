namespace AiLabs.Unity.Hq
{
    /// <summary>Seasonal overlay + a11y chrome tail (NEXT art table batches 53/59).</summary>
    public static class CampusSeasonalA11yChromeTailPresentationSpec
    {
        public static int SeasonalOverlays =>
            CampusSeasonalOverlayIndexPresentationSpec.DocumentedSeasonalOverlays;

        public static int PostRollupBatch =>
            CampusAlt76RollupRefreshB187PresentationSpec.LastDocumentedBatch;

        public const string A11yIndexDoc = "A11Y-CHROME-INDEX.md";
        public const string TailDoc = "SEASONAL-A11Y-CHROME-TAIL.md";
    }
}