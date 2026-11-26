using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRulesAdjacentGraphicsTailRefreshThrough280PresentationSpecTests
    {
        [Test]
        public void Tail_refresh_tracks_presentation_280()
        {
            Assert.AreEqual(280,
                CampusRulesAdjacentGraphicsTailRefreshThrough280PresentationSpec
                    .TailRefreshThroughPresentationBatch);
        }
    }
}