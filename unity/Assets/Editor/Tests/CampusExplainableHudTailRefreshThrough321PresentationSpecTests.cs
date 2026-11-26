using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusExplainableHudTailRefreshThrough321PresentationSpecTests {
        [Test]
        public void Batch_360_lane_constant() {
            Assert.AreEqual(321, CampusExplainableHudTailRefreshThrough321PresentationSpec.TailRefreshThroughPresentationBatch);
        }
    }
}
