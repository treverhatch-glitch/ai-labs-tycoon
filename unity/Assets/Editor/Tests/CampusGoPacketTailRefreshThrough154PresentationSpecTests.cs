using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough154PresentationSpecTests
    {
        [Test]
        public void Push_154_links()
        {
            Assert.AreEqual(154, CampusGoPacketTailRefreshThrough154PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(139, CampusGoPacketTailRefreshThrough154PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(136, CampusGoPacketTailRefreshThrough154PresentationSpec.TailRollupRefreshBatch);
        }
    }
}