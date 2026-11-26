using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalA11yChromeTailRefreshThrough300PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_300()
        {
            Assert.AreEqual(300,
                CampusSeasonalA11yChromeTailRefreshThrough300PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}