using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough289PresentationSpecTests
    {
        [Test]
        public void Last_documented_push_advances_from_286()
        {
            Assert.Greater(
                CampusGoPacketTailRefreshThrough289PresentationSpec.LastDocumentedPushBatch,
                CampusGoPacketTailRefreshThrough286PresentationSpec.LastDocumentedPushBatch);
        }
    }
}