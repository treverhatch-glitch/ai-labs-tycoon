namespace AiLabs.Unity.Hq
{
    /// <summary>Mascot strip tail cross-links (see HQ-MASCOT-STRIP-TAIL-INDEX.md).</summary>
    public static class CampusHqMascotStripTailIndexPresentationSpec
    {
        public const int TailAnchorCount = 3;

        public static int MascotHubLayers =>
            CampusHqMascotStripIndexPresentationSpec.DocumentedMascotStripLayers;

        public static int TrainingTailAnchors =>
            CampusTrainingPeekTailIndexPresentationSpec.TailAnchorCount;

        public static int MaxVisibleMascots =>
            CampusHqMascotStripIndexPresentationSpec.MaxVisibleMascots;

        public const string TailIndexDoc = "HQ-MASCOT-STRIP-TAIL-INDEX.md";
    }
}