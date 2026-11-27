using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough286PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_283()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough286PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough283PresentationSpec.LastDocumentedPushBatch);
        }
    }
}