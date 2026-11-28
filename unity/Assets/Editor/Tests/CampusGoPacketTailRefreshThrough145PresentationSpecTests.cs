using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough145PresentationSpecTests
    {
        [Test]
        public void Documented_push_145()
        {
            Assert.AreEqual(145, CampusGoPacketTailRefreshThrough145PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Manifest_133()
        {
            Assert.AreEqual(133, CampusGoPacketTailRefreshThrough145PresentationSpec.ManifestRefreshBatch);
        }
    }
}