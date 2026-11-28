using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class SimulationHostTests
    {
        [Test]
        public void StepWeek_advances_rules_tick()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            int changes = 0;
            host.StateChanged += () => changes++;
            host.StartNewGame(config, 99L);

            int before = host.Simulation.Tick;
            host.StepWeek();
            Assert.Greater(host.Simulation.Tick, before);
            Assert.GreaterOrEqual(changes, 2);
        }
    }
}