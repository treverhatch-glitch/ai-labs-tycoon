using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB127PresentationSpecTests
    {
        [Test]
        public void Last_documented_127()
        {
            Assert.AreEqual(127, CampusAlt76RollupRefreshB127PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Prior_121()
        {
            Assert.AreEqual(121, CampusAlt76RollupRefreshB127PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Status_127()
        {
            Assert.AreEqual(127, CampusAlt76RollupRefreshB127PresentationSpec.LastStatusSyncedBatch);
        }
    }
}