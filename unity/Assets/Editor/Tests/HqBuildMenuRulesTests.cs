using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    /// <summary>Build menu flow is rules-only: BuildRoom commands through <see cref="SimulationHost"/>.</summary>
    public sealed class HqBuildMenuRulesTests
    {
        [Test]
        public void BuildRoom_from_menu_selection_adds_rules_room()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 31L);
            HqStarterLayout.ApplyIfEmpty(host);
            int before = host.Simulation.Player.Rooms.Count;

            LotConfig lot = config.Lot(host.Simulation.Player.LotId);
            Assert.IsTrue(LotGrid.FindFreeRect(lot, host.Simulation.Player.Rooms, 4, 3, out int x, out int y));
            string typeId = "policy-office";
            Assert.IsTrue(host.TryApply(Command.BuildRoom(host.Simulation.Player.Id, typeId, x, y, 4, 3)));
            Assert.AreEqual(before + 1, host.Simulation.Player.Rooms.Count);

            var hq = new GameObject("HQ");
            var grid = hq.AddComponent<HqGridView>();
            grid.Bind(host.Simulation);
            grid.Refresh();
            Assert.IsTrue(grid.AnyModelTraining() == false);

            Object.DestroyImmediate(hq);
        }
    }
}