using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough118PresentationSpecTests
    {
        [Test]
        public void Documented_push_118()
        {
            Assert.AreEqual(118, CampusGoPacketTailRefreshThrough118PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Prior_115()
        {
            Assert.AreEqual(115, CampusGoPacketTailRefreshThrough118PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Rollup_115()
        {
            Assert.AreEqual(115, CampusGoPacketTailRefreshThrough118PresentationSpec.RollupRefreshLastBatch);
        }
    }
}