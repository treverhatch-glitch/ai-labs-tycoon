using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB190PresentationSpecTests
    {
        [Test]
        public void Rollup_190()
        {
            Assert.AreEqual(190, CampusAlt76RollupRefreshB190PresentationSpec.LastDocumentedBatch);
        }
    }
}