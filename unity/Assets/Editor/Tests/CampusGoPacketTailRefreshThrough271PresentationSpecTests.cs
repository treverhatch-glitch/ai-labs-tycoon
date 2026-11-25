using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough271PresentationSpecTests
    {
        [Test]
        public void Push_batch_advances_from_268()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough271PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough268PresentationSpec.LastDocumentedPushBatch);
        }
    }
}