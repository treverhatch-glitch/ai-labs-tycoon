using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB142PresentationSpecTests
    {
        [Test]
        public void Rollup_142()
        {
            Assert.AreEqual(142, CampusAlt76RollupRefreshB142PresentationSpec.LastDocumentedBatch);
            Assert.AreEqual(142, CampusAlt76RollupRefreshB142PresentationSpec.LastStatusSyncedBatch);
        }
    }
}