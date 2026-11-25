using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB136PresentationSpecTests
    {
        [Test]
        public void Last_documented_136()
        {
            Assert.AreEqual(136, CampusAlt76RollupRefreshB136PresentationSpec.LastDocumentedBatch);
        }

        [Test]
        public void Status_136()
        {
            Assert.AreEqual(136, CampusAlt76RollupRefreshB136PresentationSpec.LastStatusSyncedBatch);
        }
    }
}