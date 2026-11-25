using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough223PresentationSpecTests
    {
        [Test]
        public void GoPacket_223()
        {
            Assert.AreEqual(
                223,
                CampusGoPacketTailRefreshThrough223PresentationSpec.LastDocumentedPushBatch);
        }
    }
}