using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough151PresentationSpecTests
    {
        [Test]
        public void Documented_push_151()
        {
            Assert.AreEqual(151, CampusGoPacketTailRefreshThrough151PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Manifest_136()
        {
            Assert.AreEqual(136, CampusGoPacketTailRefreshThrough151PresentationSpec.ManifestRefreshBatch);
        }
    }
}