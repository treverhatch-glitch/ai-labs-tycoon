using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalsScreenHudTailPresentationSpecTests
    {
        [Test]
        public void Lab_list_fraction_positive()
        {
            Assert.Greater(CampusRivalsScreenHudTailPresentationSpec.LabListWidthFraction, 0f);
        }
    }
}