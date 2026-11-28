namespace AiLabs.Unity.Hq
{
    /// <summary>Compute split presentation index (see COMPUTE-SPLIT-INDEX.md).</summary>
    public static class CampusComputeSplitIndexPresentationSpec
    {
        public const int DocumentedComputeSplitLayers = 2;

        public static float SplitBarHeightPixels => CampusComputeSplitPresentationSpec.SplitBarHeightPixels;

        public static int SliderCount => CampusComputeSplitPresentationSpec.SliderCount;

        public const string SplitDoc = "COMPUTE-SPLIT-PRESENTATION.md";
        public const string PeekDoc = "TRAINING-PEEK-PRESENTATION.md";
    }
}