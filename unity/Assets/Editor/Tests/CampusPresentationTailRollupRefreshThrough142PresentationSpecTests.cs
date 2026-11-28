using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough142PresentationSpecTests
    {
        [Test]
        public void Tail_142()
        {
            Assert.AreEqual(142, CampusPresentationTailRollupRefreshThrough142PresentationSpec.RefreshThroughBatch);
        }
    }
}