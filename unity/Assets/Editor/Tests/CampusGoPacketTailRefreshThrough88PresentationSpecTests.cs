using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough88PresentationSpecTests
    {
        [Test]
        public void Documented_push_batch_88()
        {
            Assert.AreEqual(88, CampusGoPacketTailRefreshThrough88PresentationSpec.LastDocumentedPushBatch);
        }

        [Test]
        public void Status_sync_matches_through_86_spec()
        {
            Assert.AreEqual(
                CampusAlt76StatusSyncThrough86PresentationSpec.LastSyncedBatch,
                CampusGoPacketTailRefreshThrough88PresentationSpec.StatusSyncLastBatch);
        }

        [Test]
        public void Spacing_thirty_minutes()
        {
            Assert.AreEqual(30, CampusGoPacketTailRefreshThrough88PresentationSpec.MinSpacingMinutes);
        }
    }
}