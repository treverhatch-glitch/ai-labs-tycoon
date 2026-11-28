using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough169PresentationSpecTests
    {
        [Test]
        public void Status_169()
        {
            Assert.AreEqual(169, CampusAlt76StatusSyncThrough169PresentationSpec.LastSyncedBatch);
        }
    }
}