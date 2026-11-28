using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough139PresentationSpecTests
    {
        [Test]
        public void Documented_push_139()
        {
            Assert.AreEqual(139, CampusGoPacketTailRefreshThrough139PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Manifest_130()
        {
            Assert.AreEqual(130, CampusGoPacketTailRefreshThrough139PresentationSpec.ManifestRefreshBatch);
        }
    }
}