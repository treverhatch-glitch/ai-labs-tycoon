using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusBreakdownPresentationSpecTests
    {
        [Test]
        public void Drawer_fits_at_least_four_part_rows()
        {
            float need = CampusBreakdownPresentationSpec.PartRowHeightPixels * 4;
            Assert.GreaterOrEqual(CampusBreakdownPresentationSpec.DrawerMinHeightPixels, need);
        }
    }
}