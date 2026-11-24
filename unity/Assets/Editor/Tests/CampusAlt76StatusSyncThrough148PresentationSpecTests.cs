using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough148PresentationSpecTests
    {
        [Test]
        public void Status_148()
        {
            Assert.AreEqual(148, CampusAlt76StatusSyncThrough148PresentationSpec.LastSyncedBatch);
        }
    }
}