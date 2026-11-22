using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqCampusPhaseTests
    {
        [Test]
        public void Early_eras_are_garage_later_are_campus()
        {
            string path = Path.GetFullPath(Path.Combine(
                Directory.GetParent(UnityEngine.Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(path));

            long start = config.PlayerStart.Cash;
            var poor = new LabState { Cash = start / 4 };
            var rich = new LabState { Cash = start };

            Assert.AreEqual("Garage lot", HqCampusPhase.LabelForEra(config, "dial-up-days", poor));
            Assert.AreEqual("Garage → campus", HqCampusPhase.LabelForEra(config, "dial-up-days", rich));
            Assert.AreEqual("Campus HQ", HqCampusPhase.LabelForEra(config, "the-chat-boom", rich));
            Assert.AreEqual("Global campus", HqCampusPhase.LabelForEra(config, "harnesses", rich));
        }
    }
}