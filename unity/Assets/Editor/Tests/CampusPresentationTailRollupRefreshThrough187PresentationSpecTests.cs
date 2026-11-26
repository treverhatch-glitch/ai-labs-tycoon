using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough187PresentationSpecTests
    {
        [Test]
        public void Tail_rollup_187()
        {
            Assert.AreEqual(187, CampusPresentationTailRollupRefreshThrough187PresentationSpec.RefreshThroughBatch);
        }
    }
}