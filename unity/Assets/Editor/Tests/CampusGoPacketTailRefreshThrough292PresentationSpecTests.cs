using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough292PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_289()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough292PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough289PresentationSpec.LastDocumentedPushBatch);
        }
    }
}