using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough160PresentationSpecTests
    {
        [Test]
        public void Go_160_links()
        {
            Assert.AreEqual(160, CampusGoPacketTailRefreshThrough160PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(142, CampusGoPacketTailRefreshThrough160PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(139, CampusGoPacketTailRefreshThrough160PresentationSpec.TailRollupRefreshBatch);
        }
    }
}