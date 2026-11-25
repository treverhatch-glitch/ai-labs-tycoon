using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGraphicsStudioHandoffSpecHubTests
    {
        [Test]
        public void Hub_docs_match_unity_integration_spec()
        {
            Assert.AreEqual(
                CampusUnityIntegrationHubPresentationSpec.PresentationHubDoc,
                CampusGraphicsStudioHandoffSpec.PresentationHubDoc);
            Assert.AreEqual(
                CampusUnityIntegrationHubPresentationSpec.HandoffRollupDoc,
                CampusGraphicsStudioHandoffSpec.HandoffRollupDoc);
        }
    }
}