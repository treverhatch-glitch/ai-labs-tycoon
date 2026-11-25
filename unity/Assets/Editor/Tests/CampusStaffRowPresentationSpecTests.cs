using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffRowPresentationSpecTests
    {
        [Test]
        public void Staff_icon_resolves_in_catalog()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusStaffRowPresentationSpec.StaffIconId));
        }
    }
}