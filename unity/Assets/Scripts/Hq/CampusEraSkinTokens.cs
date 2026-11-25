namespace AiLabs.Unity.Hq
{
    /// <summary>Era presentation ids for skin cross-fade (see era-skin-tokens.json).</summary>
    public static class CampusEraSkinTokens
    {
        public const float EraTransitionSeconds = 0.8f;

        public static readonly string[] DocumentedEraIds =
        {
            "1995", "2007", "2017", "2035",
        };

        public const string TokensJson = "Assets/Art/Generated/era-skin-tokens.json";
    }
}