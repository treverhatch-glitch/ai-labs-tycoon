using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStartEraPickerPresentationSpecTests
    {
        [Test]
        public void Picker_era_count_and_swatch()
        {
            Assert.GreaterOrEqual(CampusStartEraPickerPresentationSpec.DocumentedEraCount, 4);
            Assert.AreEqual(28f, CampusStartEraPickerPresentationSpec.SwatchSizePixels);
        }
    }
}