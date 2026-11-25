using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB169PresentationSpecTests
    {
        [Test]
        public void Rollup_169()
        {
            Assert.AreEqual(169, CampusAlt76RollupRefreshB169PresentationSpec.LastDocumentedBatch);
        }
    }
}