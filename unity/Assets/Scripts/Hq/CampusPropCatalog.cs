using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>OBJ paths for generated props (keep in sync with manifest.json).</summary>
    public static class CampusPropCatalog
    {
        const string Root = "Assets/Art/Generated/campus-kit/";

        public static readonly IReadOnlyList<string> AllAssetIds = new[]
        {
            "prop-desk", "prop-chair", "prop-server-rack", "prop-potted-plant", "prop-whiteboard",
            "building-module-1x1", "exterior-quad-tree", "exterior-sign-pole", "ui-world-map-pin",
            "ui-rival-badge", "prop-gpu-blade", "prop-coffee-cart", "prop-research-bench",
            "prop-data-stack", "prop-gauntlet-lectern", "ui-bloc-tile", "prop-safety-cone",
            "prop-monitor-wall", "exterior-bike-rack", "ui-tab-rail-plaque", "prop-lounge-sofa",
            "exterior-fence-segment", "exterior-solar-shade", "prop-water-cooler", "building-elevator-core",
            "prop-cafeteria-table", "prop-vending-machine", "exterior-hallway-bench",
        };

        public static readonly IReadOnlyDictionary<string, string> ObjById =
            AllAssetIds.ToDictionary(id => id, id => Root + id + ".obj");

        public static string PathFor(string propId) =>
            ObjById.TryGetValue(propId, out string path) ? path : null;
    }
}