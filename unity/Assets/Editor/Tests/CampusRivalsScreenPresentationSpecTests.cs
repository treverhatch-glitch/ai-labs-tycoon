using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalsScreenPresentationSpecTests
    {
        [Test]
        public void Rivals_screen_fractions_and_icon()
        {
            Assert.AreEqual(0.35f, CampusRivalsScreenPresentationSpec.LabListWidthFraction);
            Assert.NotNull(CampusIconCatalog.PathFor(CampusRivalsScreenPresentationSpec.RivalsIconId));
        }
    }
}