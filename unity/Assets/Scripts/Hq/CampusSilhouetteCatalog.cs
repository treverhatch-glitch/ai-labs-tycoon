namespace AiLabs.Unity.Hq
{
    /// <summary>Rival silhouette SVG paths (see rival-silhouettes-manifest.json).</summary>
    public static class CampusSilhouetteCatalog
    {
        const string Root = "Assets/Art/Generated/rival-silhouettes/";

        public static string PathFor(string labId) => Root + "silhouette-" + labId + ".svg";

        public static readonly string[] Supported =
        {
            "openaye", "anthoptics", "xay", "envydia",
        };
    }
}