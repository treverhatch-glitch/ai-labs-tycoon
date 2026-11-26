using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>Dial-up HUD SVG paths (sync with hud-manifest.json).</summary>
    public static class EarlyGameHudCatalog
    {
        const string Root = "Assets/Art/EarlyGame/hud-icons/";

        public static readonly IReadOnlyList<string> HudIconIds = new[]
        {
            "icon-cash-era0", "icon-date-era0", "icon-era-bar", "icon-build-panel",
            "icon-hire-panel", "icon-train-panel", "icon-gpu-hire", "icon-week-advance",
            "panel-build-bg", "panel-hire-bg", "panel-train-bg", "hud-topbar-beige",
        };

        public static readonly IReadOnlyDictionary<string, string> SvgById =
            HudIconIds.ToDictionary(id => id, id => Root + id + ".svg");

        public static string PathFor(string iconId) =>
            SvgById.TryGetValue(iconId, out string path) ? path : null;
    }
}