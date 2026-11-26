using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalWorldTailIndexRefreshThrough300PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_300()
        {
            Assert.AreEqual(300,
                CampusRivalWorldTailIndexRefreshThrough300PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}