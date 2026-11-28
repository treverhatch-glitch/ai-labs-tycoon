using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusFullScreenLayoutIndexPresentationSpecTests
    {
        [Test]
        public void Full_screen_count()
        {
            Assert.AreEqual(3, CampusFullScreenLayoutIndexPresentationSpec.DocumentedFullScreenLayouts);
        }
    }
}