namespace AiLabs.Unity.Hq
{
    /// <summary>Rival-intent news ticker chrome (era schema 150); presentation only.</summary>
    public static class CampusEraRivalIntentNewsTickerChromePresentationSpec
    {
        public const int EraSchemaSlot = 150;

        public static int RivalWorldTailBatch =>
            CampusRivalWorldTailIndexRefreshThrough300PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string ChromeDoc = "ERA-RIVAL-INTENT-NEWS-TICKER-CHROME.md";
    }
}