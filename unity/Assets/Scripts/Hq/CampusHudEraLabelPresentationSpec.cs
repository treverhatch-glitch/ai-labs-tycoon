namespace AiLabs.Unity.Hq
{
    /// <summary>HUD era name label chrome (see HUD-ERA-LABEL-PRESENTATION.md).</summary>
    public static class CampusHudEraLabelPresentationSpec
    {
        public const float LabelWidthPixels = 420f;
        public const float LabelHeightPixels = CampusHudDateWeekPresentationSpec.DateRowHeightPixels;
        public const int MinFontSizePixels = CampusHqCashDisplayPresentationSpec.MinFontSizePixels;

        public const float EraTransitionSeconds = CampusEraSkinTokens.EraTransitionSeconds;
    }
}