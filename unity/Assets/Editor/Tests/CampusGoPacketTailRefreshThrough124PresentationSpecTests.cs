using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough124PresentationSpecTests
    {
        [Test]
        public void Documented_push_124()
        {
            Assert.AreEqual(124, CampusGoPacketTailRefreshThrough124PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Prior_121()
        {
            Assert.AreEqual(121, CampusGoPacketTailRefreshThrough124PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Status_121()
        {
            Assert.AreEqual(121, CampusGoPacketTailRefreshThrough124PresentationSpec.StatusSyncLastBatch);
        }
    }
}