using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentation657Through662PresentationSpecTests
    {
        [Test] public void B662Status() => Assert.AreEqual(662, CampusAlt76StatusSync662PresentationSpec.PresentationBatch);
        [Test] public void B662Hub() => Assert.AreEqual(662, CampusPresentationIndexHubThrough662PresentationSpec.LastHubBatch);
    }
}