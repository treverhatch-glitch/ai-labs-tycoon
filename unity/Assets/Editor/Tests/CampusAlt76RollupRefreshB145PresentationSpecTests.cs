using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB145PresentationSpecTests
    {
        [Test]
        public void Rollup_145()
        {
            Assert.AreEqual(145, CampusAlt76RollupRefreshB145PresentationSpec.LastDocumentedBatch);
        }
    }
}