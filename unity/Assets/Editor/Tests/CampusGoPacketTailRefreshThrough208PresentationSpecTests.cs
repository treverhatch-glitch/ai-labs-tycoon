using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough208PresentationSpecTests
    {
        [Test]
        public void GoPacket_208()
        {
            Assert.AreEqual(
                208,
                CampusGoPacketTailRefreshThrough208PresentationSpec.LastDocumentedPushBatch);
        }
    }
}