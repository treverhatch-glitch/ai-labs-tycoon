using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusMascotPresentationSpecTests
    {
        [Test]
        public void Bob_amplitudes_increase_with_severity()
        {
            Assert.Less(CampusMascotPresentationSpec.HealthyBobMeters, CampusMascotPresentationSpec.OverworkedBobMeters);
            Assert.Less(CampusMascotPresentationSpec.OverworkedBobMeters, CampusMascotPresentationSpec.GlitchingBobMeters);
        }

        [Test]
        public void Model_presentation_uses_spec_amplitudes()
        {
            Assert.AreEqual(CampusMascotPresentationSpec.HealthyBobMeters,
                ModelMascotPresentation.ForMood(AiLabs.Rules.State.ModelMood.Healthy).BobAmplitude);
        }
    }
}