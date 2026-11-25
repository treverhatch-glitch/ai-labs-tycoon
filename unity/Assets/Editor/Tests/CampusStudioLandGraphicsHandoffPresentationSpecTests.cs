using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStudioLandGraphicsHandoffPresentationSpecTests
    {
        [Test]
        public void Next_milestone_row()
        {
            Assert.AreEqual(5, CampusStudioLandGraphicsHandoffPresentationSpec.DocumentedNextMilestoneRow);
        }
    }
}