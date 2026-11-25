using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough187PresentationSpecTests
    {
        [Test]
        public void Status_187()
        {
            Assert.AreEqual(187, CampusAlt76StatusSyncThrough187PresentationSpec.LastSyncedBatch);
        }
    }
}