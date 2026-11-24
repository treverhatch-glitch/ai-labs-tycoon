using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB121PresentationSpecTests
    {
        [Test]
        public void Last_documented_121()
        {
            Assert.AreEqual(121, CampusAlt76RollupRefreshB121PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Prior_118()
        {
            Assert.AreEqual(118, CampusAlt76RollupRefreshB121PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_121()
        {
            Assert.AreEqual(121, CampusAlt76RollupRefreshB121PresentationSpec.LastStatusSyncedBatch);
        }
    }
}