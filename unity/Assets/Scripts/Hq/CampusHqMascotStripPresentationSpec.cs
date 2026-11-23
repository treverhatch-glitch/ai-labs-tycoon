namespace AiLabs.Unity.Hq
{
    /// <summary>HQ mascot strip band chrome (see HQ-MASCOT-STRIP-PRESENTATION.md).</summary>
    public static class CampusHqMascotStripPresentationSpec
    {
        public static float StripHeightFraction => CampusHudLayoutSpec.MascotStripHeightFraction;
        public static float RowHeightPixels => CampusMascotRowHudPresentationSpec.RowHeightPixels;
        public static int MaxVisibleMascots => CampusMascotRowHudPresentationSpec.MaxVisibleMascots;
    }
}