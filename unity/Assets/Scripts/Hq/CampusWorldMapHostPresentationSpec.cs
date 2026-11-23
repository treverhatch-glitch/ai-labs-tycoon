namespace AiLabs.Unity.Hq
{
    /// <summary>World map host shell chrome (see WORLD-MAP-HOST-PRESENTATION.md).</summary>
    public static class CampusWorldMapHostPresentationSpec
    {
        public const string WorldIconId = "icon-world";
        public const float SidePanelWidthFraction = 0.38f;
        public static int KnownBlocCount => CampusWorldMapTokens.KnownBlocIds.Length;
    }
}