using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentation669Through674PresentationSpecTests
    {
        [Test] public void B674Spine() => Assert.AreEqual(674, CampusUnityIntegrationSpine674PresentationSpec.PresentationBatch);
        [Test] public void B674Hub() => Assert.AreEqual(674, CampusPresentationIndexHubThrough674PresentationSpec.LastHubBatch);
    }
}