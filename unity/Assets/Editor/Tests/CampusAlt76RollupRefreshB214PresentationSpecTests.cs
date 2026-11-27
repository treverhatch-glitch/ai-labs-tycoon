using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB214PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B211()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB214PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB211PresentationSpec.LastDocumentedBatch);
        }
    }
}