using System.Collections.Generic;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Accents for parody rival labs — curated + hash fallback (no logos).</summary>
    public static class RivalPresentationPalette
    {
        static readonly Dictionary<string, Color> Curated = new Dictionary<string, Color>
        {
            { "openaye", CampusUiTokens.Hex("#5A8CF2") },
            { "anthoptics", CampusUiTokens.Hex("#F2808C") },
            { "xay", CampusUiTokens.Hex("#E85D4A") },
            { "envydia", CampusUiTokens.Hex("#76B900") },
        };

        public static Color AccentFor(string labId)
        {
            if (string.IsNullOrEmpty(labId)) return CampusUiTokens.UiSlate;
            if (Curated.TryGetValue(labId, out Color c)) return c;
            int hash = labId.GetHashCode();
            float h = Mathf.Abs(hash % 360) / 360f;
            return Color.HSVToRGB(h, 0.55f, 0.9f);
        }
    }
}