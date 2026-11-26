using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough214PresentationSpecTests
    {
        [Test]
        public void Refresh_through_matches_STATUS_lane_214()
        {
            Assert.AreEqual(214,
                CampusPresentationTailRollupRefreshThrough214PresentationSpec.RefreshThroughBatch);
        }
    }
}