namespace AiLabs.Unity.Hq
{
    /// <summary>Frozen bloc overlay chrome (see WORLD-MAP-FREEZE-PRESENTATION.md).</summary>
    public static class CampusWorldMapFreezePresentationSpec
    {
        public const string DangerIconId = "icon-danger";
        public const float FrozenPinAlpha = 0.7f;
        public const float BlocTileDesaturate = 0.45f;

        public static string PinPropId => CampusWorldMapTokens.PinPropId;
        public static string BlocTilePropId => CampusWorldMapTokens.BlocTilePropId;
    }
}