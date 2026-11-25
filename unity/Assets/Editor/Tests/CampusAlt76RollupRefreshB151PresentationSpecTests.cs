using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB151PresentationSpecTests
    {
        [Test]
        public void Rollup_151()
        {
            Assert.AreEqual(151, CampusAlt76RollupRefreshB151PresentationSpec.LastDocumentedBatch);
        }
    }
}