using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationIndexHubThrough347PresentationSpecTests
    {
        [Test]
        public void Batch_349_constant_matches_lane()
        {
            Assert.AreEqual(347, CampusPresentationIndexHubThrough347PresentationSpec.LastHubBatch);
        }
    }
}
