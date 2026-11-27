using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudTailRefreshThrough318PresentationSpecTests
    {
        [Test]
        public void Batch_351_constant_matches_lane()
        {
            Assert.AreEqual(318, CampusExplainableHudTailRefreshThrough318PresentationSpec.TailRefreshThroughPresentationBatch);
        }
    }
}
