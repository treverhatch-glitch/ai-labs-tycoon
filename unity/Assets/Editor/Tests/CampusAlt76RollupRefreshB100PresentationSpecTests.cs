using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB100PresentationSpecTests
    {
        [Test]
        public void Last_documented_batch_100()
        {
            Assert.AreEqual(100, CampusAlt76RollupRefreshB100PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Ahead_of_89_refresh()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB100PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB100PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_sync_matches_98()
        {
            Assert.AreEqual(98, CampusAlt76RollupRefreshB100PresentationSpec.LastStatusSyncedBatch);
        }
    }
}