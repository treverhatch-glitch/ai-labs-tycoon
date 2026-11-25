using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    /// <summary>Presentation-facing summaries that hire/GPU panels display.</summary>
    public sealed class StaffGpuFormatTests
    {
        [Test]
        public void New_game_shows_no_staff_and_no_gpus()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));
            Simulation sim = Simulation.NewGame(config, 11L);
            LabState lab = sim.Player;

            Assert.AreEqual(0, LabQueries.TotalStaff(lab));
            Assert.AreEqual(0, lab.RentedGpus);
            Assert.AreEqual(0, LabQueries.HqGpus(lab));
        }
    }
}