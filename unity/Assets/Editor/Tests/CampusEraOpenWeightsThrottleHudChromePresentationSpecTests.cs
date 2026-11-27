using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusEraOpenWeightsThrottleHudChromePresentationSpecTests {
        [Test]
        public void Batch_366_lane_constant() {
            Assert.AreEqual(158, CampusEraOpenWeightsThrottleHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}
