using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough220PresentationSpecTests
    {
        [Test]
        public void GoPacket_220()
        {
            Assert.AreEqual(
                220,
                CampusGoPacketTailRefreshThrough220PresentationSpec.LastDocumentedPushBatch);
        }
    }
}