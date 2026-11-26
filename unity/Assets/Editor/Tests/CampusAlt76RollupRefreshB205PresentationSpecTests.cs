using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB205PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B202()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB205PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB202PresentationSpec.LastDocumentedBatch);
        }
    }
}