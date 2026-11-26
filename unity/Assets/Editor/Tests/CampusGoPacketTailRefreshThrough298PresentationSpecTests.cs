using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough298PresentationSpecTests
    {
        [Test]
        public void Batch_350_constant_matches_lane()
        {
            Assert.AreEqual(298, CampusGoPacketTailRefreshThrough298PresentationSpec.LastDocumentedPushBatch);
        }
    }
}
