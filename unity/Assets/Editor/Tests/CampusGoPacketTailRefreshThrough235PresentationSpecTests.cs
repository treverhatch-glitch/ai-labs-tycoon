using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough235PresentationSpecTests
    {
        [Test]
        public void GoPacket_235()
        {
            Assert.AreEqual(
                235,
                CampusGoPacketTailRefreshThrough235PresentationSpec.LastDocumentedPushBatch);
        }
    }
}