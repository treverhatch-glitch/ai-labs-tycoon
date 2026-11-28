using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB181PresentationSpecTests
    {
        [Test]
        public void Rollup_181()
        {
            Assert.AreEqual(181, CampusAlt76RollupRefreshB181PresentationSpec.LastDocumentedBatch);
        }
    }
}