using System.IO;
using AiLabs.Unity.Sim;

namespace AiLabs.Unity.Hq
{
    /// <summary>Thin file wrapper around <see cref="SimulationHost"/> save JSON.</summary>
    public static class HqSaveLoad
    {
        public const string DefaultFileName = "hq-save.json";

        public static string DefaultPath =>
            Path.Combine(UnityEngine.Application.persistentDataPath, DefaultFileName);

        public static bool SaveToFile(SimulationHost host, string path)
        {
            string json = host?.SaveJson();
            if (string.IsNullOrEmpty(json)) return false;
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, json);
            return true;
        }

        public static bool LoadFromFile(SimulationHost host, string path)
        {
            if (host == null || !File.Exists(path)) return false;
            return host.LoadJson(File.ReadAllText(path));
        }
    }
}