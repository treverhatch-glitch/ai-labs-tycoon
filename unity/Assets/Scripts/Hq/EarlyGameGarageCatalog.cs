using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>Garage HQ OBJ paths (sync with garage-manifest.json).</summary>
    public static class EarlyGameGarageCatalog
    {
        const string Root = "Assets/Art/EarlyGame/garage-kit/";

        public static readonly IReadOnlyList<string> AllAssetIds = new[]
        {
            "garage-floor-tile", "garage-wall-panel", "garage-exterior-wall", "garage-roll-door",
            "garage-server-closet", "garage-land-for-sale-sign", "garage-lot-curb", "garage-window-frame",
            "garage-desk-crt", "garage-crt-tower", "garage-voodude-rack", "garage-whiteboard-1995",
            "garage-coffee-drip", "garage-folding-chair", "garage-cable-spool",
        };

        public static readonly IReadOnlyDictionary<string, string> ObjById =
            AllAssetIds.ToDictionary(id => id, id => Root + id + ".obj");

        public static string PathFor(string assetId) =>
            ObjById.TryGetValue(assetId, out string path) ? path : null;
    }
}