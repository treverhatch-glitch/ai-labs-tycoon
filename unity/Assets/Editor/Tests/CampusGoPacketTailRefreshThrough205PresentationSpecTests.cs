using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough205PresentationSpecTests
    {
        [Test]
        public void GoPacket_205()
        {
            Assert.AreEqual(
                205,
                CampusGoPacketTailRefreshThrough205PresentationSpec.LastDocumentedPushBatch);
        }
    }
}