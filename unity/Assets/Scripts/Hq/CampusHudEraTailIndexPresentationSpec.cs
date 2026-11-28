namespace AiLabs.Unity.Hq
{
    /// <summary>HUD era tail navigation (see HUD-ERA-TAIL-INDEX.md).</summary>
    public static class CampusHudEraTailIndexPresentationSpec
    {
        public const int EraTailBatchStart = 29;
        public const int EraTailBatchEnd = 53;
        public const int EraTailEntryCount = 4;

        public static float EraLabelWidthPixels =>
            CampusHudEraLabelPresentationSpec.LabelWidthPixels;

        public static int MinEraSkinCount =>
            CampusEraSkinPreviewPresentationSpec.MinEraSkinCount;

        public static int SeasonalOverlaySlots =>
            CampusSeasonalOverlayIndexPresentationSpec.DocumentedSeasonalOverlays;

        public const string TailDoc = "HUD-ERA-TAIL-INDEX.md";
        public const string TopBarIndexDoc = "TOP-BAR-TOKEN-INDEX.md";
    }
}