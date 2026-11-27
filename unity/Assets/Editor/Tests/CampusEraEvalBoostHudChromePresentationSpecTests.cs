using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusEraEvalBoostHudChromePresentationSpecTests {
        [Test]
        public void Batch_367_lane_constant() {
            Assert.AreEqual(159, CampusEraEvalBoostHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}
