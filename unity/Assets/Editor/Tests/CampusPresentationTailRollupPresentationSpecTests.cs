using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupPresentationSpecTests
    {
        [Test]
        public void Tail_lanes_seven()
        {
            Assert.AreEqual(7, CampusPresentationTailRollupPresentationSpec.DocumentedTailLanes);
        }

        [Test]
        public void Hub_tail_four_rows()
        {
            Assert.AreEqual(4, CampusPresentationTailRollupPresentationSpec.HubTailRows);
        }

        [Test]
        public void Extended_manifest_positive()
        {
            Assert.Greater(CampusPresentationTailRollupPresentationSpec.ExtendedManifestEntries, 0);
        }
    }
}