using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudTailRefreshThrough288PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_288()
        {
            Assert.AreEqual(288,
                CampusExplainableHudTailRefreshThrough288PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}