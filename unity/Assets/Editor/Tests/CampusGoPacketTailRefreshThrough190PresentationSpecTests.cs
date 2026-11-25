using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough190PresentationSpecTests
    {
        [Test]
        public void Go_190_links()
        {
            Assert.AreEqual(190, CampusGoPacketTailRefreshThrough190PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(157, CampusGoPacketTailRefreshThrough190PresentationSpec.StatusSyncLastBatch);
        }
    }
}