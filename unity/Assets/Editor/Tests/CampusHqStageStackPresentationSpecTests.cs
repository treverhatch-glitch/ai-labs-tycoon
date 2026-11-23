using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqStageStackPresentationSpecTests
    {
        [Test]
        public void Hq_band_count_and_viewport()
        {
            Assert.AreEqual(5, CampusHqStageStackPresentationSpec.HqPresentationBandCount);
            Assert.AreEqual(0.70f, CampusHqStageStackPresentationSpec.LotViewportWidthFraction);
        }
    }
}