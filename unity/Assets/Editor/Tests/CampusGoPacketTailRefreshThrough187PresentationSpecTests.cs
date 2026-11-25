using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough187PresentationSpecTests
    {
        [Test]
        public void Go_187_links()
        {
            Assert.AreEqual(187, CampusGoPacketTailRefreshThrough187PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(154, CampusGoPacketTailRefreshThrough187PresentationSpec.ManifestRefreshBatch);
        }
    }
}