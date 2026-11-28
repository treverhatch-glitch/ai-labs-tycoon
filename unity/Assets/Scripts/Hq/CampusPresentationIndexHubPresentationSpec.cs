namespace AiLabs.Unity.Hq
{
    /// <summary>Presentation index hub (see PRESENTATION-INDEX-HUB.md).</summary>
    public static class CampusPresentationIndexHubPresentationSpec
    {
        public const int FirstHubBatch = 50;
        public const int LastHubBatch = 60;

        public const int DocumentedHubEntries = LastHubBatch - FirstHubBatch + 1;

        public const string HubDoc = "PRESENTATION-INDEX-HUB.md";
    }
}