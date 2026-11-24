using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB115PresentationSpecTests
    {
        [Test]
        public void Last_documented_115()
        {
            Assert.AreEqual(115, CampusAlt76RollupRefreshB115PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Prior_refresh_112()
        {
            Assert.AreEqual(112, CampusAlt76RollupRefreshB115PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_sync_115()
        {
            Assert.AreEqual(115, CampusAlt76RollupRefreshB115PresentationSpec.LastStatusSyncedBatch);
        }
    }
}