using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough148PresentationSpecTests
    {
        [Test]
        public void Tail_148()
        {
            Assert.AreEqual(148, CampusPresentationTailRollupRefreshThrough148PresentationSpec.RefreshThroughBatch);
        }
    }
}