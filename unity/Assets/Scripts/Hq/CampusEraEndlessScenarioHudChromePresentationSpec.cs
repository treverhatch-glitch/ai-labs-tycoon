namespace AiLabs.Unity.Hq
{
    /// <summary>Endless-scenario HUD chrome (era schema 152); presentation only.</summary>
    public static class CampusEraEndlessScenarioHudChromePresentationSpec
    {
        public const int EraSchemaSlot = 152;

        public static int HudTailRefreshBatch =>
            CampusExplainableHudTailRefreshThrough312PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string ChromeDoc = "ERA-ENDLESS-SCENARIO-HUD-CHROME.md";
    }
}