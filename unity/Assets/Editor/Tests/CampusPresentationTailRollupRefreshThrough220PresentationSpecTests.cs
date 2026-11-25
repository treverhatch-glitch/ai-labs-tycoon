using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusPresentationTailRollupRefreshThrough220PresentationSpecTests {
        [Test]
        public void Batch_362_lane_constant() {
            Assert.AreEqual(220, CampusPresentationTailRollupRefreshThrough220PresentationSpec.RefreshThroughBatch);
        }
    }
}
