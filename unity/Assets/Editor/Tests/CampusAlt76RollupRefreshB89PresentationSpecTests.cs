using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB89PresentationSpecTests
    {
        [Test]
        public void Last_batch_ahead_of_85_refresh()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB89PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB89PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_sync_matches_89()
        {
            Assert.AreEqual(
                89,
                CampusAlt76RollupRefreshB89PresentationSpec.LastStatusSyncedBatch);
        }
    }
}