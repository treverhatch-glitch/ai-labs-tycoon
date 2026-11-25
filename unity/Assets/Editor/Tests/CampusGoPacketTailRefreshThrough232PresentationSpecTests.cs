using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough232PresentationSpecTests
    {
        [Test]
        public void GoPacket_232()
        {
            Assert.AreEqual(
                232,
                CampusGoPacketTailRefreshThrough232PresentationSpec.LastDocumentedPushBatch);
        }
    }
}