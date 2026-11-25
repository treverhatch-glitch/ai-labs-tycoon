using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough139PresentationSpecTests
    {
        [Test]
        public void Tail_139()
        {
            Assert.AreEqual(139, CampusPresentationTailRollupRefreshThrough139PresentationSpec.RefreshThroughBatch);
            Assert.AreEqual(11, CampusPresentationTailRollupRefreshThrough139PresentationSpec.TailLaneCount);
        }
    }
}