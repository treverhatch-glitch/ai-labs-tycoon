using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough102PresentationSpecTests
    {
        [Test]
        public void Documented_push_102()
        {
            Assert.AreEqual(102, CampusGoPacketTailRefreshThrough102PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Ahead_of_98()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough102PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough102PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Rollup_refresh_103()
        {
            Assert.AreEqual(103, CampusGoPacketTailRefreshThrough102PresentationSpec.RollupRefreshLastBatch);
        }
    }
}