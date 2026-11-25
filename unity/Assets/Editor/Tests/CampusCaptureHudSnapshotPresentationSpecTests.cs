using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusCaptureHudSnapshotPresentationSpecTests
    {
        [Test]
        public void Min_bytes_match_capture_contract()
        {
            Assert.AreEqual(CampusCapturePresentationSpec.MinArtifactBytes,
                CampusCaptureHudSnapshotPresentationSpec.MinArtifactBytes);
        }
    }
}