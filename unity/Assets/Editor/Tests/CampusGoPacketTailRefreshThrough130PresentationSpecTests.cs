using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough130PresentationSpecTests
    {
        [Test]
        public void Documented_push_130()
        {
            Assert.AreEqual(130, CampusGoPacketTailRefreshThrough130PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Prior_127()
        {
            Assert.AreEqual(127, CampusGoPacketTailRefreshThrough130PresentationSpec.PriorDocumentedPushBatch);
        }

        [Test]
        public void Status_127()
        {
            Assert.AreEqual(127, CampusGoPacketTailRefreshThrough130PresentationSpec.StatusSyncLastBatch);
        }
    }
}