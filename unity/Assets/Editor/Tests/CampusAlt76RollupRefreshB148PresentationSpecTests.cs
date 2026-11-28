using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB148PresentationSpecTests
    {
        [Test]
        public void Rollup_148()
        {
            Assert.AreEqual(148, CampusAlt76RollupRefreshB148PresentationSpec.LastDocumentedBatch);
        }
    }
}