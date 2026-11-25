namespace AiLabs.Unity.Hq
{
    /// <summary>Mascot strip chrome (see MASCOT-ROW-HUD.md).</summary>
    public static class CampusMascotRowHudPresentationSpec
    {
        public const float RowHeightPixels = 48f;
        public const int MaxVisibleMascots = 4;
        public static float StripHeightFraction => CampusHudLayoutSpec.MascotStripHeightFraction;
    }
}