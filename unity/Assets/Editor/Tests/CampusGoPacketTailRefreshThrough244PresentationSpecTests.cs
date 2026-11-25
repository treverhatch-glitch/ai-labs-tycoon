using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough244PresentationSpecTests
    {
        [Test]
        public void Go_packet_244()
        {
            Assert.AreEqual(244, CampusGoPacketTailRefreshThrough244PresentationSpec.LastDocumentedPushBatch);
        }
    }
}