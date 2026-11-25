using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough250PresentationSpecTests
    {
        [Test]
        public void Go_packet_250()
        {
            Assert.AreEqual(250, CampusGoPacketTailRefreshThrough250PresentationSpec.LastDocumentedPushBatch);
        }
    }
}