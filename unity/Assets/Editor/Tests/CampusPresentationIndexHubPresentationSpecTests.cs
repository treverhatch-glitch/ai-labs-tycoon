using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationIndexHubPresentationSpecTests
    {
        [Test]
        public void Hub_entry_count_spans_batches_50_to_60()
        {
            Assert.AreEqual(11, CampusPresentationIndexHubPresentationSpec.DocumentedHubEntries);
        }

        [Test]
        public void Hub_layout_index_uses_three_full_screens()
        {
            Assert.AreEqual(3, CampusFullScreenLayoutIndexPresentationSpec.DocumentedFullScreenLayouts);
        }
    }
}