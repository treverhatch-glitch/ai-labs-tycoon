using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough136PresentationSpecTests
    {
        [Test]
        public void Documented_push_136()
        {
            Assert.AreEqual(136, CampusGoPacketTailRefreshThrough136PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Status_130_tail_127()
        {
            Assert.AreEqual(130, CampusGoPacketTailRefreshThrough136PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(127, CampusGoPacketTailRefreshThrough136PresentationSpec.TailRollupRefreshBatch);
        }
    }
}