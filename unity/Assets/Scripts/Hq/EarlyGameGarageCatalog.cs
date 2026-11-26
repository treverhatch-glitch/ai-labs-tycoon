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
            "garage-cable-spool",
            "garage-coffee-drip",
            "garage-crt-tower",
            "garage-desk-crt",
            "garage-exterior-wall",
            "garage-floor-tile",
            "garage-folding-chair",
            "garage-land-for-sale-sign",
            "garage-lot-curb",
            "garage-roll-door",
            "garage-server-closet",
            "garage-voodude-rack",
            "garage-wall-panel",
            "garage-whiteboard-1995",
            "garage-window-frame",
        };

        public static readonly IReadOnlyDictionary<string, string> ObjById =
            AllAssetIds.ToDictionary(id => id, id => Root + id + ".glb");

        public static string PathFor(string assetId) =>
            ObjById.TryGetValue(assetId, out string path) ? path : null;
    }
}