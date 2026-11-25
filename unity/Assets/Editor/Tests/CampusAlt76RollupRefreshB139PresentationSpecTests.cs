using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB139PresentationSpecTests
    {
        [Test]
        public void Rollup_139()
        {
            Assert.AreEqual(139, CampusAlt76RollupRefreshB139PresentationSpec.LastDocumentedBatch);
            Assert.AreEqual(139, CampusAlt76RollupRefreshB139PresentationSpec.LastStatusSyncedBatch);
        }
    }
}