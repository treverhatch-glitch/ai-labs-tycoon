using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB154PresentationSpecTests
    {
        [Test]
        public void Rollup_154()
        {
            Assert.AreEqual(154, CampusAlt76RollupRefreshB154PresentationSpec.LastDocumentedBatch);
        }
    }
}