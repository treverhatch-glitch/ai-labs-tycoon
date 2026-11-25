using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough274PresentationSpecTests
    {
        [Test]
        public void Push_batch_advances_from_271()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough274PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough271PresentationSpec.LastDocumentedPushBatch);
        }
    }
}