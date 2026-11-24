using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough106PresentationSpecTests
    {
        [Test]
        public void Documented_push_106()
        {
            Assert.AreEqual(106, CampusGoPacketTailRefreshThrough106PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Manifest_refresh_107()
        {
            Assert.AreEqual(107, CampusGoPacketTailRefreshThrough106PresentationSpec.ManifestTailRefreshBatch);
        }

        [Test]
        public void Ahead_of_102()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough106PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough106PresentationSpec.PriorDocumentedPushBatch);
        }
    }
}