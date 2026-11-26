using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough283PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_280()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough283PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough280PresentationSpec.LastDocumentedPushBatch);
        }
    }
}