using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusAlt76RollupRefreshB220PresentationSpecTests {
        [Test]
        public void Batch_357_lane_constant() {
            Assert.AreEqual(220, CampusAlt76RollupRefreshB220PresentationSpec.LastDocumentedBatch);
        }
    }
}
