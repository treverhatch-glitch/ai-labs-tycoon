using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudTailRefreshThrough315PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_315()
        {
            Assert.AreEqual(315,
                CampusExplainableHudTailRefreshThrough315PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}