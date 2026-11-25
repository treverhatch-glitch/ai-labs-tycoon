using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough193PresentationSpecTests
    {
        [Test]
        public void Go_193_links()
        {
            Assert.AreEqual(193, CampusGoPacketTailRefreshThrough193PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(157, CampusGoPacketTailRefreshThrough193PresentationSpec.ManifestRefreshBatch);
        }
    }
}