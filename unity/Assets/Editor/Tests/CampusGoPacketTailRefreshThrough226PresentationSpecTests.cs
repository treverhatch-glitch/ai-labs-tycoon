using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough226PresentationSpecTests
    {
        [Test]
        public void GoPacket_226()
        {
            Assert.AreEqual(
                226,
                CampusGoPacketTailRefreshThrough226PresentationSpec.LastDocumentedPushBatch);
        }
    }
}