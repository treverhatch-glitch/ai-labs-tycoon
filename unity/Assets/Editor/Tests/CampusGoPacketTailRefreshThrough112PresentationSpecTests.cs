using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough112PresentationSpecTests
    {
        [Test]
        public void Documented_push_112()
        {
            Assert.AreEqual(112, CampusGoPacketTailRefreshThrough112PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Rollup_112()
        {
            Assert.AreEqual(112, CampusGoPacketTailRefreshThrough112PresentationSpec.RollupRefreshLastBatch);
        }

        [Test]
        public void Ahead_of_109()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough112PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough112PresentationSpec.PriorDocumentedPushBatch);
        }
    }
}