using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough121PresentationSpecTests
    {
        [Test]
        public void Documented_push_121()
        {
            Assert.AreEqual(121, CampusGoPacketTailRefreshThrough121PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Prior_118()
        {
            Assert.AreEqual(118, CampusGoPacketTailRefreshThrough121PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Rollup_118()
        {
            Assert.AreEqual(118, CampusGoPacketTailRefreshThrough121PresentationSpec.RollupRefreshLastBatch);
        }
    }
}