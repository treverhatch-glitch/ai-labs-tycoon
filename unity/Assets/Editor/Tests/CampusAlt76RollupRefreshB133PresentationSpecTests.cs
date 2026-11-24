using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB133PresentationSpecTests
    {
        [Test]
        public void Last_documented_133()
        {
            Assert.AreEqual(133, CampusAlt76RollupRefreshB133PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Status_133()
        {
            Assert.AreEqual(133, CampusAlt76RollupRefreshB133PresentationSpec.LastStatusSyncedBatch);
        }
    }
}