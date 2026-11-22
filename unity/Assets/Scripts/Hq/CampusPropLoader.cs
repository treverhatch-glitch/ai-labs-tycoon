using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>
    /// Presentation hook for imported OBJ (Unity thread loads via AssetDatabase).
    /// Logs the catalog path until meshes are wired in HqGridView.
    /// </summary>
    public static class CampusPropLoader
    {
        public static void LogResolve(string propId)
        {
            string path = CampusPropCatalog.PathFor(propId);
            if (path == null)
                Debug.LogWarning("CampusPropLoader: unknown prop id " + propId);
            else
                Debug.Log("CampusPropLoader: " + propId + " → " + path);
        }
    }
}