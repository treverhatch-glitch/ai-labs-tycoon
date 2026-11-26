using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationIndexHubThrough644PresentationSpecTests
    {
        [Test]
        public void B644HubCap()
        {
            Assert.AreEqual(644, CampusPresentationIndexHubThrough644PresentationSpec.LastHubBatch);
        }
    }
}