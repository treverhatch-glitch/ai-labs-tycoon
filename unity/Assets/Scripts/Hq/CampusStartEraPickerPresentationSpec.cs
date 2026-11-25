namespace AiLabs.Unity.Hq
{
    /// <summary>Start-era picker chrome (see START-ERA-PICKER-PRESENTATION.md).</summary>
    public static class CampusStartEraPickerPresentationSpec
    {
        public const int TaglineMaxLines = 2;

        public const float EraCardMinWidthPixels = 160f;

        public static int DocumentedEraCount => CampusEraSkinTokens.DocumentedEraIds.Length;

        public const float SwatchSizePixels = CampusEraSkinPreviewPresentationSpec.SwatchSizePixels;
    }
}