using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusEraCampaignGraceHudChromePresentationSpecTests {
        [Test]
        public void Batch_365_lane_constant() {
            Assert.AreEqual(160, CampusEraCampaignGraceHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}
