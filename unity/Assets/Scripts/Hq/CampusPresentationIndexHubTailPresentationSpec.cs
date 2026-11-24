namespace AiLabs.Unity.Hq
{
    /// <summary>Presentation hub tail navigation (see PRESENTATION-INDEX-HUB-TAIL.md).</summary>
    public static class CampusPresentationIndexHubTailPresentationSpec
    {
        public const int TailEntryCount = 4;

        public static int HubEntryCount =>
            CampusPresentationIndexHubPresentationSpec.DocumentedHubEntries;

        public static int ExtendedLaneEntryCount =>
            CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneEntryCount;

        public const string HubTailDoc = "PRESENTATION-INDEX-HUB-TAIL.md";
        public const string HubDoc = "PRESENTATION-INDEX-HUB.md";
    }
}