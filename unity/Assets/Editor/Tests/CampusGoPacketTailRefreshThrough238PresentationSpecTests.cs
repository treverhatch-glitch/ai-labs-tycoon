using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough238PresentationSpecTests
    {
        [Test]
        public void GoPacket_238()
        {
            Assert.AreEqual(
                238,
                CampusGoPacketTailRefreshThrough238PresentationSpec.LastDocumentedPushBatch);
        }
    }
}