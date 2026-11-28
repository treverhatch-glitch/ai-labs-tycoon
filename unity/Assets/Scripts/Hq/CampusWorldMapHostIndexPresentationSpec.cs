namespace AiLabs.Unity.Hq
{
    /// <summary>World map host presentation index (see WORLD-MAP-HOST-INDEX.md).</summary>
    public static class CampusWorldMapHostIndexPresentationSpec
    {
        public const int DocumentedWorldMapLayers = 2;

        public static int KnownBlocCount => CampusWorldMapHostPresentationSpec.KnownBlocCount;

        public const string HostPresentationDoc = "WORLD-MAP-HOST-PRESENTATION.md";
        public const string PanelDoc = "WORLD-MAP-PANEL.md";
    }
}