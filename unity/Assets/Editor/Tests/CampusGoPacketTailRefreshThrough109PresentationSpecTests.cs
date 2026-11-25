using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough109PresentationSpecTests
    {
        [Test]
        public void Documented_push_109()
        {
            Assert.AreEqual(109, CampusGoPacketTailRefreshThrough109PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Rollup_refresh_106()
        {
            Assert.AreEqual(106, CampusGoPacketTailRefreshThrough109PresentationSpec.RollupRefreshLastBatch);
        }

        [Test]
        public void Ahead_of_106_packet()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough109PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough109PresentationSpec.PriorDocumentedPushBatch);
        }
    }
}