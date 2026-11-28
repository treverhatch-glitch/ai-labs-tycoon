namespace AiLabs.Unity.Hq
{
    /// <summary>Training tail cross-links (see TRAINING-PEEK-TAIL-INDEX.md).</summary>
    public static class CampusTrainingPeekTailIndexPresentationSpec
    {
        public const int TailAnchorCount = 3;

        public static int TrainingHubLayers =>
            CampusTrainingPeekIndexPresentationSpec.DocumentedTrainingPeekLayers;

        public static int StaffTailAnchors =>
            CampusStaffPresentationTailIndexPresentationSpec.TailAnchorCount;

        public static int PostTailRollupLanes =>
            CampusPresentationTailRollupPresentationSpec.DocumentedTailLanes;

        public const string TailIndexDoc = "TRAINING-PEEK-TAIL-INDEX.md";
    }
}