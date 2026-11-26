using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqGridRoomsTests
    {
        [Test]
        public void Grid_rebuild_matches_rules_room_count()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            var host = new SimulationHost();
            host.StartNewGame(config, 11L);
            HqStarterLayout.ApplyIfEmpty(host);

            var hq = new GameObject("HQ");
            var grid = hq.AddComponent<HqGridView>();
            grid.Bind(host.Simulation);

            int blocks = 0;
            foreach (var _ in Object.FindObjectsByType<RoomBlockView>(FindObjectsSortMode.None))
                blocks++;

            Assert.AreEqual(host.Simulation.Player.Rooms.Count, blocks);

            Object.DestroyImmediate(hq);
        }
    }
}