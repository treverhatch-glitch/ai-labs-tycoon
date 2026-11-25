using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough184PresentationSpecTests
    {
        [Test]
        public void Status_184()
        {
            Assert.AreEqual(184, CampusAlt76StatusSyncThrough184PresentationSpec.LastSyncedBatch);
        }
    }
}