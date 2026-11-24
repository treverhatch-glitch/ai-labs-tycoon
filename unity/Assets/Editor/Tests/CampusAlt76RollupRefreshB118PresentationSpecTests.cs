using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB118PresentationSpecTests
    {
        [Test]
        public void Last_documented_118()
        {
            Assert.AreEqual(118, CampusAlt76RollupRefreshB118PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Prior_115()
        {
            Assert.AreEqual(115, CampusAlt76RollupRefreshB118PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_118()
        {
            Assert.AreEqual(118, CampusAlt76RollupRefreshB118PresentationSpec.LastStatusSyncedBatch);
        }
    }
}