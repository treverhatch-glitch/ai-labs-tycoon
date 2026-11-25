using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough151PresentationSpecTests
    {
        [Test]
        public void Tail_151()
        {
            Assert.AreEqual(151, CampusPresentationTailRollupRefreshThrough151PresentationSpec.RefreshThroughBatch);
        }
    }
}