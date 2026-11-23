using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqLotViewportPresentationSpecTests
    {
        [Test]
        public void Viewport_fraction_and_camera()
        {
            Assert.AreEqual(0.70f, CampusHqLotViewportPresentationSpec.ViewportWidthFraction);
            Assert.AreEqual(38f, CampusHqLotViewportPresentationSpec.CameraYawDegrees);
        }
    }
}