using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough169PresentationSpecTests
    {
        [Test]
        public void Go_169_links()
        {
            Assert.AreEqual(169, CampusGoPacketTailRefreshThrough169PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(145, CampusGoPacketTailRefreshThrough169PresentationSpec.ManifestRefreshBatch);
        }
    }
}