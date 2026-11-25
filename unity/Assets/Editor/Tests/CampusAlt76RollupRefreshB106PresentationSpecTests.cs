using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB106PresentationSpecTests
    {
        [Test]
        public void Last_documented_106()
        {
            Assert.AreEqual(106, CampusAlt76RollupRefreshB106PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Status_sync_104()
        {
            Assert.AreEqual(104, CampusAlt76RollupRefreshB106PresentationSpec.LastStatusSyncedBatch);
        }

        [Test]
        public void Ahead_of_104_refresh()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB106PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB106PresentationSpec.PriorRefreshLastBatch);
        }
    }
}