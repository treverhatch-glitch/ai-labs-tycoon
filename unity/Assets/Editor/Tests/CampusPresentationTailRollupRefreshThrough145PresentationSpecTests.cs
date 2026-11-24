using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough145PresentationSpecTests
    {
        [Test]
        public void Tail_145()
        {
            Assert.AreEqual(145, CampusPresentationTailRollupRefreshThrough145PresentationSpec.RefreshThroughBatch);
        }
    }
}