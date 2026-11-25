using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingIncidentIndexPresentationSpecTests
    {
        [Test]
        public void Incident_tier_count()
        {
            Assert.AreEqual(3, CampusTrainingIncidentIndexPresentationSpec.DocumentedIncidentTiers);
        }

        [Test]
        public void Pulse_scale_matches_incident_spec()
        {
            Assert.AreEqual(
                CampusTrainingIncidentPresentationSpec.PulseScaleMax,
                CampusTrainingIncidentIndexPresentationSpec.PulseScaleMax,
                0.001f);
        }
    }
}