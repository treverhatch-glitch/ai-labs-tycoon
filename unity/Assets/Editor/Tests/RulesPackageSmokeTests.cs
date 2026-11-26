using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class RulesPackageSmokeTests
    {
        static string ConfigPath()
        {
            string unityRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.GetFullPath(Path.Combine(unityRoot, "..", "Rules", "Data", "default-config.json"));
        }

        [Test]
        public void Default_config_loads_and_starts_a_garage_game()
        {
            string json = File.ReadAllText(ConfigPath());
            GameConfig config = GameConfig.FromJson(json);
            Simulation sim = Simulation.NewGame(config, 7L);

            Assert.AreEqual("garage", sim.Player.LotId);
            Assert.AreEqual(0, sim.Player.Rooms.Count);
            int tick = sim.Tick;
            sim.Step();
            Assert.Greater(sim.Tick, tick);
        }
    }
}