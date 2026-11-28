using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB166PresentationSpecTests
    {
        [Test]
        public void Rollup_166()
        {
            Assert.AreEqual(166, CampusAlt76RollupRefreshB166PresentationSpec.LastDocumentedBatch);
        }
    }
}