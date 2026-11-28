using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough151PresentationSpecTests
    {
        [Test]
        public void Status_151()
        {
            Assert.AreEqual(151, CampusAlt76StatusSyncThrough151PresentationSpec.LastSyncedBatch);
        }
    }
}