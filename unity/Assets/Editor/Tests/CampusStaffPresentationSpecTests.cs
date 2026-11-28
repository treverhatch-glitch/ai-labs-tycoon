using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffPresentationSpecTests
    {
        [Test]
        public void Seat_prop_in_catalog()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusStaffPresentationSpec.PlaceholderSeatPropId));
        }

        [Test]
        public void Avatar_height_reasonable()
        {
            Assert.Greater(CampusStaffPresentationSpec.AvatarHeightMeters, 1.4f);
            Assert.Less(CampusStaffPresentationSpec.AvatarHeightMeters, 2.0f);
        }
    }
}