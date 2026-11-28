using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough217PresentationSpecTests
    {
        [Test]
        public void GoPacket_217()
        {
            Assert.AreEqual(
                217,
                CampusGoPacketTailRefreshThrough217PresentationSpec.LastDocumentedPushBatch);
        }
    }
}