using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB184PresentationSpecTests
    {
        [Test]
        public void Rollup_184()
        {
            Assert.AreEqual(184, CampusAlt76RollupRefreshB184PresentationSpec.LastDocumentedBatch);
        }
    }
}