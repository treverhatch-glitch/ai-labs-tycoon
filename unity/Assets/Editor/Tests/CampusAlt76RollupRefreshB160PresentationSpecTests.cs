using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB160PresentationSpecTests
    {
        [Test]
        public void Rollup_160()
        {
            Assert.AreEqual(160, CampusAlt76RollupRefreshB160PresentationSpec.LastDocumentedBatch);
        }
    }
}