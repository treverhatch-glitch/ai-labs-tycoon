using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class EraRoomEvolutionTests
    {
        [Test]
        public void Later_era_increases_evolution_step_for_dial_up_rooms()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            int atStart = EraRoomEvolution.Step(config, "dial-up-days", "dial-up-days");
            int later = EraRoomEvolution.Step(config, "dial-up-days", "the-chat-boom");
            Assert.AreEqual(0, atStart);
            Assert.Greater(later, atStart);
        }
    }
}