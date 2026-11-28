using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>HUD and material colour tokens (matches docs/art/MATERIAL-TOKENS.json).</summary>
    public static class CampusUiTokens
    {
        public static readonly Color UiInk = Hex("#1A1D24");
        public static readonly Color UiPaper = Hex("#F4F0E8");
        public static readonly Color UiSlate = new Color(0.078f, 0.09f, 0.114f, 0.92f);
        public static readonly Color UiDanger = Hex("#E85D4A");
        public static readonly float DefaultSmoothness = 0.2f;
        public static readonly float FloorSmoothness = 0.15f;
        public static readonly float EmissionHdr = 1.8f;

        public static Color Hex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color c)) return c;
            return Color.magenta;
        }
    }
}