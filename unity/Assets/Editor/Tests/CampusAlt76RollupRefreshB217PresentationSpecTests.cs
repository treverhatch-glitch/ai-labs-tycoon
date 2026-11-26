using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB217PresentationSpecTests
    {
        [Test]
        public void Last_documented_advances_from_B214()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB217PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB214PresentationSpec.LastDocumentedBatch);
        }
    }
}