namespace AiLabs.Unity.Hq
{
    /// <summary>World map presentation paths (see WORLD-MAP-PANEL.md).</summary>
    public static class CampusWorldMapTokens
    {
        public const string BlocTintsJson = "Assets/Art/Generated/world-bloc-tints.json";

        public static readonly string[] KnownBlocIds =
        {
            "us", "eu", "china", "gulf", "singapore", "thailand",
        };
        public const string PinPropId = "ui-world-map-pin";
        public const string BlocTilePropId = "ui-bloc-tile";
        public const string WorldIconId = "icon-world";
    }
}