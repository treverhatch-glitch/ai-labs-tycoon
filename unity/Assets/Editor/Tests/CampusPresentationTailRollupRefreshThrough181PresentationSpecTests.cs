using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough181PresentationSpecTests
    {
        [Test]
        public void Tail_rollup_181()
        {
            Assert.AreEqual(181, CampusPresentationTailRollupRefreshThrough181PresentationSpec.RefreshThroughBatch);
        }
    }
}