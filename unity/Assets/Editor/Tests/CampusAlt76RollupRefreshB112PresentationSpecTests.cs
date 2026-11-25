using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB112PresentationSpecTests
    {
        [Test]
        public void Last_documented_112()
        {
            Assert.AreEqual(112, CampusAlt76RollupRefreshB112PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Status_sync_109()
        {
            Assert.AreEqual(109, CampusAlt76RollupRefreshB112PresentationSpec.LastStatusSyncedBatch);
        }

        [Test]
        public void Ahead_of_106_rollup()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB112PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB112PresentationSpec.PriorRefreshLastBatch);
        }
    }
}