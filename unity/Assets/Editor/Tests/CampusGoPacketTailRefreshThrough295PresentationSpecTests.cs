using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough295PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_292()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough295PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough292PresentationSpec.LastDocumentedPushBatch);
        }
    }
}