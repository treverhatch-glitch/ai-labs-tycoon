using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqStarterLayoutTests
    {
        static GameConfig LoadConfig()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(UnityEngine.Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            return GameConfig.FromJson(File.ReadAllText(path));
        }

        [Test]
        public void ApplyIfEmpty_builds_four_rooms_from_rules()
        {
            var host = new SimulationHost();
            host.StartNewGame(LoadConfig(), 7L);
            Assert.AreEqual(0, host.Simulation.Player.Rooms.Count);

            HqStarterLayout.ApplyIfEmpty(host);
            Assert.GreaterOrEqual(host.Simulation.Player.Rooms.Count, 4);
        }

        [Test]
        public void ApplyIfEmpty_skips_when_rooms_exist()
        {
            var host = new SimulationHost();
            host.StartNewGame(LoadConfig(), 9L);
            HqStarterLayout.ApplyIfEmpty(host);
            int count = host.Simulation.Player.Rooms.Count;
            HqStarterLayout.ApplyIfEmpty(host);
            Assert.AreEqual(count, host.Simulation.Player.Rooms.Count);
        }
    }
}