using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough229PresentationSpecTests
    {
        [Test]
        public void GoPacket_229()
        {
            Assert.AreEqual(
                229,
                CampusGoPacketTailRefreshThrough229PresentationSpec.LastDocumentedPushBatch);
        }
    }
}