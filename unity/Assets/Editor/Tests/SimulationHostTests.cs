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

        [Test]
        public void StepWeeks_advances_multiple_ticks()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 4L);
            int before = host.Simulation.Tick;
            int month = config.Calendar.WeeksPerMonth;
            host.StepWeeks(month);
            Assert.AreEqual(before + month, host.Simulation.Tick);
        }

        [Test]
        public void LoadJson_restores_save_from_SaveJson()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 12L);
            host.StepWeeks(5);
            string save = host.SaveJson();

            var host2 = new SimulationHost();
            host2.StartNewGame(config, 999L);
            Assert.IsTrue(host2.LoadJson(save));
            Assert.AreEqual(host.Simulation.Tick, host2.Simulation.Tick);
        }
    }
}