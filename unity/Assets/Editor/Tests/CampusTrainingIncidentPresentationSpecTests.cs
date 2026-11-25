using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingIncidentPresentationSpecTests
    {
        [Test]
        public void Incident_assets_resolve_in_catalog()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusTrainingIncidentPresentationSpec.ConePropId));
            Assert.NotNull(CampusIconCatalog.PathFor(CampusTrainingIncidentPresentationSpec.DangerIconId));
        }

        [Test]
        public void Rung_thresholds_are_ordered()
        {
            Assert.Less(CampusTrainingIncidentPresentationSpec.RungConeTint,
                CampusTrainingIncidentPresentationSpec.RungDangerIcon);
            Assert.Less(CampusTrainingIncidentPresentationSpec.RungDangerIcon,
                CampusTrainingIncidentPresentationSpec.RungPulseScale);
        }
    }
}