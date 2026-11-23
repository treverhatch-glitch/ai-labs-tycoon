namespace AiLabs.Unity.Hq
{
    /// <summary>Training peek presentation index (see TRAINING-PEEK-INDEX.md).</summary>
    public static class CampusTrainingPeekIndexPresentationSpec
    {
        public const int DocumentedTrainingPeekLayers = 2;

        public static float PeekWidthFraction => CampusTrainingPeekPresentationSpec.PanelWidthFraction;

        public static float PeekHeightFraction => CampusTrainingPeekPresentationSpec.PanelHeightFraction;

        public static float SplitBarHeightPixels => CampusTrainingPeekPresentationSpec.SplitBarHeightPixels;

        public const string PeekDoc = "TRAINING-PEEK-PRESENTATION.md";
        public const string ScreenDoc = "TRAINING-SCREEN-PRESENTATION.md";
    }
}