using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough253PresentationSpecTests
    {
        [Test]
        public void Go_packet_253()
        {
            Assert.AreEqual(253, CampusGoPacketTailRefreshThrough253PresentationSpec.LastDocumentedPushBatch);
        }
    }
}