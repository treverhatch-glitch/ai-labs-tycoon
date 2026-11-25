using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough214PresentationSpecTests
    {
        [Test]
        public void GoPacket_214()
        {
            Assert.AreEqual(
                214,
                CampusGoPacketTailRefreshThrough214PresentationSpec.LastDocumentedPushBatch);
        }
    }
}