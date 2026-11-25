using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalFeelingIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusRivalFeelingIndexPresentationSpec.DocumentedRivalFeelingLayers);
        }

        [Test]
        public void Meter_height_matches_feeling_spec()
        {
            Assert.AreEqual(
                CampusRivalFeelingPresentationSpec.MeterHeightPixels,
                CampusRivalFeelingIndexPresentationSpec.MeterHeightPixels,
                0.001f);
        }
    }
}