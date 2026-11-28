using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough247PresentationSpecTests
    {
        [Test]
        public void Go_packet_247()
        {
            Assert.AreEqual(247, CampusGoPacketTailRefreshThrough247PresentationSpec.LastDocumentedPushBatch);
        }
    }
}