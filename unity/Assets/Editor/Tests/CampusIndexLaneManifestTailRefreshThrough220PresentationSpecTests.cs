using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusIndexLaneManifestTailRefreshThrough220PresentationSpecTests {
        [Test]
        public void Batch_361_lane_constant() {
            Assert.AreEqual(220, CampusIndexLaneManifestTailRefreshThrough220PresentationSpec.RefreshThroughBatch);
        }
    }
}
