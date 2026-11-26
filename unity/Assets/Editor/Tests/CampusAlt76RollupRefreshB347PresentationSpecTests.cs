using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB347PresentationSpecTests
    {
        [Test]
        public void Batch_348_constant_matches_lane()
        {
            Assert.AreEqual(347, CampusAlt76RollupRefreshB347PresentationSpec.LastDocumentedBatch);
        }
    }
}
