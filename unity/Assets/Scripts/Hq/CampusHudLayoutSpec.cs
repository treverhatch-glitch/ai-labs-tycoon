using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Layout constants at 1920×1080 (see UI-HUD-LAYOUTS.md). Presentation only.</summary>
    public static class CampusHudLayoutSpec
    {
        public const float TopBarHeight = 72f;
        public const float AdvanceWeekWidth = 200f;
        public const float AdvanceWeekHeight = 48f;
        public const float SafeInset = 24f;
        public const float TabRailWidth = 64f;
        public const float MascotStripHeightFraction = 0.18f;
        public const float BuildPanelWidthFraction = 0.28f;
        public const float RivalsListWidthFraction = 0.35f;
        public const float RivalsLogWidthFraction = 0.65f;
        public const float GauntletOverlayFadeSeconds = 0.6f;

        public static Vector2 AdvanceWeekAnchoredPosition =>
            new Vector2(-SafeInset, SafeInset);
    }
}