using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshPresentationSpecTests
    {
        [Test]
        public void Last_batch_ahead_of_prior_rollup()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshPresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshPresentationSpec.PriorRollupLastBatch);
        }

        [Test]
        public void Manifest_entries()
        {
            Assert.AreEqual(9, CampusAlt76RollupRefreshPresentationSpec.Post60ManifestEntries);
        }
    }
}