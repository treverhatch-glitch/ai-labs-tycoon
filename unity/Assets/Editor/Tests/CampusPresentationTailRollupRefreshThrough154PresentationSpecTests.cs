using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough154PresentationSpecTests
    {
        [Test]
        public void Tail_154()
        {
            Assert.AreEqual(154, CampusPresentationTailRollupRefreshThrough154PresentationSpec.RefreshThroughBatch);
        }
    }
}