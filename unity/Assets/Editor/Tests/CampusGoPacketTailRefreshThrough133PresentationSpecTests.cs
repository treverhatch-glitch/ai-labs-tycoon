using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough133PresentationSpecTests
    {
        [Test]
        public void Documented_push_133()
        {
            Assert.AreEqual(133, CampusGoPacketTailRefreshThrough133PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Manifest_127()
        {
            Assert.AreEqual(127, CampusGoPacketTailRefreshThrough133PresentationSpec.ManifestRefreshBatch);
        }
    }
}