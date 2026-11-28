using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB172PresentationSpecTests
    {
        [Test]
        public void Rollup_172()
        {
            Assert.AreEqual(172, CampusAlt76RollupRefreshB172PresentationSpec.LastDocumentedBatch);
        }
    }
}