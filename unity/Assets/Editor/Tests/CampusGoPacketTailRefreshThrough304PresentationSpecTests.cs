using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusGoPacketTailRefreshThrough304PresentationSpecTests {
        [Test]
        public void Batch_359_lane_constant() {
            Assert.AreEqual(304, CampusGoPacketTailRefreshThrough304PresentationSpec.LastDocumentedPushBatch);
        }
    }
}
