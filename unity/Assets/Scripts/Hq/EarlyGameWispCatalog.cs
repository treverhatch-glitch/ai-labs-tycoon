using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>AI wisp mood art paths (sync with wisp-manifest.json).</summary>
    public static class EarlyGameWispCatalog
    {
        const string Root = "Assets/Art/EarlyGame/wisp-moods/";

        public static readonly IReadOnlyList<string> WispMoodIds = new[]
        {
            "wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing",
        };

        public static readonly IReadOnlyDictionary<string, string> SvgById =
            WispMoodIds.ToDictionary(id => id, id => Root + id + ".svg");

        public static string PathFor(string moodId) =>
            SvgById.TryGetValue(moodId, out string path) ? path : null;
    }
}