using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough175PresentationSpecTests
    {
        [Test]
        public void Go_175_links()
        {
            Assert.AreEqual(175, CampusGoPacketTailRefreshThrough175PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(148, CampusGoPacketTailRefreshThrough175PresentationSpec.ManifestRefreshBatch);
        }
    }
}