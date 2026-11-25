using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough157PresentationSpecTests
    {
        [Test]
        public void Tail_157()
        {
            Assert.AreEqual(157, CampusPresentationTailRollupRefreshThrough157PresentationSpec.RefreshThroughBatch);
        }
    }
}