namespace AiLabs.Unity.Hq
{
    /// <summary>HUD SVG paths (see ui-icons-manifest.json).</summary>
    public static class CampusIconCatalog
    {
        const string Root = "Assets/Art/Generated/ui-icons/";

        public static readonly string[] AllIconIds =
        {
            "icon-cash", "icon-calendar", "icon-compute", "icon-research", "icon-rivals",
            "icon-world", "icon-training", "icon-hq",
        };

        public static string PathFor(string iconId) => Root + iconId + ".svg";
    }
}