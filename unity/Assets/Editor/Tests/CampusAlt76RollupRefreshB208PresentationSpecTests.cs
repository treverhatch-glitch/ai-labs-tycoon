using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB208PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B205()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB208PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB205PresentationSpec.LastDocumentedBatch);
        }
    }
}