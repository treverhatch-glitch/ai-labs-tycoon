using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalFeelingPresentationSpecTests
    {
        [Test]
        public void NormalizedFill_maps_endpoints()
        {
            Assert.AreEqual(0f, CampusRivalFeelingPresentationSpec.NormalizedFill(-1000), 0.001f);
            Assert.AreEqual(1f, CampusRivalFeelingPresentationSpec.NormalizedFill(1000), 0.001f);
            Assert.AreEqual(0.5f, CampusRivalFeelingPresentationSpec.NormalizedFill(0), 0.001f);
        }
    }
}