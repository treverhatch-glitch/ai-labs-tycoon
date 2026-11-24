using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB104PresentationSpecTests
    {
        [Test]
        public void Last_documented_104()
        {
            Assert.AreEqual(104, CampusAlt76RollupRefreshB104PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Ahead_of_100_refresh()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB104PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB104PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_sync_102()
        {
            Assert.AreEqual(102, CampusAlt76RollupRefreshB104PresentationSpec.LastStatusSyncedBatch);
        }
    }
}