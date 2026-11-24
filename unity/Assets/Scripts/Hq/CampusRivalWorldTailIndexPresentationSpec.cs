namespace AiLabs.Unity.Hq
{
    /// <summary>Rival/world cross-index tail (see RIVAL-WORLD-TAIL-INDEX.md).</summary>
    public static class CampusRivalWorldTailIndexPresentationSpec
    {
        public const int TailEntryCount = 4;

        public static int RivalFeelingLayers =>
            CampusRivalFeelingIndexPresentationSpec.DocumentedRivalFeelingLayers;

        public static int WorldMapLayers =>
            CampusWorldMapHostIndexPresentationSpec.DocumentedWorldMapLayers;

        public static int KnownBlocs =>
            CampusWorldMapHostIndexPresentationSpec.KnownBlocCount;

        public static float WorldFreezePinAlpha =>
            CampusSeasonalOverlayIndexPresentationSpec.WorldMapFrozenPinAlpha;

        public const string TailDoc = "RIVAL-WORLD-TAIL-INDEX.md";
    }
}