using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough184PresentationSpecTests
    {
        [Test]
        public void Tail_rollup_184()
        {
            Assert.AreEqual(184, CampusPresentationTailRollupRefreshThrough184PresentationSpec.RefreshThroughBatch);
        }
    }
}