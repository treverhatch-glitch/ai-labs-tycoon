using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB175PresentationSpecTests
    {
        [Test]
        public void Rollup_175()
        {
            Assert.AreEqual(175, CampusAlt76RollupRefreshB175PresentationSpec.LastDocumentedBatch);
        }
    }
}