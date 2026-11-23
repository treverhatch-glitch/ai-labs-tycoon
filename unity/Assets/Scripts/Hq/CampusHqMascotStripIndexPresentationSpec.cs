namespace AiLabs.Unity.Hq
{
    /// <summary>HQ mascot strip index (see HQ-MASCOT-STRIP-INDEX.md).</summary>
    public static class CampusHqMascotStripIndexPresentationSpec
    {
        public const int DocumentedMascotStripLayers = 2;

        public static int MaxVisibleMascots => CampusHqMascotStripPresentationSpec.MaxVisibleMascots;

        public static float StripHeightFraction => CampusHqMascotStripPresentationSpec.StripHeightFraction;

        public const string StripDoc = "HQ-MASCOT-STRIP-PRESENTATION.md";
        public const string RowHudDoc = "MASCOT-ROW-HUD.md";
    }
}