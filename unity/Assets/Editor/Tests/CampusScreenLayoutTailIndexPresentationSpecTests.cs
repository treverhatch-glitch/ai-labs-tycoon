using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusScreenLayoutTailIndexPresentationSpecTests
    {
        [Test]
        public void Full_screen_count_matches_index()
        {
            Assert.AreEqual(3, CampusScreenLayoutTailIndexPresentationSpec.FullScreenLayoutCount);
        }

        [Test]
        public void Tail_rows_include_hub_alias()
        {
            Assert.AreEqual(4, CampusScreenLayoutTailIndexPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Hub_tail_rows_match_hub_spec()
        {
            Assert.AreEqual(
                CampusPresentationIndexHubTailPresentationSpec.TailEntryCount,
                CampusScreenLayoutTailIndexPresentationSpec.HubTailRows);
        }
    }
}