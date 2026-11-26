using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough280PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_277()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough280PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough277PresentationSpec.LastDocumentedPushBatch);
        }
    }
}