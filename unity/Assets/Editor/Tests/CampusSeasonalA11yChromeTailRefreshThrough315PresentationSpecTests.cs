using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalA11yChromeTailRefreshThrough315PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_315()
        {
            Assert.AreEqual(315,
                CampusSeasonalA11yChromeTailRefreshThrough315PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}