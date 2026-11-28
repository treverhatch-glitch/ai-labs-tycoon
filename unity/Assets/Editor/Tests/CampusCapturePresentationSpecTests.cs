using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusCapturePresentationSpecTests
    {
        [Test]
        public void Capture_dimensions_match_doc()
        {
            Assert.AreEqual(960, CampusCapturePresentationSpec.CaptureWidth);
            Assert.AreEqual(540, CampusCapturePresentationSpec.CaptureHeight);
        }

        [Test]
        public void Min_artifact_bytes_matches_polish_checklist()
        {
            Assert.Greater(CampusCapturePresentationSpec.MinArtifactBytes, 4095);
        }
    }
}