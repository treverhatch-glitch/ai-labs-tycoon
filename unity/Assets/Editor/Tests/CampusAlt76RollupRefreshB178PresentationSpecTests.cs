using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB178PresentationSpecTests
    {
        [Test]
        public void Rollup_178()
        {
            Assert.AreEqual(178, CampusAlt76RollupRefreshB178PresentationSpec.LastDocumentedBatch);
        }
    }
}