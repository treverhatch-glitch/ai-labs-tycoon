using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalA11yChromeTailRefreshThrough312PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_312()
        {
            Assert.AreEqual(312,
                CampusSeasonalA11yChromeTailRefreshThrough312PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}