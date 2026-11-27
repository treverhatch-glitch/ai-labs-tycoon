using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationIndexHubThrough632PresentationSpecTests
    {
        [Test]
        public void B632HubCap()
        {
            Assert.AreEqual(632, CampusPresentationIndexHubThrough632PresentationSpec.LastHubBatch);
            Assert.AreEqual(620, CampusPresentationIndexHubThrough632PresentationSpec.PriorLastHubBatch);
        }
    }
}