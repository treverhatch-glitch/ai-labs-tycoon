using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB163PresentationSpecTests
    {
        [Test]
        public void Rollup_163()
        {
            Assert.AreEqual(163, CampusAlt76RollupRefreshB163PresentationSpec.LastDocumentedBatch);
        }
    }
}