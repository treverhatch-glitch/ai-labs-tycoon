using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough265PresentationSpecTests
    {
        [Test]
        public void Push_batch_advances_from_262()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough265PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough262PresentationSpec.LastDocumentedPushBatch);
        }
    }
}