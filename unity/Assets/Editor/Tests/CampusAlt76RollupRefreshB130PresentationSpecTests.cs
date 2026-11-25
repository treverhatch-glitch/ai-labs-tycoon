using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB130PresentationSpecTests
    {
        [Test]
        public void Last_documented_130()
        {
            Assert.AreEqual(130, CampusAlt76RollupRefreshB130PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Status_130()
        {
            Assert.AreEqual(130, CampusAlt76RollupRefreshB130PresentationSpec.LastStatusSyncedBatch);
        }
    }
}