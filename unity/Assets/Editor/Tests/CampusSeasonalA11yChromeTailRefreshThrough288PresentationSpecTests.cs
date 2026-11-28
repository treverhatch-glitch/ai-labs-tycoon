using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalA11yChromeTailRefreshThrough288PresentationSpecTests
    {
        [Test]
        public void Chrome_tail_tracks_presentation_288()
        {
            Assert.AreEqual(288,
                CampusSeasonalA11yChromeTailRefreshThrough288PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}