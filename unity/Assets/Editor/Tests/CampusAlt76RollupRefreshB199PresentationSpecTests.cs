using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB199PresentationSpecTests
    {
        [Test]
        public void Rollup_batch_advances_from_196()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB199PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB196PresentationSpec.LastDocumentedBatch);
        }
    }
}