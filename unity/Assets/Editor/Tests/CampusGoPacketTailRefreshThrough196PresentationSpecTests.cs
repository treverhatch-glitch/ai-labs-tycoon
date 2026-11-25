using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough196PresentationSpecTests
    {
        [Test]
        public void Go_196_links()
        {
            Assert.AreEqual(196, CampusGoPacketTailRefreshThrough196PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(160, CampusGoPacketTailRefreshThrough196PresentationSpec.StatusSyncLastBatch);
        }
    }
}