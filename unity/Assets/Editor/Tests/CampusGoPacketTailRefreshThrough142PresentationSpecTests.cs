using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough142PresentationSpecTests
    {
        [Test]
        public void Documented_push_142()
        {
            Assert.AreEqual(142, CampusGoPacketTailRefreshThrough142PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Status_133_tail_130()
        {
            Assert.AreEqual(133, CampusGoPacketTailRefreshThrough142PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(130, CampusGoPacketTailRefreshThrough142PresentationSpec.TailRollupRefreshBatch);
        }
    }
}