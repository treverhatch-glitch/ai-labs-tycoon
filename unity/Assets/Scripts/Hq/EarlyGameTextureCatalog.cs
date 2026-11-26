using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>PNG swatches for garage materials (sync with textures-manifest.json).</summary>
    public static class EarlyGameTextureCatalog
    {
        const string Root = "Assets/Art/EarlyGame/textures/";

        public static readonly IReadOnlyList<string> TextureIds = new[]
        {
            "tex-garage-carpet", "tex-garage-concrete", "tex-garage-trim-orange",
            "tex-garage-wall-beige", "tex-garage-door-metal", "tex-garage-server-paint",
            "tex-hud-paper-warm", "tex-crt-phosphor",
        };

        public static readonly IReadOnlyDictionary<string, string> PngById =
            TextureIds.ToDictionary(id => id, id => Root + id + ".png");

        public static string PathFor(string textureId) =>
            PngById.TryGetValue(textureId, out string path) ? path : null;
    }
}