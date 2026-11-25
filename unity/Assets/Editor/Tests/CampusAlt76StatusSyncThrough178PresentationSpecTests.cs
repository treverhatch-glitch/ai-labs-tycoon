using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough178PresentationSpecTests
    {
        [Test]
        public void Status_178()
        {
            Assert.AreEqual(178, CampusAlt76StatusSyncThrough178PresentationSpec.LastSyncedBatch);
        }
    }
}