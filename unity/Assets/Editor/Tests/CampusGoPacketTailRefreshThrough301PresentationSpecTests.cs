using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough301PresentationSpecTests
    {
        [Test]
        public void Batch_355_constant_matches_lane()
        {
            Assert.AreEqual(301, CampusGoPacketTailRefreshThrough301PresentationSpec.LastDocumentedPushBatch);
        }
    }
}
