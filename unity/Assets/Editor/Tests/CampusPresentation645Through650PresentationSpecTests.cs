using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentation645Through650PresentationSpecTests
    {
        [Test] public void B650Host() => Assert.AreEqual(650, CampusWorldMapHostPresentationThickener650PresentationSpec.PresentationBatch);
        [Test] public void B650Hub() => Assert.AreEqual(650, CampusPresentationIndexHubThrough650PresentationSpec.LastHubBatch);
    }
}