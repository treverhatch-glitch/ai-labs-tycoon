using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusUnityIntegrationHubPresentationSpecTests
    {
        [Test]
        public void Prop_count_matches_handoff_rollup()
        {
            Assert.AreEqual(
                CampusHandoffRollupPresentationSpec.ExpectedPropCount,
                CampusUnityIntegrationHubPresentationSpec.ExpectedPropImportCount);
        }
    }
}