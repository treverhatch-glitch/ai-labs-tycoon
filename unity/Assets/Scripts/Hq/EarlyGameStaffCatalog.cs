using System.Collections.Generic;
using System.Linq;

namespace AiLabs.Unity.Hq
{
    /// <summary>Early staff silhouettes (sync with staff-manifest.json).</summary>
    public static class EarlyGameStaffCatalog
    {
        const string Root = "Assets/Art/EarlyGame/staff-kit/";

        public static readonly IReadOnlyList<string> AllAssetIds = new[]
        {
            "staff-researcher-idle", "staff-engineer-work", "staff-safety-idle", "staff-sales-idle",
            "staff-recruiter-idle", "staff-gpu-tech-idle", "staff-founder-idle", "staff-pose-typing",
        };

        public static readonly IReadOnlyDictionary<string, string> ObjById =
            AllAssetIds.ToDictionary(id => id, id => Root + id + ".obj");

        public static string PathFor(string roleId) =>
            ObjById.TryGetValue(roleId, out string path) ? path : null;
    }
}