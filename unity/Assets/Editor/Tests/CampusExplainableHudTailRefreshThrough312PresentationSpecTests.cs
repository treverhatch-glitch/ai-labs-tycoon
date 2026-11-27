using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudTailRefreshThrough312PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_312()
        {
            Assert.AreEqual(312,
                CampusExplainableHudTailRefreshThrough312PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}