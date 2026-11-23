using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqDisplayQueriesHudTests
    {
        [Test]
        public void last_week_land_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -10_000;
            lab.Ledger.LastWeek.Hardware = -2_000;
            string line = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("10,000").Or.Contain("2,000"));
        }
        [Test]
        public void this_week_construction_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -3_000;
            lab.Ledger.ThisWeek.Salaries = -4_000;
            string line = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("3,000").Or.Contain("4,000"));
        }

    }
}