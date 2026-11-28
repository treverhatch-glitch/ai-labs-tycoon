using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough163PresentationSpecTests
    {
        [Test]
        public void Go_163()
        {
            Assert.AreEqual(163, CampusGoPacketTailRefreshThrough163PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(142, CampusGoPacketTailRefreshThrough163PresentationSpec.ManifestRefreshBatch);
        }
    }
}