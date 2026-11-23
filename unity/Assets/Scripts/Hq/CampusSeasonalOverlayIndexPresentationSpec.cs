namespace AiLabs.Unity.Hq
{
    /// <summary>Seasonal overlay index (see SEASONAL-OVERLAY-INDEX.md).</summary>
    public static class CampusSeasonalOverlayIndexPresentationSpec
    {
        public const int DocumentedSeasonalOverlays = 2;

        public const float GauntletFadeSeconds = CampusHudLayoutSpec.GauntletOverlayFadeSeconds;

        public const float WorldMapFrozenPinAlpha = CampusWorldMapFreezePresentationSpec.FrozenPinAlpha;

        public const string GauntletStageDoc = "GAUNTLET-STAGE-HUD.md";
        public const string WorldMapFreezeDoc = "WORLD-MAP-FREEZE-PRESENTATION.md";
    }
}