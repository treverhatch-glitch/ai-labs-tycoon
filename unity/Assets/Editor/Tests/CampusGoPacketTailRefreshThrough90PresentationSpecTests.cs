using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough90PresentationSpecTests
    {
        [Test]
        public void Documented_push_batch_90()
        {
            Assert.AreEqual(90, CampusGoPacketTailRefreshThrough90PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Ahead_of_88_refresh()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough90PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough90PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Rollup_and_status_align()
        {
            Assert.AreEqual(89, CampusGoPacketTailRefreshThrough90PresentationSpec.StatusSyncLastBatch);
            Assert.AreEqual(89, CampusGoPacketTailRefreshThrough90PresentationSpec.RollupLastBatch);
        }
    }
}