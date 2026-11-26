using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Logs early-game asset paths until OBJ/SVG importers land in HqGridView.</summary>
    public static class EarlyGameArtPaths
    {
        public static void LogResolve(string path)
        {
            if (string.IsNullOrEmpty(path))
                Debug.LogWarning("EarlyGameArt: missing path");
            else
                Debug.Log("EarlyGameArt: " + path);
        }
    }
}