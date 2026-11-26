using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough262PresentationSpecTests
    {
        [Test]
        public void Push_batch_advances_from_259()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough262PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough259PresentationSpec.LastDocumentedPushBatch);
        }
    }
}