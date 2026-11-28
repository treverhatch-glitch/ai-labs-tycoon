using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough157PresentationSpecTests
    {
        [Test]
        public void Go_157_manifest_139()
        {
            Assert.AreEqual(157, CampusGoPacketTailRefreshThrough157PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(139, CampusGoPacketTailRefreshThrough157PresentationSpec.ManifestRefreshBatch);
        }
    }
}