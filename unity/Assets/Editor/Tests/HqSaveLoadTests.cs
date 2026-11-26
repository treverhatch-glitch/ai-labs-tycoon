using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqSaveLoadTests
    {
        [Test]
        public void Save_and_load_round_trips_tick_and_rooms()
        {
            string configPath = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(configPath));

            var host = new SimulationHost();
            host.StartNewGame(config, 77L);
            HqStarterLayout.ApplyIfEmpty(host);
            int tick = host.Simulation.Tick;
            int rooms = host.Simulation.Player.Rooms.Count;
            host.StepWeeks(3);

            string savePath = Path.Combine(Path.GetTempPath(), "hq-test-" + System.Guid.NewGuid().ToString("N") + ".json");
            try
            {
                Assert.IsTrue(HqSaveLoad.SaveToFile(host, savePath));
                Assert.IsTrue(File.Exists(savePath));

                var host2 = new SimulationHost();
                host2.StartNewGame(config, 1L);
                Assert.IsTrue(HqSaveLoad.LoadFromFile(host2, savePath));
                Assert.AreEqual(tick + 3, host2.Simulation.Tick);
                Assert.AreEqual(rooms, host2.Simulation.Player.Rooms.Count);
            }
            finally
            {
                if (File.Exists(savePath)) File.Delete(savePath);
            }
        }

        [Test]
        public void SaveJson_matches_Simulation_save()
        {
            string configPath = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(configPath));

            var host = new SimulationHost();
            host.StartNewGame(config, 5L);
            string viaHost = host.SaveJson();
            string viaSim = host.Simulation.Save();
            Assert.AreEqual(viaSim, viaHost);
        }
    }
}