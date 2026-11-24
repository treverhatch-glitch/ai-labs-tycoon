using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough148PresentationSpecTests
    {
        [Test]
        public void Documented_push_148()
        {
            Assert.AreEqual(148, CampusGoPacketTailRefreshThrough148PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Status_136_tail_133()
        {
            Assert.AreEqual(136, CampusGoPacketTailRefreshThrough148PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(133, CampusGoPacketTailRefreshThrough148PresentationSpec.TailRollupRefreshBatch);
        }
    }
}