using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB211PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B208()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB211PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB208PresentationSpec.LastDocumentedBatch);
        }
    }
}