using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough172PresentationSpecTests
    {
        [Test]
        public void Go_172_links()
        {
            Assert.AreEqual(172, CampusGoPacketTailRefreshThrough172PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(148, CampusGoPacketTailRefreshThrough172PresentationSpec.StatusSyncLastBatch);
        }
    }
}