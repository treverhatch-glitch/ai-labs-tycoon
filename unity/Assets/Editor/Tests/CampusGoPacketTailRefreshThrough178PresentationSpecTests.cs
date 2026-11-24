using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough178PresentationSpecTests
    {
        [Test]
        public void Go_178_links()
        {
            Assert.AreEqual(178, CampusGoPacketTailRefreshThrough178PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(151, CampusGoPacketTailRefreshThrough178PresentationSpec.StatusSyncLastBatch);
        }
    }
}