using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB157PresentationSpecTests
    {
        [Test]
        public void Rollup_157()
        {
            Assert.AreEqual(157, CampusAlt76RollupRefreshB157PresentationSpec.LastDocumentedBatch);
        }
    }
}