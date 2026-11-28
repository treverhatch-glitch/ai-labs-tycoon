namespace AiLabs.Unity.Hq
{
    /// <summary>Tab rail presentation index (see TAB-RAIL-INTEGRATION-INDEX.md).</summary>
    public static class CampusTabRailIntegrationIndexPresentationSpec
    {
        public const int DocumentedTabRailLayers = 3;

        public static int TabCount => CampusTabRailMock.TabIds.Length;

        public static float RailWidthPixels => CampusTabRailFuturePresentationSpec.RailWidthPixels;

        public const string IntegrationDoc = "TAB-RAIL-INTEGRATION.md";
        public const string ActiveStateDoc = "TAB-RAIL-ACTIVE-PRESENTATION.md";
        public const string FutureRailDoc = "TAB-RAIL-FUTURE-PRESENTATION.md";
    }
}