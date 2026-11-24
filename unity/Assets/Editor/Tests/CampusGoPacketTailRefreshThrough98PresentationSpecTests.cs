using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough98PresentationSpecTests
    {
        [Test]
        public void Documented_push_batch_98()
        {
            Assert.AreEqual(98, CampusGoPacketTailRefreshThrough98PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Ahead_of_90_refresh()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough98PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough98PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Hq_completion_tail_99()
        {
            Assert.AreEqual(99, CampusGoPacketTailRefreshThrough98PresentationSpec.HqCompletionTailBatch);
        }
    }
}