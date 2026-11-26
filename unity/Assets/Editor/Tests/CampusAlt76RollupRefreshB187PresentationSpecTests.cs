using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB187PresentationSpecTests
    {
        [Test]
        public void Rollup_187()
        {
            Assert.AreEqual(187, CampusAlt76RollupRefreshB187PresentationSpec.LastDocumentedBatch);
        }
    }
}