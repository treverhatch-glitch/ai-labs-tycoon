namespace AiLabs.Unity.Hq
{
    /// <summary>Compute split tail cross-links (see COMPUTE-SPLIT-TAIL-INDEX.md).</summary>
    public static class CampusComputeSplitTailIndexPresentationSpec
    {
        public const int TailAnchorCount = 3;

        public static int ComputeHubLayers =>
            CampusComputeSplitIndexPresentationSpec.DocumentedComputeSplitLayers;

        public static int MascotTailAnchors =>
            CampusHqMascotStripTailIndexPresentationSpec.TailAnchorCount;

        public static int SliderCount =>
            CampusComputeSplitIndexPresentationSpec.SliderCount;

        public const string TailIndexDoc = "COMPUTE-SPLIT-TAIL-INDEX.md";
    }
}