namespace AiLabs.Unity.Hq
{
    /// <summary>Presentation tail rollup (see PRESENTATION-TAIL-ROLLUP.md).</summary>
    public static class CampusPresentationTailRollupPresentationSpec
    {
        public const int DocumentedTailLanes = 7;

        public static int HubTailRows =>
            CampusPresentationIndexHubTailPresentationSpec.TailEntryCount;

        public static int ExtendedManifestEntries =>
            CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneEntryCount;

        public static int CaptureTailEntries =>
            CampusCaptureHudTailIndexPresentationSpec.TailEntryCount;

        public static int LastLocalRollupBatch => 92;

        public const string RollupDoc = "PRESENTATION-TAIL-ROLLUP.md";
    }
}