using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentation651Through656PresentationSpecTests
    {
        [Test] public void B651() => Assert.AreEqual(651, CampusExplainableHudTailRefresh651PresentationSpec.PresentationBatch);
        [Test] public void B656Hub() => Assert.AreEqual(656, CampusPresentationIndexHubThrough656PresentationSpec.LastHubBatch);
    }
}