using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough181PresentationSpecTests
    {
        [Test]
        public void Go_181_links()
        {
            Assert.AreEqual(181, CampusGoPacketTailRefreshThrough181PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(151, CampusGoPacketTailRefreshThrough181PresentationSpec.ManifestRefreshBatch);
        }
    }
}