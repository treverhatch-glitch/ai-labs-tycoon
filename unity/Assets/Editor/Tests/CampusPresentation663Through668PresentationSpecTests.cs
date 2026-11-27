using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentation663Through668PresentationSpecTests
    {
        [Test] public void B668Breakdown() => Assert.AreEqual(668, CampusDisplayBreakdownPresentationBatch668PresentationSpec.PresentationBatch);
        [Test] public void B668Hub() => Assert.AreEqual(668, CampusPresentationIndexHubThrough668PresentationSpec.LastHubBatch);
    }
}