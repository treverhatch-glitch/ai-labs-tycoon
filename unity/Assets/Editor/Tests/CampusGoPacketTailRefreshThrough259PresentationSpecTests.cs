using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough259PresentationSpecTests
    {
        [Test]
        public void Go_packet_259()
        {
            Assert.AreEqual(259, CampusGoPacketTailRefreshThrough259PresentationSpec.LastDocumentedPushBatch);
        }
    }
}