using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough268PresentationSpecTests
    {
        [Test]
        public void Push_batch_advances_from_265()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough268PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough265PresentationSpec.LastDocumentedPushBatch);
        }
    }
}