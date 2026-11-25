using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough199PresentationSpecTests
    {
        [Test]
        public void Go_199_links()
        {
            Assert.AreEqual(199, CampusGoPacketTailRefreshThrough199PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(160, CampusGoPacketTailRefreshThrough199PresentationSpec.ManifestRefreshBatch);
        }
    }
}