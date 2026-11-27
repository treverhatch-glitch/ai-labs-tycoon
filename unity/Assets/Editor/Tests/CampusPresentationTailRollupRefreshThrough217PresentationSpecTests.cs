using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough217PresentationSpecTests
    {
        [Test]
        public void Batch_353_constant_matches_lane()
        {
            Assert.AreEqual(217, CampusPresentationTailRollupRefreshThrough217PresentationSpec.RefreshThroughBatch);
        }
    }
}
