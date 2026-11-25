using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough202PresentationSpecTests
    {
        [Test]
        public void GoPacket_202()
        {
            Assert.AreEqual(
                202,
                CampusGoPacketTailRefreshThrough202PresentationSpec.LastDocumentedPushBatch);
        }
    }
}