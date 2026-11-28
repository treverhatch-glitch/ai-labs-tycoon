using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough241PresentationSpecTests
    {
        [Test]
        public void Go_packet_241()
        {
            Assert.AreEqual(241, CampusGoPacketTailRefreshThrough241PresentationSpec.LastDocumentedPushBatch);
        }
    }
}