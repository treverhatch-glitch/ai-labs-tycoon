using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough256PresentationSpecTests
    {
        [Test]
        public void Go_packet_256()
        {
            Assert.AreEqual(256, CampusGoPacketTailRefreshThrough256PresentationSpec.LastDocumentedPushBatch);
        }
    }
}