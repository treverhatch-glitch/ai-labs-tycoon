using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqGlowLiveTests
    {
        [Test]
        public void RefreshGlowFromTraining_updates_without_full_rebuild_count()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 18L);
            HqLivedInCampus.Apply(host, trainWeeks: 0);

            var hq = new GameObject("HQ");
            var grid = hq.AddComponent<HqGridView>();
            grid.Bind(host.Simulation);

            bool training = grid.AnyModelTraining();
            Assert.IsTrue(training);

            grid.RefreshGlowFromTraining();
            int blocks = 0;
            foreach (var _ in Object.FindObjectsByType<RoomBlockView>(FindObjectsSortMode.None))
                blocks++;
            Assert.AreEqual(host.Simulation.Player.Rooms.Count, blocks);

            Object.DestroyImmediate(hq);
        }

        [Test]
        public void Lived_in_scenario_steps_tick_and_keeps_rooms()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 2L);
            int tick0 = host.Simulation.Tick;
            HqLivedInCampus.Apply(host);
            Assert.Greater(host.Simulation.Tick, tick0);
            Assert.GreaterOrEqual(host.Simulation.Player.Rooms.Count, 4);
            Assert.Greater(host.Simulation.Player.Models.Count, 0);
        }
    }
}