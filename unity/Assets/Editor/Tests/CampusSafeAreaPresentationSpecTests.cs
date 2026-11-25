using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSafeAreaPresentationSpecTests
    {
        [Test]
        public void Safe_inset_and_reference_size()
        {
            Assert.AreEqual(24f, CampusSafeAreaPresentationSpec.SafeInsetPixels);
            Assert.AreEqual(1920, CampusSafeAreaPresentationSpec.DesignWidthPixels);
        }
    }
}