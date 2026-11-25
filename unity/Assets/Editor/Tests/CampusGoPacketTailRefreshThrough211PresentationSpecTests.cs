using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough211PresentationSpecTests
    {
        [Test]
        public void GoPacket_211()
        {
            Assert.AreEqual(
                211,
                CampusGoPacketTailRefreshThrough211PresentationSpec.LastDocumentedPushBatch);
        }
    }
}