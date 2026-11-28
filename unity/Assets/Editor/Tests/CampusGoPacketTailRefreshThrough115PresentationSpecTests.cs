using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough115PresentationSpecTests
    {
        [Test]
        public void Documented_push_115()
        {
            Assert.AreEqual(115, CampusGoPacketTailRefreshThrough115PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Ahead_of_112()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough115PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough115PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Tail_rollup_112()
        {
            Assert.AreEqual(112, CampusGoPacketTailRefreshThrough115PresentationSpec.TailRollupRefreshBatch);
        }
    }
}