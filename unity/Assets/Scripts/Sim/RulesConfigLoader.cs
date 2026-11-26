using System;
using System.IO;
using UnityEngine;

namespace AiLabs.Unity.Sim
{
    /// <summary>Loads Rules/Data/default-config.json without copying tunables into Unity assets.</summary>
    public static class RulesConfigLoader
    {
        const string PackageConfigPath = "Packages/com.hatchtag.ailabs.rules/Data/default-config.json";

        public static string LoadJsonText()
        {
#if UNITY_EDITOR
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(PackageConfigPath);
            if (asset != null) return asset.text;
#endif
            string repoPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "Rules", "Data", "default-config.json"));
            if (File.Exists(repoPath)) return File.ReadAllText(repoPath);

            throw new InvalidOperationException(
                "Could not load default-config.json from the Rules package or repo path " + repoPath + ".");
        }
    }
}