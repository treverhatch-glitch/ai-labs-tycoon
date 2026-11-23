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

        [Test]
        public void last_week_hiring_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -2_500;
            lab.Ledger.LastWeek.CapitalIn = 12_000;
            string line = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,500").Or.Contain("12,000"));
        }

        [Test]
        public void last_week_construction_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_000;
            lab.Ledger.LastWeek.Hiring = -1_000;
            string line = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("6,000").Or.Contain("1,000"));
        }

        [Test]
        public void this_week_chip_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 9_000;
            lab.Ledger.ThisWeek.Revenue = 15_000;
            string line = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("15,000"));
        }

        [Test]
        public void lifetime_grants_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 50_000;
            lab.Ledger.Lifetime.Net = 20_000;
            string line = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
            Assert.That(line, Does.Contain("50,000").Or.Contain("20,000"));
        }

        [Test]
        public void last_week_site_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_000;
            lab.Ledger.LastWeek.CapitalIn = 25_000;
            string line = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("7,000").Or.Contain("25,000"));
        }

        [Test]
        public void this_week_fines_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -800;
            lab.Ledger.ThisWeek.Power = -2_200;
            string line = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("800").Or.Contain("2,200"));
        }

        [Test]
        public void last_week_salaries_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -11_000;
            lab.Ledger.LastWeek.RoomUpkeep = -900;
            string line = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("900"));
        }

        [Test]
        public void lifetime_revenue_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 200_000;
            lab.Ledger.Lifetime.ChipSales = 75_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
            Assert.That(line, Does.Contain("200,000").Or.Contain("75,000"));
        }

        [Test]
        public void last_week_gpu_upkeep_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuUpkeep = -1_200;
            lab.Ledger.LastWeek.GpuRent = -3_400;
            string line = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
            Assert.That(line, Does.Contain("1,200").Or.Contain("3,400"));
        }

        [Test]
        public void last_week_revenue_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 18_000;
            lab.Ledger.LastWeek.Net = 4_500;
            string line = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
            Assert.That(line, Does.Contain("18,000").Or.Contain("4,500"));
        }

        [Test]
        public void this_week_net_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 3_300;
            lab.Ledger.ThisWeek.Training = -6_500;
            string line = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("6,500"));
        }

        [Test]
        public void last_week_chip_dev_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipDevelopment = -4_500;
            lab.Ledger.LastWeek.Land = -2_000;
            string line = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
            Assert.That(line, Does.Contain("4,500").Or.Contain("2,000"));
        }

        [Test]
        public void this_week_grants_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_500;
            lab.Ledger.ThisWeek.Fines = -600;
            string line = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("600"));
        }

        [Test]
        public void lifetime_power_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -120_000;
            lab.Ledger.Lifetime.Fines = -8_000;
            string line = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("120,000").Or.Contain("8,000"));
        }

        [Test]
        public void last_week_grey_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 3_300;
            lab.Ledger.LastWeek.Laundered = 700;
            string line = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("700"));
        }

        [Test]
        public void this_week_construction_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -9_000;
            lab.Ledger.ThisWeek.Grants = 4_000;
            string line = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("4,000"));
        }

        [Test]
        public void last_week_training_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -12_000;
            lab.Ledger.LastWeek.Revenue = 22_000;
            string line = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("12,000").Or.Contain("22,000"));
        }

        [Test]
        public void this_week_salaries_gpu_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -6_000;
            lab.Ledger.ThisWeek.GpuRent = -1_800;
            string line = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("6,000").Or.Contain("1,800"));
        }

        [Test]
        public void last_week_hardware_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -14_000;
            lab.Ledger.LastWeek.Power = -3_500;
            string line = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(lab);
            Assert.That(line, Does.Contain("14,000").Or.Contain("3,500"));
        }

        [Test]
        public void lifetime_training_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -300_000;
            lab.Ledger.Lifetime.Laundered = 15_000;
            string line = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("300,000").Or.Contain("15,000"));
        }

        [Test]
        public void last_week_chip_sales_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 11_000;
            lab.Ledger.LastWeek.Grants = 6_000;
            string line = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("6,000"));
        }

        [Test]
        public void last_week_hiring_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -4_200;
            lab.Ledger.LastWeek.GreyRevenue = 2_800;
            string line = HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("2,800"));
        }

        [Test]
        public void this_week_power_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -5_000;
            lab.Ledger.ThisWeek.ChipSales = 8_500;
            string line = HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(lab);
            Assert.That(line, Does.Contain("5,000").Or.Contain("8,500"));
        }

        [Test]
        public void last_week_patent_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -10_000;
            lab.Ledger.LastWeek.Land = -3_500;
            string line = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("10,000").Or.Contain("3,500"));
        }

        [Test]
        public void this_week_training_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -8_000;
            lab.Ledger.ThisWeek.ChipSales = 6_200;
            string line = HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(lab);
            Assert.That(line, Does.Contain("8,000").Or.Contain("6,200"));
        }

        [Test]
        public void lifetime_net_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 90_000;
            lab.Ledger.Lifetime.Training = -400_000;
            string line = HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("90,000").Or.Contain("400,000"));
        }

        [Test]
        public void last_week_fines_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -1_100;
            lab.Ledger.LastWeek.Training = -15_000;
            string line = HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("1,100").Or.Contain("15,000"));
        }

    }
}