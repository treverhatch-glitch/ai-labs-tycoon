using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough127PresentationSpecTests
    {
        [Test]
        public void Documented_push_127()
        {
            Assert.AreEqual(127, CampusGoPacketTailRefreshThrough127PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Prior_124()
        {
            Assert.AreEqual(124, CampusGoPacketTailRefreshThrough127PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Manifest_121()
        {
            Assert.AreEqual(121, CampusGoPacketTailRefreshThrough127PresentationSpec.ManifestRefreshBatch);
        }
    }
}