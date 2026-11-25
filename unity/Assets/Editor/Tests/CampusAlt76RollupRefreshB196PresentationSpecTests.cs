using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB196PresentationSpecTests
    {
        [Test]
        public void Rollup_batch_advances_from_193()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB196PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB193PresentationSpec.LastDocumentedBatch);
        }
    }
}