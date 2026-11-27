using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB202PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B199()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB202PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB199PresentationSpec.LastDocumentedBatch);
        }
    }
}