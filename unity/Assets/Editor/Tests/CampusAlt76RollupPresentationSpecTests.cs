using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupPresentationSpecTests
    {
        [Test]
        public void Last_batch_matches_hub_end()
        {
            Assert.AreEqual(
                CampusPresentationIndexHubPresentationSpec.LastHubBatch,
                CampusAlt76RollupPresentationSpec.LastRemoteDocumentedBatch);
        }

        [Test]
        public void Index_hub_first_batch()
        {
            Assert.AreEqual(50, CampusAlt76RollupPresentationSpec.IndexHubFirstBatch);
        }
    }
}