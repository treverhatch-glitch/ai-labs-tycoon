using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalWorldTailIndexRefreshThrough312PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_312()
        {
            Assert.AreEqual(312,
                CampusRivalWorldTailIndexRefreshThrough312PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}