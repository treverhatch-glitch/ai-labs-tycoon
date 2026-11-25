using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB193PresentationSpecTests
    {
        [Test]
        public void Rollup_batch_advances_from_190()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB193PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB190PresentationSpec.LastDocumentedBatch);
        }
    }
}