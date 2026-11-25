using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76RollupRefreshB85PresentationSpecTests
    {
        [Test]
        public void Last_batch_ahead_of_batch_76_refresh()
        {
            Assert.Greater(
                CampusAlt76RollupRefreshB85PresentationSpec.LastDocumentedBatch,
                CampusAlt76RollupRefreshB85PresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Hub_tail_rows_four()
        {
            Assert.AreEqual(4, CampusAlt76RollupRefreshB85PresentationSpec.HubTailRows);
        }
    }
}