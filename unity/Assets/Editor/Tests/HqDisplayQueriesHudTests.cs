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

        [Test]
        public void this_week_grey_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_400;
            lab.Ledger.ThisWeek.Net = 1_800;
            string line = HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("1,800"));
        }

        [Test]
        public void last_week_room_hiring_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_200;
            lab.Ledger.LastWeek.Hiring = -5_500;
            string line = HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("5,500"));
        }

        [Test]
        public void last_week_capital_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 20_000;
            lab.Ledger.LastWeek.GreyRevenue = 1_200;
            string line = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
            Assert.That(line, Does.Contain("20,000").Or.Contain("1,200"));
        }

        [Test]
        public void lifetime_chip_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 60_000;
            lab.Ledger.Lifetime.GreyRevenue = 9_000;
            string line = HqDisplayQueriesHud.LifetimeChipGreySummaryLine(lab);
            Assert.That(line, Does.Contain("60,000").Or.Contain("9,000"));
        }

        [Test]
        public void last_week_training_gpu_upkeep_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -11_000;
            lab.Ledger.LastWeek.GpuUpkeep = -900;
            string line = HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("900"));
        }

        [Test]
        public void last_week_chip_dev_sales_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipDevelopment = -5_500;
            lab.Ledger.LastWeek.ChipSales = 7_800;
            string line = HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("7,800"));
        }

        [Test]
        public void this_week_revenue_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 13_000;
            lab.Ledger.ThisWeek.Grants = 4_500;
            string line = HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("13,000").Or.Contain("4,500"));
        }

        [Test]
        public void last_week_construction_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -8_500;
            lab.Ledger.LastWeek.Land = -2_200;
            string line = HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(lab);
            Assert.That(line, Does.Contain("8,500").Or.Contain("2,200"));
        }

        [Test]
        public void last_week_grants_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 7_000;
            lab.Ledger.LastWeek.Revenue = 19_000;
            string line = HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("7,000").Or.Contain("19,000"));
        }

        [Test]
        public void lifetime_fines_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -12_000;
            lab.Ledger.Lifetime.Power = -95_000;
            string line = HqDisplayQueriesHud.LifetimeFinesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("12,000").Or.Contain("95,000"));
        }

        [Test]
        public void this_week_gpu_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_100;
            lab.Ledger.ThisWeek.Net = 1_400;
            string line = HqDisplayQueriesHud.ThisWeekGpuNetSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("1,400"));
        }

        [Test]
        public void last_week_patent_hiring_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -3_800;
            lab.Ledger.LastWeek.RoomUpkeep = -1_100;
            string line = HqDisplayQueriesHud.LastWeekPatentHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("3,800").Or.Contain("1,100"));
        }

        [Test]
        public void this_week_chip_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 5_600;
            lab.Ledger.ThisWeek.Construction = -4_400;
            string line = HqDisplayQueriesHud.ThisWeekChipConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("5,600").Or.Contain("4,400"));
        }

        [Test]
        public void this_week_salaries_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_700;
            lab.Ledger.ThisWeek.Fines = -500;
            string line = HqDisplayQueriesHud.ThisWeekSalariesFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("7,700").Or.Contain("500"));
        }

        [Test]
        public void last_week_net_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 2_800;
            lab.Ledger.LastWeek.Fines = -900;
            string line = HqDisplayQueriesHud.LastWeekNetFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,800").Or.Contain("900"));
        }

        [Test]
        public void this_week_grey_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_600;
            lab.Ledger.ThisWeek.Power = -3_300;
            string line = HqDisplayQueriesHud.ThisWeekGreyPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("1,600").Or.Contain("3,300"));
        }

        [Test]
        public void lifetime_grants_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 80_000;
            lab.Ledger.Lifetime.Laundered = 11_000;
            string line = HqDisplayQueriesHud.LifetimeGrantsLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("80,000").Or.Contain("11,000"));
        }

        [Test]
        public void last_week_salaries_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -9_500;
            lab.Ledger.LastWeek.Power = -4_000;
            string line = HqDisplayQueriesHud.LastWeekSalariesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("9,500").Or.Contain("4,000"));
        }

        [Test]
        public void this_week_training_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -6_200;
            lab.Ledger.ThisWeek.Power = -2_800;
            string line = HqDisplayQueriesHud.ThisWeekTrainingPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("6,200").Or.Contain("2,800"));
        }

        [Test]
        public void last_week_chip_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 8_900;
            lab.Ledger.LastWeek.GreyRevenue = 1_900;
            string line = HqDisplayQueriesHud.LastWeekChipGreySummaryLine(lab);
            Assert.That(line, Does.Contain("8,900").Or.Contain("1,900"));
        }

        [Test]
        public void this_week_gpu_fines_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_400;
            lab.Ledger.ThisWeek.Fines = -350;
            string line = HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("350"));
        }

        [Test]
        public void last_week_construction_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -5_500;
            lab.Ledger.LastWeek.GreyRevenue = 1_700;
            string line = HqDisplayQueriesHud.LastWeekConstructionGreySummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("1,700"));
        }

        [Test]
        public void lifetime_revenue_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 500_000;
            lab.Ledger.Lifetime.Power = -180_000;
            string line = HqDisplayQueriesHud.LifetimeRevenuePowerSummaryLine(lab);
            Assert.That(line, Does.Contain("500,000").Or.Contain("180,000"));
        }

        [Test]
        public void last_week_gpu_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -3_100;
            lab.Ledger.LastWeek.Fines = -750;
            string line = HqDisplayQueriesHud.LastWeekGpuFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("3,100").Or.Contain("750"));
        }

        [Test]
        public void this_week_salaries_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -8_200;
            lab.Ledger.ThisWeek.Net = 2_100;
            string line = HqDisplayQueriesHud.ThisWeekSalariesNetSummaryLine(lab);
            Assert.That(line, Does.Contain("8,200").Or.Contain("2,100"));
        }

        [Test]
        public void last_week_capital_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 30_000;
            lab.Ledger.LastWeek.Grants = 5_000;
            string line = HqDisplayQueriesHud.LastWeekCapitalPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("30,000").Or.Contain("5,000"));
        }

        [Test]
        public void this_week_chip_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 7_500;
            lab.Ledger.ThisWeek.Training = -9_500;
            string line = HqDisplayQueriesHud.ThisWeekChipTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("7,500").Or.Contain("9,500"));
        }

        [Test]
        public void last_week_revenue_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 24_000;
            lab.Ledger.LastWeek.ChipSales = 6_400;
            string line = HqDisplayQueriesHud.LastWeekRevenueChipSummaryLine(lab);
            Assert.That(line, Does.Contain("24,000").Or.Contain("6,400"));
        }

        [Test]
        public void lifetime_grey_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 22_000;
            lab.Ledger.Lifetime.Net = 110_000;
            string line = HqDisplayQueriesHud.LifetimeGreyNetSummaryLine(lab);
            Assert.That(line, Does.Contain("22,000").Or.Contain("110,000"));
        }

        [Test]
        public void this_week_construction_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -6_600;
            lab.Ledger.ThisWeek.Power = -3_900;
            string line = HqDisplayQueriesHud.ThisWeekConstructionPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("6,600").Or.Contain("3,900"));
        }

        [Test]
        public void last_week_salaries_hiring_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -10_500;
            lab.Ledger.LastWeek.Hiring = -4_000;
            string line = HqDisplayQueriesHud.LastWeekSalariesHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("10,500").Or.Contain("4,000"));
        }

        [Test]
        public void this_week_grants_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 6_800;
            lab.Ledger.ThisWeek.ChipSales = 4_200;
            string line = HqDisplayQueriesHud.ThisWeekGrantsChipSummaryLine(lab);
            Assert.That(line, Does.Contain("6,800").Or.Contain("4,200"));
        }

        [Test]
        public void last_week_net_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 5_200;
            lab.Ledger.LastWeek.Power = -4_100;
            string line = HqDisplayQueriesHud.LastWeekNetPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("5,200").Or.Contain("4,100"));
        }

        [Test]
        public void this_week_fines_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -650;
            lab.Ledger.ThisWeek.ChipSales = 5_100;
            string line = HqDisplayQueriesHud.ThisWeekFinesChipSummaryLine(lab);
            Assert.That(line, Does.Contain("650").Or.Contain("5,100"));
        }

        [Test]
        public void lifetime_chip_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 65_000;
            lab.Ledger.Lifetime.Power = -210_000;
            string line = HqDisplayQueriesHud.LifetimeChipPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("65,000").Or.Contain("210,000"));
        }

        [Test]
        public void last_week_grey_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_900;
            lab.Ledger.LastWeek.Grants = 8_000;
            string line = HqDisplayQueriesHud.LastWeekGreyGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("2,900").Or.Contain("8,000"));
        }

        [Test]
        public void this_week_gpu_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_900;
            lab.Ledger.ThisWeek.Construction = -5_800;
            string line = HqDisplayQueriesHud.ThisWeekGpuConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("1,900").Or.Contain("5,800"));
        }

        [Test]
        public void last_week_site_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -9_000;
            lab.Ledger.LastWeek.Hardware = -12_500;
            string line = HqDisplayQueriesHud.LastWeekSiteHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("12,500"));
        }

        [Test]
        public void last_week_launder_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 1_400;
            lab.Ledger.LastWeek.Fines = -850;
            string line = HqDisplayQueriesHud.LastWeekLaunderFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("1,400").Or.Contain("850"));
        }

        [Test]
        public void lifetime_training_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -350_000;
            lab.Ledger.Lifetime.ChipSales = 42_000;
            string line = HqDisplayQueriesHud.LifetimeTrainingChipSummaryLine(lab);
            Assert.That(line, Does.Contain("350,000").Or.Contain("42,000"));
        }

        [Test]
        public void this_week_revenue_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 16_000;
            lab.Ledger.ThisWeek.Training = -7_200;
            string line = HqDisplayQueriesHud.ThisWeekRevenueTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("16,000").Or.Contain("7,200"));
        }

        [Test]
        public void last_week_power_chip_dev_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -5_500;
            lab.Ledger.LastWeek.ChipDevelopment = -4_000;
            string line = HqDisplayQueriesHud.LastWeekPowerChipDevSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("4,000"));
        }

        [Test]
        public void this_week_salaries_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -9_300;
            lab.Ledger.ThisWeek.Grants = 3_600;
            string line = HqDisplayQueriesHud.ThisWeekSalariesGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("9,300").Or.Contain("3,600"));
        }

        [Test]
        public void last_week_net_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 6_700;
            lab.Ledger.LastWeek.GreyRevenue = 2_100;
            string line = HqDisplayQueriesHud.LastWeekNetGreySummaryLine(lab);
            Assert.That(line, Does.Contain("6,700").Or.Contain("2,100"));
        }

        [Test]
        public void this_week_grant_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_200;
            lab.Ledger.ThisWeek.Net = 1_900;
            string line = HqDisplayQueriesHud.ThisWeekGrantNetSummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("1,900"));
        }

        [Test]
        public void last_week_training_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -13_000;
            lab.Ledger.LastWeek.Construction = -7_200;
            string line = HqDisplayQueriesHud.LastWeekTrainingConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("13,000").Or.Contain("7,200"));
        }

        [Test]
        public void lifetime_fines_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -15_000;
            lab.Ledger.Lifetime.GreyRevenue = 18_000;
            string line = HqDisplayQueriesHud.LifetimeFinesGreySummaryLine(lab);
            Assert.That(line, Does.Contain("15,000").Or.Contain("18,000"));
        }

        [Test]
        public void this_week_chip_gpu_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 6_800;
            lab.Ledger.ThisWeek.GpuRent = -2_300;
            string line = HqDisplayQueriesHud.ThisWeekChipGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("6,800").Or.Contain("2,300"));
        }

        [Test]
        public void last_week_hiring_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -4_800;
            lab.Ledger.LastWeek.CapitalIn = 35_000;
            string line = HqDisplayQueriesHud.LastWeekHiringCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("4,800").Or.Contain("35,000"));
        }

        [Test]
        public void last_week_room_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_400;
            lab.Ledger.LastWeek.Power = -5_100;
            string line = HqDisplayQueriesHud.LastWeekRoomPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("5,100"));
        }

        [Test]
        public void this_week_grey_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_800;
            lab.Ledger.ThisWeek.Construction = -4_900;
            string line = HqDisplayQueriesHud.ThisWeekGreyConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("1,800").Or.Contain("4,900"));
        }

        [Test]
        public void last_week_chip_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 9_200;
            lab.Ledger.LastWeek.CapitalIn = 28_000;
            string line = HqDisplayQueriesHud.LastWeekChipPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("9,200").Or.Contain("28,000"));
        }

        [Test]
        public void lifetime_revenue_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 600_000;
            lab.Ledger.Lifetime.Grants = 90_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("600,000").Or.Contain("90,000"));
        }

        [Test]
        public void this_week_fines_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -1_050;
            lab.Ledger.ThisWeek.Training = -5_400;
            string line = HqDisplayQueriesHud.ThisWeekFinesTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("1,050").Or.Contain("5,400"));
        }

        [Test]
        public void last_week_gpu_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_700;
            lab.Ledger.LastWeek.GreyRevenue = 1_500;
            string line = HqDisplayQueriesHud.LastWeekGpuGreySummaryLine(lab);
            Assert.That(line, Does.Contain("2,700").Or.Contain("1,500"));
        }

        [Test]
        public void this_week_net_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 2_600;
            lab.Ledger.ThisWeek.ChipSales = 4_100;
            string line = HqDisplayQueriesHud.ThisWeekNetChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,600").Or.Contain("4,100"));
        }

        [Test]
        public void last_week_grant_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 9_000;
            lab.Ledger.LastWeek.Training = -14_000;
            string line = HqDisplayQueriesHud.LastWeekGrantTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("14,000"));
        }

        [Test]
        public void this_week_revenue_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 17_500;
            lab.Ledger.ThisWeek.Power = -4_600;
            string line = HqDisplayQueriesHud.ThisWeekRevenuePowerSummaryLine(lab);
            Assert.That(line, Does.Contain("17,500").Or.Contain("4,600"));
        }

        [Test]
        public void lifetime_chip_dev_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipDevelopment = -55_000;
            lab.Ledger.Lifetime.ChipSales = 48_000;
            string line = HqDisplayQueriesHud.LifetimeChipDevSummaryLine(lab);
            Assert.That(line, Does.Contain("55,000").Or.Contain("48,000"));
        }

        [Test]
        public void last_week_construction_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -8_800;
            lab.Ledger.LastWeek.ChipSales = 7_100;
            string line = HqDisplayQueriesHud.LastWeekConstructionChipSummaryLine(lab);
            Assert.That(line, Does.Contain("8,800").Or.Contain("7,100"));
        }

        [Test]
        public void this_week_grey_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_300;
            lab.Ledger.ThisWeek.Training = -6_100;
            string line = HqDisplayQueriesHud.ThisWeekGreyTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("2,300").Or.Contain("6,100"));
        }

        [Test]
        public void last_week_fines_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -1_200;
            lab.Ledger.LastWeek.CapitalIn = 40_000;
            string line = HqDisplayQueriesHud.LastWeekFinesCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("1,200").Or.Contain("40,000"));
        }

        [Test]
        public void this_week_gpu_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_050;
            lab.Ledger.ThisWeek.Salaries = -8_800;
            string line = HqDisplayQueriesHud.ThisWeekGpuSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("8,800"));
        }

        [Test]
        public void last_week_land_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -4_500;
            lab.Ledger.LastWeek.SiteBuild = -11_000;
            string line = HqDisplayQueriesHud.LastWeekLandPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("4,500").Or.Contain("11,000"));
        }

        [Test]
        public void lifetime_launder_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 25_000;
            lab.Ledger.Lifetime.Grants = 95_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("25,000").Or.Contain("95,000"));
        }

        [Test]
        public void this_week_construction_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -5_700;
            lab.Ledger.ThisWeek.Net = 3_400;
            string line = HqDisplayQueriesHud.ThisWeekConstructionNetSummaryLine(lab);
            Assert.That(line, Does.Contain("5,700").Or.Contain("3,400"));
        }

        [Test]
        public void last_week_room_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -1_800;
            lab.Ledger.LastWeek.GreyRevenue = 2_600;
            string line = HqDisplayQueriesHud.LastWeekRoomGreySummaryLine(lab);
            Assert.That(line, Does.Contain("1,800").Or.Contain("2,600"));
        }

        [Test]
        public void this_week_chip_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 5_900;
            lab.Ledger.ThisWeek.Fines = -480;
            string line = HqDisplayQueriesHud.ThisWeekChipFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,900").Or.Contain("480"));
        }

        [Test]
        public void last_week_hardware_hiring_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -6_300;
            lab.Ledger.LastWeek.Hiring = -2_100;
            string line = HqDisplayQueriesHud.LastWeekHardwareHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("6,300").Or.Contain("2,100"));
        }

        [Test]
        public void this_week_revenue_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 16_000;
            lab.Ledger.ThisWeek.Salaries = -7_400;
            string line = HqDisplayQueriesHud.ThisWeekRevenueSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("16,000").Or.Contain("7,400"));
        }

        [Test]
        public void lifetime_net_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 21_000;
            lab.Ledger.Lifetime.Revenue = 520_000;
            string line = HqDisplayQueriesHud.LifetimeNetLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("21,000").Or.Contain("520,000"));
        }

        [Test]
        public void last_week_site_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -12_500;
            lab.Ledger.LastWeek.Training = -8_000;
            string line = HqDisplayQueriesHud.LastWeekSiteTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("12,500").Or.Contain("8,000"));
        }

        [Test]
        public void this_week_grey_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_100;
            lab.Ledger.ThisWeek.ChipSales = 5_200;
            string line = HqDisplayQueriesHud.ThisWeekGreyChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("5,200"));
        }

        [Test]
        public void last_week_power_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -3_900;
            lab.Ledger.LastWeek.Net = 2_500;
            string line = HqDisplayQueriesHud.LastWeekPowerNetSummaryLine(lab);
            Assert.That(line, Does.Contain("3,900").Or.Contain("2,500"));
        }

        [Test]
        public void this_week_construction_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -4_200;
            lab.Ledger.ThisWeek.GreyRevenue = 1_700;
            string line = HqDisplayQueriesHud.ThisWeekConstructionGreySummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("1,700"));
        }

        [Test]
        public void last_week_gpu_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_400;
            lab.Ledger.LastWeek.Land = -18_000;
            string line = HqDisplayQueriesHud.LastWeekGpuLandSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("18,000"));
        }

        [Test]
        public void lifetime_power_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -120_000;
            lab.Ledger.Lifetime.ChipSales = 200_000;
            string line = HqDisplayQueriesHud.LifetimePowerChipSummaryLine(lab);
            Assert.That(line, Does.Contain("120,000").Or.Contain("200,000"));
        }

        [Test]
        public void this_week_gpu_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_950;
            lab.Ledger.ThisWeek.GreyRevenue = 1_300;
            string line = HqDisplayQueriesHud.ThisWeekGpuGreySummaryLine(lab);
            Assert.That(line, Does.Contain("1,950").Or.Contain("1,300"));
        }

        [Test]
        public void last_week_patent_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 22_000;
            lab.Ledger.LastWeek.Net = 3_600;
            string line = HqDisplayQueriesHud.LastWeekPatentNetSummaryLine(lab);
            Assert.That(line, Does.Contain("22,000").Or.Contain("3,600"));
        }

        [Test]
        public void this_week_training_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -5_800;
            lab.Ledger.ThisWeek.Grants = 6_500;
            string line = HqDisplayQueriesHud.ThisWeekTrainingGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("5,800").Or.Contain("6,500"));
        }

        [Test]
        public void last_week_capital_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 50_000;
            lab.Ledger.LastWeek.Hardware = -5_500;
            string line = HqDisplayQueriesHud.LastWeekCapitalHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("50,000").Or.Contain("5,500"));
        }

        [Test]
        public void this_week_fines_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -650;
            lab.Ledger.ThisWeek.Net = 2_100;
            string line = HqDisplayQueriesHud.ThisWeekFinesNetSummaryLine(lab);
            Assert.That(line, Does.Contain("650").Or.Contain("2,100"));
        }

        [Test]
        public void lifetime_grants_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 88_000;
            lab.Ledger.Lifetime.ChipSales = 175_000;
            string line = HqDisplayQueriesHud.LifetimeGrantsChipSummaryLine(lab);
            Assert.That(line, Does.Contain("88,000").Or.Contain("175,000"));
        }

        [Test]
        public void last_week_launder_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 3_200;
            lab.Ledger.LastWeek.Power = -4_100;
            string line = HqDisplayQueriesHud.LastWeekLaunderPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("3,200").Or.Contain("4,100"));
        }

        [Test]
        public void this_week_salaries_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -8_200;
            lab.Ledger.ThisWeek.Construction = -3_900;
            string line = HqDisplayQueriesHud.ThisWeekSalariesConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("8,200").Or.Contain("3,900"));
        }

        [Test]
        public void last_week_chip_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_800;
            lab.Ledger.LastWeek.Training = -11_000;
            string line = HqDisplayQueriesHud.LastWeekChipTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("7,800").Or.Contain("11,000"));
        }

        [Test]
        public void this_week_grants_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_500;
            lab.Ledger.ThisWeek.Power = -2_900;
            string line = HqDisplayQueriesHud.ThisWeekGrantsPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("2,900"));
        }

        [Test]
        public void last_week_room_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -1_900;
            lab.Ledger.LastWeek.Land = -14_000;
            string line = HqDisplayQueriesHud.LastWeekRoomLandSummaryLine(lab);
            Assert.That(line, Does.Contain("1,900").Or.Contain("14,000"));
        }

        [Test]
        public void lifetime_fines_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -18_000;
            lab.Ledger.Lifetime.Training = -220_000;
            string line = HqDisplayQueriesHud.LifetimeFinesTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("18,000").Or.Contain("220,000"));
        }

        [Test]
        public void this_week_net_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 15_500;
            lab.Ledger.ThisWeek.Net = 1_800;
            string line = HqDisplayQueriesHud.ThisWeekNetRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("15,500").Or.Contain("1,800"));
        }

        [Test]
        public void last_week_hiring_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -3_400;
            lab.Ledger.LastWeek.SiteBuild = -9_800;
            string line = HqDisplayQueriesHud.LastWeekHiringSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("3,400").Or.Contain("9,800"));
        }

        [Test]
        public void this_week_chip_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 4_600;
            lab.Ledger.ThisWeek.GpuRent = -1_750;
            string line = HqDisplayQueriesHud.ThisWeekChipRentSummaryLine(lab);
            Assert.That(line, Does.Contain("4,600").Or.Contain("1,750"));
        }

        [Test]
        public void last_week_patent_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_400;
            lab.Ledger.LastWeek.CapitalIn = 32_000;
            string line = HqDisplayQueriesHud.LastWeekPatentGreySummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("32,000"));
        }

        [Test]
        public void this_week_rent_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_050;
            lab.Ledger.ThisWeek.Net = 2_400;
            string line = HqDisplayQueriesHud.ThisWeekRentNetSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("2,400"));
        }

        [Test]
        public void lifetime_revenue_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 610_000;
            lab.Ledger.Lifetime.Laundered = 19_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("610,000").Or.Contain("19,000"));
        }

        [Test]
        public void last_week_construction_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_700;
            lab.Ledger.LastWeek.Power = -3_200;
            string line = HqDisplayQueriesHud.LastWeekConstructionPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("6,700").Or.Contain("3,200"));
        }

        [Test]
        public void last_week_salaries_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -8_900;
            lab.Ledger.LastWeek.GreyRevenue = 1_600;
            string line = HqDisplayQueriesHud.LastWeekSalariesGreySummaryLine(lab);
            Assert.That(line, Does.Contain("8,900").Or.Contain("1,600"));
        }

        [Test]
        public void this_week_grant_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_800;
            lab.Ledger.ThisWeek.ChipSales = 5_100;
            string line = HqDisplayQueriesHud.ThisWeekGrantChipSummaryLine(lab);
            Assert.That(line, Does.Contain("4,800").Or.Contain("5,100"));
        }

        [Test]
        public void last_week_hardware_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -5_100;
            lab.Ledger.LastWeek.SiteBuild = -10_500;
            string line = HqDisplayQueriesHud.LastWeekHardwareSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("5,100").Or.Contain("10,500"));
        }

        [Test]
        public void this_week_fines_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -420;
            lab.Ledger.ThisWeek.Construction = -4_100;
            string line = HqDisplayQueriesHud.ThisWeekFinesConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("420").Or.Contain("4,100"));
        }

        [Test]
        public void lifetime_grey_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 42_000;
            lab.Ledger.Lifetime.Training = -200_000;
            string line = HqDisplayQueriesHud.LifetimeGreyTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("42,000").Or.Contain("200,000"));
        }

        [Test]
        public void last_week_grant_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 6_200;
            lab.Ledger.LastWeek.Laundered = 2_800;
            string line = HqDisplayQueriesHud.LastWeekGrantLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("6,200").Or.Contain("2,800"));
        }

        [Test]
        public void this_week_grant_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 3_900;
            lab.Ledger.ThisWeek.Power = -2_600;
            string line = HqDisplayQueriesHud.ThisWeekGrantPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("3,900").Or.Contain("2,600"));
        }

        [Test]
        public void last_week_training_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -9_400;
            lab.Ledger.LastWeek.GreyRevenue = 1_900;
            string line = HqDisplayQueriesHud.LastWeekTrainingGreySummaryLine(lab);
            Assert.That(line, Does.Contain("9,400").Or.Contain("1,900"));
        }

        [Test]
        public void last_week_revenue_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 24_000;
            lab.Ledger.LastWeek.CapitalIn = 45_000;
            string line = HqDisplayQueriesHud.LastWeekRevenueCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("24,000").Or.Contain("45,000"));
        }

        [Test]
        public void this_week_net_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_100;
            lab.Ledger.ThisWeek.Net = 2_300;
            string line = HqDisplayQueriesHud.ThisWeekNetGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,100").Or.Contain("2,300"));
        }

        [Test]
        public void lifetime_launder_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 20_500;
            lab.Ledger.Lifetime.GreyRevenue = 38_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderNetSummaryLine(lab);
            Assert.That(line, Does.Contain("20,500").Or.Contain("38,000"));
        }

        [Test]
        public void last_week_fines_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -880;
            lab.Ledger.LastWeek.Hardware = -4_200;
            string line = HqDisplayQueriesHud.LastWeekFinesHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("880").Or.Contain("4,200"));
        }

        [Test]
        public void this_week_salaries_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_600;
            lab.Ledger.ThisWeek.Power = -2_700;
            string line = HqDisplayQueriesHud.ThisWeekSalariesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("7,600").Or.Contain("2,700"));
        }

        [Test]
        public void last_week_grant_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 5_800;
            lab.Ledger.LastWeek.SiteBuild = -9_200;
            string line = HqDisplayQueriesHud.LastWeekGrantSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("5,800").Or.Contain("9,200"));
        }

        [Test]
        public void this_week_construction_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -3_800;
            lab.Ledger.ThisWeek.ChipSales = 4_900;
            string line = HqDisplayQueriesHud.ThisWeekConstructionChipSummaryLine(lab);
            Assert.That(line, Does.Contain("3,800").Or.Contain("4,900"));
        }

        [Test]
        public void last_week_net_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -16_000;
            lab.Ledger.LastWeek.Net = 3_100;
            string line = HqDisplayQueriesHud.LastWeekNetLandSummaryLine(lab);
            Assert.That(line, Does.Contain("16,000").Or.Contain("3,100"));
        }

        [Test]
        public void lifetime_hardware_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -210_000;
            lab.Ledger.Lifetime.Power = -105_000;
            string line = HqDisplayQueriesHud.LifetimeHardwareTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("210,000").Or.Contain("105,000"));
        }

        [Test]
        public void this_week_grey_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_900;
            lab.Ledger.ThisWeek.Grants = 3_600;
            string line = HqDisplayQueriesHud.ThisWeekGreyGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("1,900").Or.Contain("3,600"));
        }

        [Test]
        public void last_week_chip_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 8_100;
            lab.Ledger.LastWeek.Power = -3_400;
            string line = HqDisplayQueriesHud.LastWeekChipPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("8,100").Or.Contain("3,400"));
        }

        [Test]
        public void this_week_training_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -5_500;
            lab.Ledger.ThisWeek.Fines = -390;
            string line = HqDisplayQueriesHud.ThisWeekTrainingFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("390"));
        }

        [Test]
        public void last_week_salaries_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -8_400;
            lab.Ledger.LastWeek.SiteBuild = -7_600;
            string line = HqDisplayQueriesHud.LastWeekSalariesSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("8,400").Or.Contain("7,600"));
        }

        [Test]
        public void this_week_revenue_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 14_200;
            lab.Ledger.ThisWeek.GreyRevenue = 2_050;
            string line = HqDisplayQueriesHud.ThisWeekRevenueGreySummaryLine(lab);
            Assert.That(line, Does.Contain("14,200").Or.Contain("2,050"));
        }

        [Test]
        public void lifetime_patent_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 92_000;
            lab.Ledger.Lifetime.Revenue = 640_000;
            string line = HqDisplayQueriesHud.LifetimePatentGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("92,000").Or.Contain("640,000"));
        }

        [Test]
        public void last_week_training_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -10_200;
            lab.Ledger.LastWeek.CapitalIn = 38_000;
            string line = HqDisplayQueriesHud.LastWeekTrainingCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("10,200").Or.Contain("38,000"));
        }

        [Test]
        public void this_week_gpu_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_880;
            lab.Ledger.ThisWeek.Grants = 4_400;
            string line = HqDisplayQueriesHud.ThisWeekGpuGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("1,880").Or.Contain("4,400"));
        }

        [Test]
        public void last_week_grey_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_200;
            lab.Ledger.LastWeek.Land = -12_500;
            string line = HqDisplayQueriesHud.LastWeekGreyLandSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("12,500"));
        }

        [Test]
        public void this_week_net_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 1_950;
            lab.Ledger.ThisWeek.Construction = -4_300;
            string line = HqDisplayQueriesHud.ThisWeekNetConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("1,950").Or.Contain("4,300"));
        }

        [Test]
        public void last_week_revenue_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 21_000;
            lab.Ledger.LastWeek.Laundered = 2_600;
            string line = HqDisplayQueriesHud.LastWeekRevenueLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("21,000").Or.Contain("2,600"));
        }

        [Test]
        public void lifetime_site_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 190_000;
            lab.Ledger.Lifetime.Grants = 96_000;
            string line = HqDisplayQueriesHud.LifetimeSiteChipSummaryLine(lab);
            Assert.That(line, Does.Contain("190,000").Or.Contain("96,000"));
        }

        [Test]
        public void this_week_fines_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -520;
            lab.Ledger.ThisWeek.GreyRevenue = 1_750;
            string line = HqDisplayQueriesHud.ThisWeekFinesGreySummaryLine(lab);
            Assert.That(line, Does.Contain("520").Or.Contain("1,750"));
        }

        [Test]
        public void last_week_hardware_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -5_600;
            lab.Ledger.LastWeek.Net = 2_800;
            string line = HqDisplayQueriesHud.LastWeekHardwareNetSummaryLine(lab);
            Assert.That(line, Does.Contain("5,600").Or.Contain("2,800"));
        }

        [Test]
        public void this_week_grant_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_200;
            lab.Ledger.ThisWeek.Training = -6_300;
            string line = HqDisplayQueriesHud.ThisWeekGrantTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("5,200").Or.Contain("6,300"));
        }

        [Test]
        public void last_week_patent_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -3_500;
            lab.Ledger.LastWeek.Fines = -720;
            string line = HqDisplayQueriesHud.LastWeekPatentPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("3,500").Or.Contain("720"));
        }

        [Test]
        public void this_week_chip_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 4_400;
            lab.Ledger.ThisWeek.Grants = 3_800;
            string line = HqDisplayQueriesHud.ThisWeekChipGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,400").Or.Contain("3,800"));
        }

        [Test]
        public void lifetime_net_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 185_000;
            lab.Ledger.Lifetime.Grants = 94_000;
            string line = HqDisplayQueriesHud.LifetimeNetChipSummaryLine(lab);
            Assert.That(line, Does.Contain("185,000").Or.Contain("94,000"));
        }

        [Test]
        public void last_week_site_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -8_900;
            lab.Ledger.LastWeek.GreyRevenue = 2_100;
            string line = HqDisplayQueriesHud.LastWeekSiteGreySummaryLine(lab);
            Assert.That(line, Does.Contain("8,900").Or.Contain("2,100"));
        }

        [Test]
        public void this_week_revenue_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 13_800;
            lab.Ledger.ThisWeek.Fines = -460;
            string line = HqDisplayQueriesHud.ThisWeekRevenueFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("13,800").Or.Contain("460"));
        }

        [Test]
        public void last_week_grant_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 4_900;
            lab.Ledger.LastWeek.Hardware = -4_700;
            string line = HqDisplayQueriesHud.LastWeekGrantHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("4,900").Or.Contain("4,700"));
        }

        [Test]
        public void this_week_rent_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_100;
            lab.Ledger.ThisWeek.Grants = 4_200;
            string line = HqDisplayQueriesHud.ThisWeekRentGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("4,200"));
        }

        [Test]
        public void last_week_fines_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -1_100;
            lab.Ledger.LastWeek.SiteBuild = -7_800;
            string line = HqDisplayQueriesHud.LastWeekFinesSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("1,100").Or.Contain("7,800"));
        }

        [Test]
        public void lifetime_grey_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 41_000;
            lab.Ledger.Lifetime.Power = -118_000;
            string line = HqDisplayQueriesHud.LifetimeGreyPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("41,000").Or.Contain("118,000"));
        }

        [Test]
        public void this_week_salaries_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_900;
            lab.Ledger.ThisWeek.Grants = 3_500;
            string line = HqDisplayQueriesHud.ThisWeekSalariesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("7,900").Or.Contain("3,500"));
        }

        [Test]
        public void last_week_chip_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_200;
            lab.Ledger.LastWeek.Land = -15_000;
            string line = HqDisplayQueriesHud.LastWeekChipLandSummaryLine(lab);
            Assert.That(line, Does.Contain("7,200").Or.Contain("15,000"));
        }

        [Test]
        public void last_week_upkeep_gpu_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuUpkeep = -1_400;
            lab.Ledger.LastWeek.GpuRent = -2_300;
            string line = HqDisplayQueriesHud.LastWeekUpkeepGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("1,400").Or.Contain("2,300"));
        }

        [Test]
        public void last_week_training_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -9_800;
            lab.Ledger.LastWeek.SiteBuild = -6_400;
            string line = HqDisplayQueriesHud.LastWeekTrainingSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("9,800").Or.Contain("6,400"));
        }

        [Test]
        public void this_week_net_chip_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 2_050;
            lab.Ledger.ThisWeek.ChipSales = 4_700;
            string line = HqDisplayQueriesHud.ThisWeekNetChipGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("4,700"));
        }

        [Test]
        public void lifetime_launder_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 22_000;
            lab.Ledger.Lifetime.ChipSales = 188_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderChipSummaryLine(lab);
            Assert.That(line, Does.Contain("22,000").Or.Contain("188,000"));
        }

        [Test]
        public void last_week_room_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_100;
            lab.Ledger.LastWeek.Revenue = 19_500;
            string line = HqDisplayQueriesHud.LastWeekRoomPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("19,500"));
        }

        [Test]
        public void this_week_power_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -2_800;
            lab.Ledger.ThisWeek.GreyRevenue = 1_650;
            string line = HqDisplayQueriesHud.ThisWeekPowerGreySummaryLine(lab);
            Assert.That(line, Does.Contain("2,800").Or.Contain("1,650"));
        }

        [Test]
        public void last_week_revenue_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 20_500;
            lab.Ledger.LastWeek.Hardware = -4_900;
            string line = HqDisplayQueriesHud.LastWeekRevenueHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("20,500").Or.Contain("4,900"));
        }

        [Test]
        public void this_week_grant_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_600;
            lab.Ledger.ThisWeek.Construction = -3_700;
            string line = HqDisplayQueriesHud.ThisWeekGrantConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("4,600").Or.Contain("3,700"));
        }

        [Test]
        public void lifetime_fines_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -19_500;
            lab.Ledger.Lifetime.ChipSales = 192_000;
            string line = HqDisplayQueriesHud.LifetimeFinesChipSummaryLine(lab);
            Assert.That(line, Does.Contain("19,500").Or.Contain("192,000"));
        }

        [Test]
        public void last_week_launder_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 2_900;
            lab.Ledger.LastWeek.ChipSales = 6_800;
            string line = HqDisplayQueriesHud.LastWeekLaunderChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,900").Or.Contain("6,800"));
        }

        [Test]
        public void last_week_capital_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 42_000;
            lab.Ledger.LastWeek.Laundered = 2_400;
            string line = HqDisplayQueriesHud.LastWeekCapitalLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("42,000").Or.Contain("2,400"));
        }

        [Test]
        public void this_week_grant_chip_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 3_300;
            lab.Ledger.ThisWeek.Net = 1_850;
            string line = HqDisplayQueriesHud.ThisWeekGrantChipNetSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("1,850"));
        }

        [Test]
        public void last_week_patent_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -8_700;
            lab.Ledger.LastWeek.GpuRent = -2_150;
            string line = HqDisplayQueriesHud.LastWeekPatentTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("8,700").Or.Contain("2,150"));
        }

        [Test]
        public void last_week_grey_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_300;
            lab.Ledger.LastWeek.Hardware = -4_600;
            string line = HqDisplayQueriesHud.LastWeekGreyHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("2,300").Or.Contain("4,600"));
        }

        [Test]
        public void this_week_net_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 2_200;
            lab.Ledger.ThisWeek.Power = -2_900;
            string line = HqDisplayQueriesHud.ThisWeekNetPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("2,900"));
        }

        [Test]
        public void lifetime_revenue_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 650_000;
            lab.Ledger.Lifetime.Laundered = 23_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueNetSummaryLine(lab);
            Assert.That(line, Does.Contain("650,000").Or.Contain("23,000"));
        }

        [Test]
        public void last_week_grant_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 5_400;
            lab.Ledger.LastWeek.CapitalIn = 36_000;
            string line = HqDisplayQueriesHud.LastWeekGrantPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("5,400").Or.Contain("36,000"));
        }

        [Test]
        public void this_week_chip_build_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 4_800;
            lab.Ledger.ThisWeek.Construction = -3_600;
            string line = HqDisplayQueriesHud.ThisWeekChipBuildSummaryLine(lab);
            Assert.That(line, Does.Contain("4,800").Or.Contain("3,600"));
        }

        [Test]
        public void last_week_launder_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 2_700;
            lab.Ledger.LastWeek.GreyRevenue = 1_950;
            string line = HqDisplayQueriesHud.LastWeekLaunderGreySummaryLine(lab);
            Assert.That(line, Does.Contain("2,700").Or.Contain("1,950"));
        }

        [Test]
        public void this_week_grant_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_300;
            lab.Ledger.ThisWeek.Revenue = 15_600;
            string line = HqDisplayQueriesHud.ThisWeekGrantRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("4,300").Or.Contain("15,600"));
        }

        [Test]
        public void last_week_site_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_400;
            lab.Ledger.LastWeek.RoomUpkeep = -2_000;
            string line = HqDisplayQueriesHud.LastWeekSiteRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("7,400").Or.Contain("2,000"));
        }

        [Test]
        public void lifetime_training_grants_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 98_000;
            lab.Ledger.Lifetime.Training = -215_000;
            string line = HqDisplayQueriesHud.LifetimeTrainingGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("98,000").Or.Contain("215,000"));
        }

        [Test]
        public void last_week_capital_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 44_000;
            lab.Ledger.LastWeek.SiteBuild = -8_200;
            string line = HqDisplayQueriesHud.LastWeekCapitalSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("44,000").Or.Contain("8,200"));
        }

        [Test]
        public void this_week_rent_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_920;
            lab.Ledger.ThisWeek.ChipSales = 5_100;
            string line = HqDisplayQueriesHud.ThisWeekRentChipSummaryLine(lab);
            Assert.That(line, Does.Contain("1,920").Or.Contain("5,100"));
        }

        [Test]
        public void last_week_fines_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -950;
            lab.Ledger.LastWeek.Grants = 4_800;
            string line = HqDisplayQueriesHud.LastWeekFinesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("950").Or.Contain("4,800"));
        }

        [Test]
        public void last_week_power_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -3_800;
            lab.Ledger.LastWeek.Land = -14_500;
            string line = HqDisplayQueriesHud.LastWeekPowerLandSummaryLine(lab);
            Assert.That(line, Does.Contain("3,800").Or.Contain("14,500"));
        }

        [Test]
        public void this_week_grant_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 3_700;
            lab.Ledger.ThisWeek.GreyRevenue = 1_850;
            string line = HqDisplayQueriesHud.ThisWeekGrantGreySummaryLine(lab);
            Assert.That(line, Does.Contain("3,700").Or.Contain("1,850"));
        }

        [Test]
        public void lifetime_hardware_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 195_000;
            lab.Ledger.Lifetime.Power = -122_000;
            string line = HqDisplayQueriesHud.LifetimeHardwareChipSummaryLine(lab);
            Assert.That(line, Does.Contain("195,000").Or.Contain("122,000"));
        }

        [Test]
        public void last_week_room_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_300;
            lab.Ledger.LastWeek.Training = -9_100;
            string line = HqDisplayQueriesHud.LastWeekRoomTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("2,300").Or.Contain("9,100"));
        }

        [Test]
        public void this_week_training_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -5_600;
            lab.Ledger.ThisWeek.Net = 1_900;
            string line = HqDisplayQueriesHud.ThisWeekTrainingNetSummaryLine(lab);
            Assert.That(line, Does.Contain("5,600").Or.Contain("1,900"));
        }

        [Test]
        public void last_week_chip_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_500;
            lab.Ledger.LastWeek.Grants = 5_100;
            string line = HqDisplayQueriesHud.LastWeekChipGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("7,500").Or.Contain("5,100"));
        }

        [Test]
        public void this_week_gpu_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_050;
            lab.Ledger.ThisWeek.Power = -2_400;
            string line = HqDisplayQueriesHud.ThisWeekGpuPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("2,400"));
        }

        [Test]
        public void lifetime_patent_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 198_000;
            lab.Ledger.Lifetime.Revenue = 655_000;
            string line = HqDisplayQueriesHud.LifetimePatentChipSummaryLine(lab);
            Assert.That(line, Does.Contain("198,000").Or.Contain("655,000"));
        }

        [Test]
        public void last_week_net_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 2_900;
            lab.Ledger.LastWeek.Construction = -5_800;
            string line = HqDisplayQueriesHud.LastWeekNetConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("2,900").Or.Contain("5,800"));
        }

        [Test]
        public void last_week_hiring_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -3_200;
            lab.Ledger.LastWeek.Hardware = -4_800;
            string line = HqDisplayQueriesHud.LastWeekHiringHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("3,200").Or.Contain("4,800"));
        }

        [Test]
        public void this_week_salaries_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_500;
            lab.Ledger.ThisWeek.ChipSales = 4_200;
            string line = HqDisplayQueriesHud.ThisWeekSalariesChipSummaryLine(lab);
            Assert.That(line, Does.Contain("7,500").Or.Contain("4,200"));
        }

        [Test]
        public void last_week_grey_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_100;
            lab.Ledger.LastWeek.Laundered = 2_500;
            string line = HqDisplayQueriesHud.LastWeekGreyPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("2,500"));
        }

        [Test]
        public void last_week_land_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -13_000;
            lab.Ledger.LastWeek.Grants = 5_200;
            string line = HqDisplayQueriesHud.LastWeekLandGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("13,000").Or.Contain("5,200"));
        }

        [Test]
        public void this_week_fines_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -410;
            lab.Ledger.ThisWeek.Grants = 3_900;
            string line = HqDisplayQueriesHud.ThisWeekFinesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("410").Or.Contain("3,900"));
        }

        [Test]
        public void lifetime_site_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 43_500;
            lab.Ledger.Lifetime.Laundered = 24_000;
            string line = HqDisplayQueriesHud.LifetimeSiteGreySummaryLine(lab);
            Assert.That(line, Does.Contain("43,500").Or.Contain("24,000"));
        }

        [Test]
        public void last_week_gpu_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_200;
            lab.Ledger.LastWeek.Hardware = -4_500;
            string line = HqDisplayQueriesHud.LastWeekGpuHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("4,500"));
        }

        [Test]
        public void this_week_chip_grey_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 4_600;
            lab.Ledger.ThisWeek.GreyRevenue = 1_720;
            string line = HqDisplayQueriesHud.ThisWeekChipGreyGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,600").Or.Contain("1,720"));
        }

        [Test]
        public void last_week_training_hiring_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -9_500;
            lab.Ledger.LastWeek.Hiring = -3_100;
            string line = HqDisplayQueriesHud.LastWeekTrainingHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("9,500").Or.Contain("3,100"));
        }

        [Test]
        public void this_week_revenue_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 14_400;
            lab.Ledger.ThisWeek.Construction = -4_100;
            string line = HqDisplayQueriesHud.ThisWeekRevenueConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("14,400").Or.Contain("4,100"));
        }

        [Test]
        public void lifetime_grant_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 100_000;
            lab.Ledger.Lifetime.Fines = -20_000;
            string line = HqDisplayQueriesHud.LifetimeGrantFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("100,000").Or.Contain("20,000"));
        }

        [Test]
        public void this_week_salaries_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_700;
            lab.Ledger.ThisWeek.GreyRevenue = 1_800;
            string line = HqDisplayQueriesHud.ThisWeekSalariesGreySummaryLine(lab);
            Assert.That(line, Does.Contain("7,700").Or.Contain("1,800"));
        }

        [Test]
        public void last_week_revenue_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 19_800;
            lab.Ledger.LastWeek.SiteBuild = -8_100;
            string line = HqDisplayQueriesHud.LastWeekRevenueSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("19,800").Or.Contain("8,100"));
        }

        [Test]
        public void this_week_rent_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_980;
            lab.Ledger.ThisWeek.Fines = -360;
            string line = HqDisplayQueriesHud.ThisWeekRentFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("1,980").Or.Contain("360"));
        }

        [Test]
        public void last_week_construction_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_200;
            lab.Ledger.LastWeek.Grants = 4_700;
            string line = HqDisplayQueriesHud.LastWeekConstructionGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("6,200").Or.Contain("4,700"));
        }

        [Test]
        public void this_week_net_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 2_100;
            lab.Ledger.ThisWeek.Salaries = -7_800;
            string line = HqDisplayQueriesHud.ThisWeekNetSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("7,800"));
        }

        [Test]
        public void lifetime_launder_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 25_500;
            lab.Ledger.Lifetime.Power = -125_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("25,500").Or.Contain("125,000"));
        }

        [Test]
        public void last_week_patent_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -11_000;
            lab.Ledger.LastWeek.ChipSales = 6_900;
            string line = HqDisplayQueriesHud.LastWeekPatentLandSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("6,900"));
        }

        [Test]
        public void this_week_power_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -2_600;
            lab.Ledger.ThisWeek.Construction = -3_500;
            string line = HqDisplayQueriesHud.ThisWeekPowerConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("2,600").Or.Contain("3,500"));
        }

        [Test]
        public void last_week_launder_capital_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 2_800;
            lab.Ledger.LastWeek.CapitalIn = 40_000;
            string line = HqDisplayQueriesHud.LastWeekLaunderCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("2,800").Or.Contain("40,000"));
        }

        [Test]
        public void this_week_grey_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_920;
            lab.Ledger.ThisWeek.Salaries = -7_400;
            string line = HqDisplayQueriesHud.ThisWeekGreySalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("1,920").Or.Contain("7,400"));
        }

        [Test]
        public void lifetime_net_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 102_000;
            lab.Ledger.Lifetime.ChipSales = 200_000;
            string line = HqDisplayQueriesHud.LifetimeNetGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("102,000").Or.Contain("200,000"));
        }

        [Test]
        public void last_week_hardware_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -4_700;
            lab.Ledger.LastWeek.Training = -8_900;
            string line = HqDisplayQueriesHud.LastWeekHardwareTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("4,700").Or.Contain("8,900"));
        }

        [Test]
        public void this_week_grant_net_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_100;
            lab.Ledger.ThisWeek.ChipSales = 5_000;
            string line = HqDisplayQueriesHud.ThisWeekGrantNetChipSummaryLine(lab);
            Assert.That(line, Does.Contain("4,100").Or.Contain("5,000"));
        }

        [Test]
        public void last_week_site_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_900;
            lab.Ledger.LastWeek.Power = -3_300;
            string line = HqDisplayQueriesHud.LastWeekSitePowerSummaryLine(lab);
            Assert.That(line, Does.Contain("7,900").Or.Contain("3,300"));
        }

        [Test]
        public void this_week_grant_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 3_800;
            lab.Ledger.ThisWeek.Fines = -440;
            string line = HqDisplayQueriesHud.ThisWeekGrantFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("3,800").Or.Contain("440"));
        }

        [Test]
        public void last_week_room_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_050;
            lab.Ledger.LastWeek.Net = 2_600;
            string line = HqDisplayQueriesHud.LastWeekRoomNetSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("2,600"));
        }

        [Test]
        public void this_week_chip_dev_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipDevelopment = -5_400;
            lab.Ledger.ThisWeek.ChipSales = 4_200;
            string line = HqDisplayQueriesHud.ThisWeekChipDevSummaryLine(lab);
            Assert.That(line, Does.Contain("5,400").Or.Contain("4,200"));
        }

        [Test]
        public void last_week_fines_gpu_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -520;
            lab.Ledger.LastWeek.GpuRent = -2_100;
            string line = HqDisplayQueriesHud.LastWeekFinesGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("520").Or.Contain("2,100"));
        }

        [Test]
        public void lifetime_revenue_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 500_000;
            lab.Ledger.Lifetime.Training = -200_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("500,000").Or.Contain("200,000"));
        }

        [Test]
        public void this_week_hiring_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Hiring = -3_200;
            lab.Ledger.ThisWeek.Salaries = -7_600;
            string line = HqDisplayQueriesHud.ThisWeekHiringSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("3,200").Or.Contain("7,600"));
        }

        [Test]
        public void last_week_grant_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 4_800;
            lab.Ledger.LastWeek.Land = -12_500;
            string line = HqDisplayQueriesHud.LastWeekGrantLandSummaryLine(lab);
            Assert.That(line, Does.Contain("4,800").Or.Contain("12,500"));
        }

        [Test]
        public void this_week_patent_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_650;
            lab.Ledger.ThisWeek.ChipSales = 4_900;
            string line = HqDisplayQueriesHud.ThisWeekPatentGreySummaryLine(lab);
            Assert.That(line, Does.Contain("1,650").Or.Contain("4,900"));
        }

        [Test]
        public void last_week_power_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -3_400;
            lab.Ledger.LastWeek.GpuRent = -2_050;
            string line = HqDisplayQueriesHud.LastWeekPowerRentSummaryLine(lab);
            Assert.That(line, Does.Contain("3,400").Or.Contain("2,050"));
        }

        [Test]
        public void this_week_capital_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 35_000;
            lab.Ledger.ThisWeek.Land = -9_800;
            string line = HqDisplayQueriesHud.ThisWeekCapitalLandSummaryLine(lab);
            Assert.That(line, Does.Contain("35,000").Or.Contain("9,800"));
        }

        [Test]
        public void last_week_grey_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_100;
            lab.Ledger.LastWeek.Power = -2_900;
            string line = HqDisplayQueriesHud.LastWeekGreyPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("2,900"));
        }

        [Test]
        public void this_week_site_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -6_700;
            lab.Ledger.ThisWeek.Revenue = 15_200;
            string line = HqDisplayQueriesHud.ThisWeekSiteRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("6,700").Or.Contain("15,200"));
        }

        [Test]
        public void last_week_net_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 2_400;
            lab.Ledger.LastWeek.ChipSales = 7_100;
            string line = HqDisplayQueriesHud.LastWeekNetChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("7,100"));
        }

        [Test]
        public void lifetime_construction_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Construction = -300_000;
            lab.Ledger.Lifetime.Grants = 105_000;
            string line = HqDisplayQueriesHud.LifetimeConstructionGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("300,000").Or.Contain("105,000"));
        }

        [Test]
        public void this_week_launder_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_100;
            lab.Ledger.ThisWeek.Fines = -380;
            string line = HqDisplayQueriesHud.ThisWeekLaunderFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,100").Or.Contain("380"));
        }

        [Test]
        public void last_week_training_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -8_800;
            lab.Ledger.LastWeek.Power = -3_100;
            string line = HqDisplayQueriesHud.LastWeekTrainingPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("8,800").Or.Contain("3,100"));
        }

        [Test]
        public void lifetime_patent_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 510_000;
            lab.Ledger.Lifetime.ChipSales = 220_000;
            string line = HqDisplayQueriesHud.LifetimePatentRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("510,000").Or.Contain("220,000"));
        }

        [Test]
        public void last_week_salaries_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -7_300;
            lab.Ledger.LastWeek.Construction = -5_900;
            string line = HqDisplayQueriesHud.LastWeekSalariesConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("7,300").Or.Contain("5,900"));
        }

        [Test]
        public void this_week_chip_dev_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipDevelopment = -4_900;
            lab.Ledger.ThisWeek.Grants = 3_600;
            string line = HqDisplayQueriesHud.ThisWeekChipDevGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,900").Or.Contain("3,600"));
        }

        [Test]
        public void last_week_rent_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_180;
            lab.Ledger.LastWeek.Revenue = 18_400;
            string line = HqDisplayQueriesHud.LastWeekRentRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("2,180").Or.Contain("18,400"));
        }

        [Test]
        public void this_week_hardware_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Hardware = -4_200;
            lab.Ledger.ThisWeek.Fines = -420;
            string line = HqDisplayQueriesHud.ThisWeekHardwareFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("420"));
        }

        [Test]
        public void lifetime_room_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.RoomUpkeep = -50_000;
            lab.Ledger.Lifetime.Grants = 108_000;
            string line = HqDisplayQueriesHud.LifetimeRoomGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("50,000").Or.Contain("108,000"));
        }

        [Test]
        public void last_week_chip_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 6_800;
            lab.Ledger.LastWeek.Fines = -490;
            string line = HqDisplayQueriesHud.LastWeekChipFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("6,800").Or.Contain("490"));
        }

        [Test]
        public void this_week_land_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Land = -10_200;
            lab.Ledger.ThisWeek.Training = -9_100;
            string line = HqDisplayQueriesHud.ThisWeekLandTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("10,200").Or.Contain("9,100"));
        }

        [Test]
        public void last_week_grant_gpu_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 4_500;
            lab.Ledger.LastWeek.GpuRent = -2_300;
            string line = HqDisplayQueriesHud.LastWeekGrantGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("4,500").Or.Contain("2,300"));
        }

        [Test]
        public void lifetime_net_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 50_000;
            lab.Ledger.Lifetime.Power = -130_000;
            string line = HqDisplayQueriesHud.LifetimeNetPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("50,000").Or.Contain("130,000"));
        }

        [Test]
        public void this_week_capital_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 42_000;
            lab.Ledger.ThisWeek.Training = -9_300;
            string line = HqDisplayQueriesHud.ThisWeekCapitalTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("42,000").Or.Contain("9,300"));
        }

        [Test]
        public void last_week_site_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_500;
            lab.Ledger.LastWeek.ChipSales = 6_700;
            string line = HqDisplayQueriesHud.LastWeekSiteChipSummaryLine(lab);
            Assert.That(line, Does.Contain("7,500").Or.Contain("6,700"));
        }

        [Test]
        public void lifetime_grey_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 44_000;
            lab.Ledger.Lifetime.RoomUpkeep = -52_000;
            string line = HqDisplayQueriesHud.LifetimeGreyRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("44,000").Or.Contain("52,000"));
        }

        [Test]
        public void last_week_grant_revenue_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 4_900;
            lab.Ledger.LastWeek.Revenue = 17_600;
            string line = HqDisplayQueriesHud.LastWeekGrantRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("4,900").Or.Contain("17,600"));
        }

        [Test]
        public void this_week_launder_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_250;
            lab.Ledger.ThisWeek.Grants = 3_700;
            string line = HqDisplayQueriesHud.ThisWeekLaunderGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,250").Or.Contain("3,700"));
        }

        [Test]
        public void last_week_patent_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 5_100;
            lab.Ledger.LastWeek.Fines = -480;
            string line = HqDisplayQueriesHud.LastWeekPatentGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("5,100").Or.Contain("480"));
        }

        [Test]
        public void this_week_chip_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 5_200;
            lab.Ledger.ThisWeek.RoomUpkeep = -2_100;
            string line = HqDisplayQueriesHud.ThisWeekChipRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("5,200").Or.Contain("2,100"));
        }

        [Test]
        public void last_week_grey_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_200;
            lab.Ledger.LastWeek.Construction = -6_300;
            string line = HqDisplayQueriesHud.LastWeekGreyConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("6,300"));
        }

        [Test]
        public void this_week_net_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 1_900;
            lab.Ledger.ThisWeek.Land = -10_500;
            string line = HqDisplayQueriesHud.ThisWeekNetLandSummaryLine(lab);
            Assert.That(line, Does.Contain("1,900").Or.Contain("10,500"));
        }

        [Test]
        public void last_week_salaries_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -7_500;
            lab.Ledger.LastWeek.GpuRent = -2_120;
            string line = HqDisplayQueriesHud.LastWeekSalariesRentSummaryLine(lab);
            Assert.That(line, Does.Contain("7,500").Or.Contain("2,120"));
        }

        [Test]
        public void this_week_grant_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 3_850;
            lab.Ledger.ThisWeek.Land = -9_600;
            string line = HqDisplayQueriesHud.ThisWeekGrantLandSummaryLine(lab);
            Assert.That(line, Does.Contain("3,850").Or.Contain("9,600"));
        }

        [Test]
        public void lifetime_capital_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 45_000;
            lab.Ledger.Lifetime.Laundered = 26_000;
            string line = HqDisplayQueriesHud.LifetimeCapitalGreySummaryLine(lab);
            Assert.That(line, Does.Contain("45,000").Or.Contain("26,000"));
        }

        [Test]
        public void last_week_room_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_200;
            lab.Ledger.LastWeek.Fines = -510;
            string line = HqDisplayQueriesHud.LastWeekRoomFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("510"));
        }

        [Test]
        public void this_week_gpu_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_050;
            lab.Ledger.ThisWeek.Hardware = -4_100;
            string line = HqDisplayQueriesHud.ThisWeekGpuHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("2,050").Or.Contain("4,100"));
        }

        [Test]
        public void lifetime_fines_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -21_000;
            lab.Ledger.Lifetime.Power = -128_000;
            string line = HqDisplayQueriesHud.LifetimeFinesRentSummaryLine(lab);
            Assert.That(line, Does.Contain("21,000").Or.Contain("128,000"));
        }

        [Test]
        public void last_week_capital_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 38_000;
            lab.Ledger.LastWeek.Construction = -5_800;
            string line = HqDisplayQueriesHud.LastWeekCapitalConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("38,000").Or.Contain("5,800"));
        }

        [Test]
        public void this_week_grey_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_780;
            lab.Ledger.ThisWeek.Land = -10_800;
            string line = HqDisplayQueriesHud.ThisWeekGreyLandSummaryLine(lab);
            Assert.That(line, Does.Contain("1,780").Or.Contain("10,800"));
        }

        [Test]
        public void last_week_land_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -11_200;
            lab.Ledger.LastWeek.Net = 2_700;
            string line = HqDisplayQueriesHud.LastWeekLandNetSummaryLine(lab);
            Assert.That(line, Does.Contain("11,200").Or.Contain("2,700"));
        }

        [Test]
        public void this_week_chip_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipDevelopment = -5_100;
            lab.Ledger.ThisWeek.ChipSales = 4_800;
            string line = HqDisplayQueriesHud.ThisWeekChipPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("5,100").Or.Contain("4,800"));
        }

        [Test]
        public void lifetime_hiring_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Hiring = -90_000;
            lab.Ledger.Lifetime.Salaries = -400_000;
            string line = HqDisplayQueriesHud.LifetimeHiringSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("90,000").Or.Contain("400,000"));
        }

        [Test]
        public void last_week_hardware_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -4_600;
            lab.Ledger.LastWeek.ChipSales = 7_200;
            string line = HqDisplayQueriesHud.LastWeekHardwareChipSummaryLine(lab);
            Assert.That(line, Does.Contain("4,600").Or.Contain("7,200"));
        }

        [Test]
        public void lifetime_training_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -210_000;
            lab.Ledger.Lifetime.Grants = 110_000;
            string line = HqDisplayQueriesHud.LifetimeTrainingGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("210,000").Or.Contain("110,000"));
        }

        [Test]
        public void last_week_net_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 2_500;
            lab.Ledger.LastWeek.Grants = 4_600;
            string line = HqDisplayQueriesHud.LastWeekNetGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,500").Or.Contain("4,600"));
        }

        [Test]
        public void this_week_site_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -6_900;
            lab.Ledger.ThisWeek.Grants = 3_950;
            string line = HqDisplayQueriesHud.ThisWeekSiteGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("6,900").Or.Contain("3,950"));
        }

        [Test]
        public void last_week_launder_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 2_900;
            lab.Ledger.LastWeek.RoomUpkeep = -2_150;
            string line = HqDisplayQueriesHud.LastWeekLaunderRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("2,900").Or.Contain("2,150"));
        }

        [Test]
        public void last_week_hiring_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -3_300;
            lab.Ledger.LastWeek.ChipSales = 6_600;
            string line = HqDisplayQueriesHud.LastWeekHiringChipSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("6,600"));
        }

        [Test]
        public void lifetime_land_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Land = -80_000;
            lab.Ledger.Lifetime.Grants = 112_000;
            string line = HqDisplayQueriesHud.LifetimeLandGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("80,000").Or.Contain("112,000"));
        }

        [Test]
        public void this_week_power_salaries_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -2_700;
            lab.Ledger.ThisWeek.Salaries = -7_650;
            string line = HqDisplayQueriesHud.ThisWeekPowerSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,700").Or.Contain("7,650"));
        }

        [Test]
        public void this_week_capital_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 44_000;
            lab.Ledger.ThisWeek.ChipSales = 4_900;
            string line = HqDisplayQueriesHud.ThisWeekCapitalChipSummaryLine(lab);
            Assert.That(line, Does.Contain("44,000").Or.Contain("4,900"));
        }

        [Test]
        public void lifetime_rent_grey_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GpuRent = -95_000;
            lab.Ledger.Lifetime.GreyRevenue = 46_000;
            string line = HqDisplayQueriesHud.LifetimeRentGreySummaryLine(lab);
            Assert.That(line, Does.Contain("95,000").Or.Contain("46,000"));
        }

        [Test]
        public void last_week_patent_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_000;
            lab.Ledger.LastWeek.ChipDevelopment = -5_200;
            string line = HqDisplayQueriesHud.LastWeekPatentChipSummaryLine(lab);
            Assert.That(line, Does.Contain("7,000").Or.Contain("5,200"));
        }

        [Test]
        public void this_week_grant_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_000;
            lab.Ledger.ThisWeek.RoomUpkeep = -2_080;
            string line = HqDisplayQueriesHud.ThisWeekGrantRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("4,000").Or.Contain("2,080"));
        }

        [Test]
        public void this_week_launder_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_300;
            lab.Ledger.ThisWeek.ChipSales = 5_050;
            string line = HqDisplayQueriesHud.ThisWeekLaunderChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,300").Or.Contain("5,050"));
        }

        [Test]
        public void lifetime_site_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.SiteBuild = -280_000;
            lab.Ledger.Lifetime.Training = -215_000;
            string line = HqDisplayQueriesHud.LifetimeSiteTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("280,000").Or.Contain("215,000"));
        }

        [Test]
        public void last_week_gpu_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_240;
            lab.Ledger.LastWeek.Grants = 4_400;
            string line = HqDisplayQueriesHud.LastWeekGpuGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,240").Or.Contain("4,400"));
        }

        [Test]
        public void last_week_construction_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_100;
            lab.Ledger.LastWeek.Net = 2_550;
            string line = HqDisplayQueriesHud.LastWeekConstructionNetSummaryLine(lab);
            Assert.That(line, Does.Contain("6,100").Or.Contain("2,550"));
        }

        [Test]
        public void this_week_hardware_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Hardware = -4_300;
            lab.Ledger.ThisWeek.Grants = 3_750;
            string line = HqDisplayQueriesHud.ThisWeekHardwareGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,300").Or.Contain("3,750"));
        }

        [Test]
        public void lifetime_fines_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -22_000;
            lab.Ledger.Lifetime.Land = -82_000;
            string line = HqDisplayQueriesHud.LifetimeFinesLandSummaryLine(lab);
            Assert.That(line, Does.Contain("22,000").Or.Contain("82,000"));
        }

        [Test]
        public void last_week_chip_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 6_900;
            lab.Ledger.LastWeek.RoomUpkeep = -2_120;
            string line = HqDisplayQueriesHud.LastWeekChipRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("6,900").Or.Contain("2,120"));
        }

        [Test]
        public void this_week_patent_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Land = -10_400;
            lab.Ledger.ThisWeek.Grants = 3_900;
            string line = HqDisplayQueriesHud.ThisWeekPatentLandSummaryLine(lab);
            Assert.That(line, Does.Contain("10,400").Or.Contain("3,900"));
        }

        [Test]
        public void last_week_rent_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_160;
            lab.Ledger.LastWeek.Land = -11_500;
            string line = HqDisplayQueriesHud.LastWeekRentLandSummaryLine(lab);
            Assert.That(line, Does.Contain("2,160").Or.Contain("11,500"));
        }

        [Test]
        public void this_week_room_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.RoomUpkeep = -2_180;
            lab.Ledger.ThisWeek.Training = -9_200;
            string line = HqDisplayQueriesHud.ThisWeekRoomTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("2,180").Or.Contain("9,200"));
        }

        [Test]
        public void lifetime_chip_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 225_000;
            lab.Ledger.Lifetime.Grants = 115_000;
            string line = HqDisplayQueriesHud.LifetimeChipGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("225,000").Or.Contain("115,000"));
        }

        [Test]
        public void last_week_power_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -3_450;
            lab.Ledger.LastWeek.Grants = 4_700;
            string line = HqDisplayQueriesHud.LastWeekPowerGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("3,450").Or.Contain("4,700"));
        }

        [Test]
        public void this_week_rent_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_020;
            lab.Ledger.ThisWeek.Land = -10_600;
            string line = HqDisplayQueriesHud.ThisWeekRentLandSummaryLine(lab);
            Assert.That(line, Does.Contain("2,020").Or.Contain("10,600"));
        }

        [Test]
        public void last_week_launder_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 3_000;
            lab.Ledger.LastWeek.Training = -8_700;
            string line = HqDisplayQueriesHud.LastWeekLaunderTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("3,000").Or.Contain("8,700"));
        }

        [Test]
        public void lifetime_net_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 55_000;
            lab.Ledger.Lifetime.RoomUpkeep = -55_000;
            string line = HqDisplayQueriesHud.LifetimeNetRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("55,000").Or.Contain("55,000"));
        }

        [Test]
        public void lifetime_power_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -135_000;
            lab.Ledger.Lifetime.Grants = 118_000;
            string line = HqDisplayQueriesHud.LifetimePowerGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("135,000").Or.Contain("118,000"));
        }

        [Test]
        public void last_week_grey_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_250;
            lab.Ledger.LastWeek.Grants = 4_800;
            string line = HqDisplayQueriesHud.LastWeekGreyGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,250").Or.Contain("4,800"));
        }

        [Test]
        public void this_week_fines_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -450;
            lab.Ledger.ThisWeek.Land = -10_300;
            string line = HqDisplayQueriesHud.ThisWeekFinesLandSummaryLine(lab);
            Assert.That(line, Does.Contain("450").Or.Contain("10,300"));
        }

        [Test]
        public void lifetime_hardware_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Hardware = -150_000;
            lab.Ledger.Lifetime.GpuRent = -98_000;
            string line = HqDisplayQueriesHud.LifetimeHardwareRentSummaryLine(lab);
            Assert.That(line, Does.Contain("150,000").Or.Contain("98,000"));
        }

        [Test]
        public void last_week_capital_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 41_000;
            lab.Ledger.LastWeek.Power = -3_500;
            string line = HqDisplayQueriesHud.LastWeekCapitalPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("41,000").Or.Contain("3,500"));
        }

        [Test]
        public void this_week_patent_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -9_400;
            lab.Ledger.ThisWeek.Grants = 3_850;
            string line = HqDisplayQueriesHud.ThisWeekPatentTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("9,400").Or.Contain("3,850"));
        }

        [Test]
        public void this_week_launder_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_400;
            lab.Ledger.ThisWeek.Power = -2_650;
            string line = HqDisplayQueriesHud.ThisWeekLaunderPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("2,650"));
        }

        [Test]
        public void lifetime_grey_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 47_500;
            lab.Ledger.Lifetime.ChipSales = 228_000;
            string line = HqDisplayQueriesHud.LifetimeGreyChipSummaryLine(lab);
            Assert.That(line, Does.Contain("47,500").Or.Contain("228,000"));
        }

        [Test]
        public void last_week_rent_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_200;
            lab.Ledger.LastWeek.Construction = -6_000;
            string line = HqDisplayQueriesHud.LastWeekRentConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("6,000"));
        }

        [Test]
        public void lifetime_salaries_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Salaries = -410_000;
            lab.Ledger.Lifetime.Power = -138_000;
            string line = HqDisplayQueriesHud.LifetimeSalariesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("410,000").Or.Contain("138,000"));
        }

        [Test]
        public void last_week_fines_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -530;
            lab.Ledger.LastWeek.RoomUpkeep = -2_180;
            string line = HqDisplayQueriesHud.LastWeekFinesRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("530").Or.Contain("2,180"));
        }

        [Test]
        public void last_week_land_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -11_800;
            lab.Ledger.LastWeek.ChipSales = 7_300;
            string line = HqDisplayQueriesHud.LastWeekLandChipSummaryLine(lab);
            Assert.That(line, Does.Contain("11,800").Or.Contain("7,300"));
        }

        [Test]
        public void this_week_hardware_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Hardware = -4_250;
            lab.Ledger.ThisWeek.RoomUpkeep = -2_100;
            string line = HqDisplayQueriesHud.ThisWeekHardwareRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("4,250").Or.Contain("2,100"));
        }

        [Test]
        public void lifetime_patent_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 120_000;
            lab.Ledger.Lifetime.Revenue = 520_000;
            string line = HqDisplayQueriesHud.LifetimePatentGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("120,000").Or.Contain("520,000"));
        }

        [Test]
        public void last_week_grey_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_300;
            lab.Ledger.LastWeek.ChipSales = 6_950;
            string line = HqDisplayQueriesHud.LastWeekGreyChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,300").Or.Contain("6,950"));
        }

        [Test]
        public void this_week_capital_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 45_500;
            lab.Ledger.ThisWeek.Grants = 4_050;
            string line = HqDisplayQueriesHud.ThisWeekCapitalGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("45,500").Or.Contain("4,050"));
        }

        [Test]
        public void last_week_training_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -8_950;
            lab.Ledger.LastWeek.ChipSales = 7_050;
            string line = HqDisplayQueriesHud.LastWeekTrainingChipSummaryLine(lab);
            Assert.That(line, Does.Contain("8,950").Or.Contain("7,050"));
        }

        [Test]
        public void this_week_site_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -7_100;
            lab.Ledger.ThisWeek.Fines = -470;
            string line = HqDisplayQueriesHud.ThisWeekSiteFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("7,100").Or.Contain("470"));
        }

        [Test]
        public void this_week_grant_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_150;
            lab.Ledger.ThisWeek.ChipDevelopment = -5_300;
            string line = HqDisplayQueriesHud.ThisWeekGrantPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("4,150").Or.Contain("5,300"));
        }

        [Test]
        public void last_week_launder_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 3_100;
            lab.Ledger.LastWeek.Grants = 4_750;
            string line = HqDisplayQueriesHud.LastWeekLaunderGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("3,100").Or.Contain("4,750"));
        }

        [Test]
        public void this_week_chip_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 5_200;
            lab.Ledger.ThisWeek.Power = -2_720;
            string line = HqDisplayQueriesHud.ThisWeekChipPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("5,200").Or.Contain("2,720"));
        }

        [Test]
        public void lifetime_construction_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Construction = -310_000;
            lab.Ledger.Lifetime.Power = -140_000;
            string line = HqDisplayQueriesHud.LifetimeConstructionPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("310,000").Or.Contain("140,000"));
        }

        [Test]
        public void last_week_hiring_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -3_450;
            lab.Ledger.LastWeek.Grants = 4_850;
            string line = HqDisplayQueriesHud.LastWeekHiringGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("3,450").Or.Contain("4,850"));
        }

        [Test]
        public void this_week_grey_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_850;
            lab.Ledger.ThisWeek.GpuRent = -2_000;
            string line = HqDisplayQueriesHud.ThisWeekGreyRentSummaryLine(lab);
            Assert.That(line, Does.Contain("1,850").Or.Contain("2,000"));
        }

        [Test]
        public void last_week_site_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_200;
            lab.Ledger.LastWeek.Grants = 4_950;
            string line = HqDisplayQueriesHud.LastWeekSiteGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("7,200").Or.Contain("4,950"));
        }

        [Test]
        public void this_week_launder_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_500;
            lab.Ledger.ThisWeek.Land = -10_700;
            string line = HqDisplayQueriesHud.ThisWeekLaunderLandSummaryLine(lab);
            Assert.That(line, Does.Contain("2,500").Or.Contain("10,700"));
        }

        [Test]
        public void lifetime_rent_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GpuRent = -102_000;
            lab.Ledger.Lifetime.Grants = 122_000;
            string line = HqDisplayQueriesHud.LifetimeRentGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("102,000").Or.Contain("122,000"));
        }

        [Test]
        public void last_week_capital_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 39_500;
            lab.Ledger.LastWeek.ChipSales = 7_100;
            string line = HqDisplayQueriesHud.LastWeekCapitalChipSummaryLine(lab);
            Assert.That(line, Does.Contain("39,500").Or.Contain("7,100"));
        }

        [Test]
        public void lifetime_grey_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 48_000;
            lab.Ledger.Lifetime.Land = -85_000;
            string line = HqDisplayQueriesHud.LifetimeGreyLandSummaryLine(lab);
            Assert.That(line, Does.Contain("48,000").Or.Contain("85,000"));
        }

        [Test]
        public void this_week_construction_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -5_900;
            lab.Ledger.ThisWeek.Land = -10_900;
            string line = HqDisplayQueriesHud.ThisWeekConstructionLandSummaryLine(lab);
            Assert.That(line, Does.Contain("5,900").Or.Contain("10,900"));
        }

        [Test]
        public void last_week_patent_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_190;
            lab.Ledger.LastWeek.Fines = -540;
            string line = HqDisplayQueriesHud.LastWeekPatentRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("2,190").Or.Contain("540"));
        }

        [Test]
        public void this_week_grant_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_200;
            lab.Ledger.ThisWeek.Hardware = -4_350;
            string line = HqDisplayQueriesHud.ThisWeekGrantHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("4,350"));
        }

        [Test]
        public void last_week_grey_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_350;
            lab.Ledger.LastWeek.GpuRent = -2_280;
            string line = HqDisplayQueriesHud.LastWeekGreyRentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,350").Or.Contain("2,280"));
        }

        [Test]
        public void this_week_chip_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 5_150;
            lab.Ledger.ThisWeek.Land = -10_800;
            string line = HqDisplayQueriesHud.ThisWeekChipLandSummaryLine(lab);
            Assert.That(line, Does.Contain("5,150").Or.Contain("10,800"));
        }

        [Test]
        public void this_week_site_net_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -7_000;
            lab.Ledger.ThisWeek.Net = 2_050;
            string line = HqDisplayQueriesHud.ThisWeekSiteNetSummaryLine(lab);
            Assert.That(line, Does.Contain("7,000").Or.Contain("2,050"));
        }

        [Test]
        public void last_week_grant_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 4_900;
            lab.Ledger.LastWeek.ChipSales = 7_150;
            string line = HqDisplayQueriesHud.LastWeekGrantChipSummaryLine(lab);
            Assert.That(line, Does.Contain("4,900").Or.Contain("7,150"));
        }

        [Test]
        public void lifetime_launder_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 28_000;
            lab.Ledger.Lifetime.Grants = 123_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("28,000").Or.Contain("123,000"));
        }

        [Test]
        public void this_week_rent_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_010;
            lab.Ledger.ThisWeek.Training = -9_350;
            string line = HqDisplayQueriesHud.ThisWeekRentTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("2,010").Or.Contain("9,350"));
        }

        [Test]
        public void lifetime_room_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.RoomUpkeep = -58_000;
            lab.Ledger.Lifetime.ChipSales = 230_000;
            string line = HqDisplayQueriesHud.LifetimeRoomChipSummaryLine(lab);
            Assert.That(line, Does.Contain("58,000").Or.Contain("230,000"));
        }

        [Test]
        public void last_week_fines_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -560;
            lab.Ledger.LastWeek.ChipSales = 7_050;
            string line = HqDisplayQueriesHud.LastWeekFinesChipSummaryLine(lab);
            Assert.That(line, Does.Contain("560").Or.Contain("7,050"));
        }

        [Test]
        public void this_week_capital_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 46_000;
            lab.Ledger.ThisWeek.Power = -2_780;
            string line = HqDisplayQueriesHud.ThisWeekCapitalPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("46,000").Or.Contain("2,780"));
        }

        [Test]
        public void last_week_site_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_400;
            lab.Ledger.LastWeek.Land = -11_600;
            string line = HqDisplayQueriesHud.LastWeekSiteLandSummaryLine(lab);
            Assert.That(line, Does.Contain("7,400").Or.Contain("11,600"));
        }

        [Test]
        public void last_week_launder_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 3_150;
            lab.Ledger.LastWeek.Land = -11_900;
            string line = HqDisplayQueriesHud.LastWeekLaunderLandSummaryLine(lab);
            Assert.That(line, Does.Contain("3,150").Or.Contain("11,900"));
        }

        [Test]
        public void this_week_grant_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_300;
            lab.Ledger.ThisWeek.SiteBuild = -6_950;
            string line = HqDisplayQueriesHud.ThisWeekGrantSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("4,300").Or.Contain("6,950"));
        }

        [Test]
        public void lifetime_chip_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 232_000;
            lab.Ledger.Lifetime.Land = -86_000;
            string line = HqDisplayQueriesHud.LifetimeChipLandSummaryLine(lab);
            Assert.That(line, Does.Contain("232,000").Or.Contain("86,000"));
        }

        [Test]
        public void last_week_rent_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_260;
            lab.Ledger.LastWeek.Training = -8_850;
            string line = HqDisplayQueriesHud.LastWeekRentTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("2,260").Or.Contain("8,850"));
        }

        [Test]
        public void lifetime_hiring_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Hiring = -95_000;
            lab.Ledger.Lifetime.Power = -142_000;
            string line = HqDisplayQueriesHud.LifetimeHiringPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("95,000").Or.Contain("142,000"));
        }

        [Test]
        public void this_week_net_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 2_150;
            lab.Ledger.ThisWeek.RoomUpkeep = -2_200;
            string line = HqDisplayQueriesHud.ThisWeekNetRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("2,150").Or.Contain("2,200"));
        }

        [Test]
        public void this_week_launder_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_600;
            lab.Ledger.ThisWeek.RoomUpkeep = -2_220;
            string line = HqDisplayQueriesHud.ThisWeekLaunderRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("2,600").Or.Contain("2,220"));
        }

        [Test]
        public void lifetime_rent_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GpuRent = -104_000;
            lab.Ledger.Lifetime.Land = -88_000;
            string line = HqDisplayQueriesHud.LifetimeRentLandSummaryLine(lab);
            Assert.That(line, Does.Contain("104,000").Or.Contain("88,000"));
        }

        [Test]
        public void last_week_salaries_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -7_550;
            lab.Ledger.LastWeek.Grants = 5_050;
            string line = HqDisplayQueriesHud.LastWeekSalariesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("7,550").Or.Contain("5,050"));
        }

        [Test]
        public void this_week_site_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -7_150;
            lab.Ledger.ThisWeek.ChipSales = 5_300;
            string line = HqDisplayQueriesHud.ThisWeekSiteChipSummaryLine(lab);
            Assert.That(line, Does.Contain("7,150").Or.Contain("5,300"));
        }

        [Test]
        public void this_week_capital_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 47_500;
            lab.Ledger.ThisWeek.Construction = -6_050;
            string line = HqDisplayQueriesHud.ThisWeekCapitalConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("47,500").Or.Contain("6,050"));
        }

        [Test]
        public void lifetime_grant_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 125_000;
            lab.Ledger.Lifetime.Land = -89_000;
            string line = HqDisplayQueriesHud.LifetimeGrantLandSummaryLine(lab);
            Assert.That(line, Does.Contain("125,000").Or.Contain("89,000"));
        }

        [Test]
        public void last_week_room_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_210;
            lab.Ledger.LastWeek.Grants = 5_150;
            string line = HqDisplayQueriesHud.LastWeekRoomGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,210").Or.Contain("5,150"));
        }

        [Test]
        public void this_week_patent_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_030;
            lab.Ledger.ThisWeek.Grants = 4_250;
            string line = HqDisplayQueriesHud.ThisWeekPatentRentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,030").Or.Contain("4,250"));
        }

        [Test]
        public void this_week_fines_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -490;
            lab.Ledger.ThisWeek.GpuRent = -1_990;
            string line = HqDisplayQueriesHud.ThisWeekFinesRentSummaryLine(lab);
            Assert.That(line, Does.Contain("490").Or.Contain("1,990"));
        }

        [Test]
        public void lifetime_chip_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 235_000;
            lab.Ledger.Lifetime.RoomUpkeep = -60_000;
            string line = HqDisplayQueriesHud.LifetimeChipRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("235,000").Or.Contain("60,000"));
        }

        [Test]
        public void last_week_construction_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_050;
            lab.Ledger.LastWeek.Fines = -570;
            string line = HqDisplayQueriesHud.LastWeekConstructionFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("6,050").Or.Contain("570"));
        }

        [Test]
        public void lifetime_salaries_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Salaries = -420_000;
            lab.Ledger.Lifetime.Grants = 126_000;
            string line = HqDisplayQueriesHud.LifetimeSalariesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("420,000").Or.Contain("126,000"));
        }

        [Test]
        public void last_week_rent_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_290;
            lab.Ledger.LastWeek.Grants = 5_200;
            string line = HqDisplayQueriesHud.LastWeekRentGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,290").Or.Contain("5,200"));
        }

        [Test]
        public void this_week_land_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Land = -11_100;
            lab.Ledger.ThisWeek.ChipSales = 5_250;
            string line = HqDisplayQueriesHud.ThisWeekLandChipSummaryLine(lab);
            Assert.That(line, Does.Contain("11,100").Or.Contain("5,250"));
        }

        [Test]
        public void last_week_grey_site_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_400;
            lab.Ledger.LastWeek.SiteBuild = -7_300;
            string line = HqDisplayQueriesHud.LastWeekGreySiteSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("7,300"));
        }

        [Test]
        public void this_week_hardware_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Hardware = -4_280;
            lab.Ledger.ThisWeek.Land = -10_950;
            string line = HqDisplayQueriesHud.ThisWeekHardwareLandSummaryLine(lab);
            Assert.That(line, Does.Contain("4,280").Or.Contain("10,950"));
        }

        [Test]
        public void last_week_fines_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -580;
            lab.Ledger.LastWeek.Land = -11_700;
            string line = HqDisplayQueriesHud.LastWeekFinesLandSummaryLine(lab);
            Assert.That(line, Does.Contain("580").Or.Contain("11,700"));
        }

        [Test]
        public void last_week_capital_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 42_500;
            lab.Ledger.LastWeek.Grants = 5_300;
            string line = HqDisplayQueriesHud.LastWeekCapitalGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("42,500").Or.Contain("5,300"));
        }

        [Test]
        public void this_week_room_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.RoomUpkeep = -2_240;
            lab.Ledger.ThisWeek.Grants = 4_350;
            string line = HqDisplayQueriesHud.ThisWeekRoomGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("2,240").Or.Contain("4,350"));
        }

        [Test]
        public void this_week_construction_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -5_950;
            lab.Ledger.ThisWeek.GpuRent = -2_040;
            string line = HqDisplayQueriesHud.ThisWeekConstructionRentSummaryLine(lab);
            Assert.That(line, Does.Contain("5,950").Or.Contain("2,040"));
        }

        [Test]
        public void this_week_salaries_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_720;
            lab.Ledger.ThisWeek.Land = -11_050;
            string line = HqDisplayQueriesHud.ThisWeekSalariesLandSummaryLine(lab);
            Assert.That(line, Does.Contain("7,720").Or.Contain("11,050"));
        }

        [Test]
        public void last_week_net_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 2_650;
            lab.Ledger.LastWeek.RoomUpkeep = -2_230;
            string line = HqDisplayQueriesHud.LastWeekNetRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("2,650").Or.Contain("2,230"));
        }

        [Test]
        public void lifetime_grant_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 127_000;
            lab.Ledger.Lifetime.ChipSales = 238_000;
            string line = HqDisplayQueriesHud.LifetimeGrantChipSummaryLine(lab);
            Assert.That(line, Does.Contain("127,000").Or.Contain("238,000"));
        }

        [Test]
        public void this_week_grant_launder_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 4_400;
            lab.Ledger.ThisWeek.Laundered = 2_700;
            string line = HqDisplayQueriesHud.ThisWeekGrantLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("4,400").Or.Contain("2,700"));
        }

        [Test]
        public void this_week_site_training_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.SiteBuild = -7_050;
                        lab.Ledger.ThisWeek.Training = -9_380;
            string line = HqDisplayQueriesHud.ThisWeekSiteTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("7,050").Or.Contain("9,380"));
        }

        [Test]
        public void lifetime_rent_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GpuRent = -106_000;
                        lab.Ledger.Lifetime.ChipSales = 240_000;
            string line = HqDisplayQueriesHud.LifetimeRentChipSummaryLine(lab);
            Assert.That(line, Does.Contain("106,000").Or.Contain("240,000"));
        }

        [Test]
        public void lifetime_grey_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 49_000;
                        lab.Ledger.Lifetime.Grants = 128_000;
            string line = HqDisplayQueriesHud.LifetimeGreyGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("49,000").Or.Contain("128,000"));
        }

        [Test]
        public void last_week_chip_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_180;
                        lab.Ledger.LastWeek.GpuRent = -2_310;
            string line = HqDisplayQueriesHud.LastWeekChipRentSummaryLine(lab);
            Assert.That(line, Does.Contain("7,180").Or.Contain("2,310"));
        }

        [Test]
        public void this_week_grey_patent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 1_900;
                        lab.Ledger.ThisWeek.Fines = -500;
            string line = HqDisplayQueriesHud.ThisWeekGreyPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("1,900").Or.Contain("500"));
        }

        [Test]
        public void lifetime_fines_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -23_500;
                        lab.Ledger.Lifetime.Grants = 129_000;
            string line = HqDisplayQueriesHud.LifetimeFinesGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("23,500").Or.Contain("129,000"));
        }

        [Test]
        public void last_week_land_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -11_950;
                        lab.Ledger.LastWeek.Construction = -6_100;
            string line = HqDisplayQueriesHud.LastWeekLandConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("11,950").Or.Contain("6,100"));
        }

        [Test]
        public void this_week_rent_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -2_060;
                        lab.Ledger.ThisWeek.Power = -2_750;
            string line = HqDisplayQueriesHud.ThisWeekRentPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("2,060").Or.Contain("2,750"));
        }

        [Test]
        public void last_week_grey_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_450;
                        lab.Ledger.LastWeek.Fines = -590;
            string line = HqDisplayQueriesHud.LastWeekGreyFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("2,450").Or.Contain("590"));
        }

        [Test]
        public void lifetime_power_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -145_000;
                        lab.Ledger.Lifetime.Land = -91_000;
            string line = HqDisplayQueriesHud.LifetimePowerLandSummaryLine(lab);
            Assert.That(line, Does.Contain("145,000").Or.Contain("91,000"));
        }

        [Test]
        public void last_week_hardware_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -4_550;
                        lab.Ledger.LastWeek.Grants = 5_350;
            string line = HqDisplayQueriesHud.LastWeekHardwareGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("4,550").Or.Contain("5,350"));
        }

        [Test]
        public void last_week_site_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_350;
                        lab.Ledger.LastWeek.GpuRent = -2_320;
            string line = HqDisplayQueriesHud.LastWeekSiteRentSummaryLine(lab);
            Assert.That(line, Does.Contain("7,350").Or.Contain("2,320"));
        }

        [Test]
        public void lifetime_construction_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Construction = -320_000;
                        lab.Ledger.Lifetime.ChipSales = 242_000;
            string line = HqDisplayQueriesHud.LifetimeConstructionChipSummaryLine(lab);
            Assert.That(line, Does.Contain("320,000").Or.Contain("242,000"));
        }

        [Test]
        public void last_week_grant_power_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 5_400;
                        lab.Ledger.LastWeek.Power = -3_520;
            string line = HqDisplayQueriesHud.LastWeekGrantPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("5,400").Or.Contain("3,520"));
        }

        [Test]
        public void this_week_land_grant_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Land = -11_200;
                        lab.Ledger.ThisWeek.Grants = 4_450;
            string line = HqDisplayQueriesHud.ThisWeekLandGrantSummaryLine(lab);
            Assert.That(line, Does.Contain("11,200").Or.Contain("4,450"));
        }

        [Test]
        public void this_week_salaries_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -7_740;
                        lab.Ledger.ThisWeek.GpuRent = -2_080;
            string line = HqDisplayQueriesHud.ThisWeekSalariesRentSummaryLine(lab);
            Assert.That(line, Does.Contain("7,740").Or.Contain("2,080"));
        }

        [Test]
        public void lifetime_launder_land_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 29_500;
                        lab.Ledger.Lifetime.Land = -92_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderLandSummaryLine(lab);
            Assert.That(line, Does.Contain("29,500").Or.Contain("92,000"));
        }

        [Test]
        public void last_week_site_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_450;
                        lab.Ledger.LastWeek.Fines = -600;
            string line = HqDisplayQueriesHud.LastWeekSiteFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("7,450").Or.Contain("600"));
        }

        [Test]
        public void this_week_capital_rent_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.CapitalIn = 48_000;
                        lab.Ledger.ThisWeek.GpuRent = -2_070;
            string line = HqDisplayQueriesHud.ThisWeekCapitalRentSummaryLine(lab);
            Assert.That(line, Does.Contain("48,000").Or.Contain("2,070"));
        }

        [Test]
        public void lifetime_grant_room_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 130_000;
                        lab.Ledger.Lifetime.RoomUpkeep = -62_000;
            string line = HqDisplayQueriesHud.LifetimeGrantRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("130,000").Or.Contain("62,000"));
        }

        [Test]
        public void last_week_rent_chip_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -2_340;
                        lab.Ledger.LastWeek.ChipSales = 7_220;
            string line = HqDisplayQueriesHud.LastWeekRentChipSummaryLine(lab);
            Assert.That(line, Does.Contain("2,340").Or.Contain("7,220"));
        }

        [Test]
        public void lifetime_launder_fines_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 30_000;
                        lab.Ledger.Lifetime.Fines = -24_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("30,000").Or.Contain("24,000"));
        }

        [Test]
        public void this_week_launder_construction_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 2_800;
                        lab.Ledger.ThisWeek.Construction = -6_000;
            string line = HqDisplayQueriesHud.ThisWeekLaunderConstructionSummaryLine(lab);
            Assert.That(line, Does.Contain("2,800").Or.Contain("6,000"));
        }

        [Test]
        public void this_week_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -890;
            lab.Ledger.ThisWeek.Grants = 5_230;
            string line = HqDisplayQueriesHud.ThisWeekFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("890").Or.Contain("5,230"));
        }

        [Test]
        public void last_week_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -900;
            lab.Ledger.LastWeek.Revenue = 18_360;
            string line = HqDisplayQueriesHud.LastWeekFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("900").Or.Contain("18,360"));
        }

        [Test]
        public void lifetime_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -28_200;
            lab.Ledger.Lifetime.Net = 118_000;
            string line = HqDisplayQueriesHud.LifetimeFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("28,200").Or.Contain("118,000"));
        }

        [Test]
        public void this_week_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_240;
            lab.Ledger.ThisWeek.Net = 3_410;
            string line = HqDisplayQueriesHud.ThisWeekGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("5,240").Or.Contain("3,410"));
        }

        [Test]
        public void last_week_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 6_180;
            lab.Ledger.LastWeek.Fines = -910;
            string line = HqDisplayQueriesHud.LastWeekGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("6,180").Or.Contain("910"));
        }

        [Test]
        public void lifetime_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 185_000;
            lab.Ledger.Lifetime.Revenue = 447_000;
            string line = HqDisplayQueriesHud.LifetimeGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("185,000").Or.Contain("447,000"));
        }

        [Test]
        public void this_week_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 8_200;
            lab.Ledger.ThisWeek.Grants = 5_250;
            string line = HqDisplayQueriesHud.ThisWeekChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("8,200").Or.Contain("5,250"));
        }

        [Test]
        public void last_week_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_890;
            lab.Ledger.LastWeek.Revenue = 18_370;
            string line = HqDisplayQueriesHud.LastWeekChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("7,890").Or.Contain("18,370"));
        }

        [Test]
        public void lifetime_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 112_000;
            lab.Ledger.Lifetime.Net = 118_500;
            string line = HqDisplayQueriesHud.LifetimeChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("112,000").Or.Contain("118,500"));
        }

        [Test]
        public void this_week_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipDevelopment = -4_100;
            lab.Ledger.ThisWeek.Fines = -920;
            string line = HqDisplayQueriesHud.ThisWeekDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("4,100").Or.Contain("920"));
        }

        [Test]
        public void last_week_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipDevelopment = -3_980;
            lab.Ledger.LastWeek.Grants = 6_190;
            string line = HqDisplayQueriesHud.LastWeekDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,980").Or.Contain("6,190"));
        }

        [Test]
        public void lifetime_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipDevelopment = -92_000;
            lab.Ledger.Lifetime.Grants = 186_000;
            string line = HqDisplayQueriesHud.LifetimeDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("92,000").Or.Contain("186,000"));
        }

        [Test]
        public void this_week_gpu_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_200;
            lab.Ledger.ThisWeek.Revenue = 18_820;
            string line = HqDisplayQueriesHud.ThisWeekGpuSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,200").Or.Contain("18,820"));
        }

        [Test]
        public void last_week_gpu_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -1_180;
            lab.Ledger.LastWeek.Net = 3_420;
            string line = HqDisplayQueriesHud.LastWeekGpuSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,180").Or.Contain("3,420"));
        }

        [Test]
        public void lifetime_gpu_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -168_000;
            lab.Ledger.Lifetime.Revenue = 448_000;
            string line = HqDisplayQueriesHud.LifetimeGpuSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("168,000").Or.Contain("448,000"));
        }

        [Test]
        public void this_week_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 18_830;
            lab.Ledger.ThisWeek.Grants = 5_260;
            string line = HqDisplayQueriesHud.ThisWeekRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("18,830").Or.Contain("5,260"));
        }

        [Test]
        public void last_week_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 18_380;
            lab.Ledger.LastWeek.Fines = -930;
            string line = HqDisplayQueriesHud.LastWeekRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("18,380").Or.Contain("930"));
        }

        [Test]
        public void lifetime_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 449_000;
            lab.Ledger.Lifetime.Net = 119_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("449,000").Or.Contain("119,000"));
        }

        [Test]
        public void this_week_grant_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_270;
            lab.Ledger.ThisWeek.Fines = -940;
            string line = HqDisplayQueriesHud.ThisWeekGrantFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("5,270").Or.Contain("940"));
        }

        [Test]
        public void last_week_grant_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 6_200;
            lab.Ledger.LastWeek.Fines = -950;
            string line = HqDisplayQueriesHud.LastWeekGrantFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("6,200").Or.Contain("950"));
        }

        [Test]
        public void lifetime_grant_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 187_000;
            lab.Ledger.Lifetime.Fines = -28_300;
            string line = HqDisplayQueriesHud.LifetimeGrantFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("187,000").Or.Contain("28,300"));
        }

        [Test]
        public void this_week_chip_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 8_210;
            lab.Ledger.ThisWeek.ChipDevelopment = -4_110;
            string line = HqDisplayQueriesHud.ThisWeekChipDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("8,210").Or.Contain("4,110"));
        }

        [Test]
        public void last_week_chip_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_900;
            lab.Ledger.LastWeek.ChipDevelopment = -3_990;
            string line = HqDisplayQueriesHud.LastWeekChipDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("7,900").Or.Contain("3,990"));
        }

        [Test]
        public void lifetime_chip_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 113_000;
            lab.Ledger.Lifetime.ChipDevelopment = -92_500;
            string line = HqDisplayQueriesHud.LifetimeChipDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("113,000").Or.Contain("92,500"));
        }

        [Test]
        public void this_week_hardware_site_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -2_200;
            lab.Ledger.ThisWeek.Revenue = 18_840;
            string line = HqDisplayQueriesHud.ThisWeekHardwareSiteSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("18,840"));
        }

        [Test]
        public void last_week_hardware_site_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -2_180;
            lab.Ledger.LastWeek.Net = 3_430;
            string line = HqDisplayQueriesHud.LastWeekHardwareSiteSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,180").Or.Contain("3,430"));
        }

        [Test]
        public void lifetime_hardware_site_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Construction = -85_500;
            lab.Ledger.Lifetime.Revenue = 450_000;
            string line = HqDisplayQueriesHud.LifetimeHardwareSiteSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("85,500").Or.Contain("450,000"));
        }

        [Test]
        public void this_week_land_room_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.RoomUpkeep = -1_100;
            lab.Ledger.ThisWeek.Grants = 5_280;
            string line = HqDisplayQueriesHud.ThisWeekLandRoomSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,100").Or.Contain("5,280"));
        }

        [Test]
        public void last_week_land_room_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -1_080;
            lab.Ledger.LastWeek.Revenue = 18_390;
            string line = HqDisplayQueriesHud.LastWeekLandRoomSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,080").Or.Contain("18,390"));
        }

        [Test]
        public void lifetime_land_room_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.RoomUpkeep = -41_000;
            lab.Ledger.Lifetime.Net = 119_500;
            string line = HqDisplayQueriesHud.LifetimeLandRoomSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("41,000").Or.Contain("119,500"));
        }

        [Test]
        public void this_week_rent_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GpuRent = -1_210;
            lab.Ledger.ThisWeek.Training = -900;
            string line = HqDisplayQueriesHud.ThisWeekRentTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,210").Or.Contain("900"));
        }

        [Test]
        public void last_week_rent_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuRent = -1_190;
            lab.Ledger.LastWeek.Training = -880;
            string line = HqDisplayQueriesHud.LastWeekRentTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("1,190").Or.Contain("880"));
        }

        [Test]
        public void lifetime_rent_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -52_000;
            lab.Ledger.Lifetime.Power = -169_000;
            string line = HqDisplayQueriesHud.LifetimeRentTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("52,000").Or.Contain("169,000"));
        }

        [Test]
        public void this_week_power_construction_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -3_010;
            lab.Ledger.ThisWeek.Construction = -2_210;
            string line = HqDisplayQueriesHud.ThisWeekPowerConstructionSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,010").Or.Contain("2,210"));
        }

        [Test]
        public void last_week_power_construction_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Power = -2_460;
            lab.Ledger.LastWeek.Construction = -2_190;
            string line = HqDisplayQueriesHud.LastWeekPowerConstructionSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,460").Or.Contain("2,190"));
        }

        [Test]
        public void lifetime_power_construction_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -170_000;
            lab.Ledger.Lifetime.Construction = -86_000;
            string line = HqDisplayQueriesHud.LifetimePowerConstructionSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("170,000").Or.Contain("86,000"));
        }

        [Test]
        public void this_week_salaries_hiring_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -12_400;
            lab.Ledger.ThisWeek.Hiring = -600;
            string line = HqDisplayQueriesHud.ThisWeekSalariesHiringSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("12,400").Or.Contain("600"));
        }

        [Test]
        public void last_week_salaries_hiring_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -12_300;
            lab.Ledger.LastWeek.Hiring = -590;
            string line = HqDisplayQueriesHud.LastWeekSalariesHiringSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("12,300").Or.Contain("590"));
        }

        [Test]
        public void lifetime_salaries_hiring_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Salaries = -890_000;
            lab.Ledger.Lifetime.Hiring = -18_000;
            string line = HqDisplayQueriesHud.LifetimeSalariesHiringSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("890,000").Or.Contain("18,000"));
        }

        [Test]
        public void this_week_launder_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Laundered = 3_500;
            lab.Ledger.ThisWeek.Grants = 5_290;
            string line = HqDisplayQueriesHud.ThisWeekLaunderCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,500").Or.Contain("5,290"));
        }

        [Test]
        public void last_week_launder_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Laundered = 3_120;
            lab.Ledger.LastWeek.Net = 3_440;
            string line = HqDisplayQueriesHud.LastWeekLaunderCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,120").Or.Contain("3,440"));
        }

        [Test]
        public void lifetime_launder_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Laundered = 33_100;
            lab.Ledger.Lifetime.Grants = 188_000;
            string line = HqDisplayQueriesHud.LifetimeLaunderCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("33,100").Or.Contain("188,000"));
        }

        [Test]
        public void this_week_patent_upkeep_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -960;
            lab.Ledger.ThisWeek.Power = -3_020;
            string line = HqDisplayQueriesHud.ThisWeekPatentUpkeepSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("960").Or.Contain("3,020"));
        }

        [Test]
        public void last_week_patent_upkeep_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.PatentLicense = 4_200;
            lab.Ledger.LastWeek.Power = -2_470;
            string line = HqDisplayQueriesHud.LastWeekPatentUpkeepSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("2,470"));
        }

        [Test]
        public void lifetime_patent_upkeep_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -28_400;
            lab.Ledger.Lifetime.Power = -171_000;
            string line = HqDisplayQueriesHud.LifetimePatentUpkeepSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("28,400").Or.Contain("171,000"));
        }

        [Test]
        public void this_week_net_grey_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 3_450;
            lab.Ledger.ThisWeek.GreyRevenue = 2_160;
            string line = HqDisplayQueriesHud.ThisWeekNetGreySpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,450").Or.Contain("2,160"));
        }

        [Test]
        public void last_week_net_grey_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 3_460;
            lab.Ledger.LastWeek.GreyRevenue = 2_680;
            string line = HqDisplayQueriesHud.LastWeekNetGreySpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,460").Or.Contain("2,680"));
        }

        [Test]
        public void lifetime_net_grey_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 120_000;
            lab.Ledger.Lifetime.GreyRevenue = 53_400;
            string line = HqDisplayQueriesHud.LifetimeNetGreySpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("120,000").Or.Contain("53,400"));
        }

        [Test]
        public void this_week_grant_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_300;
            lab.Ledger.ThisWeek.ChipSales = 8_220;
            string line = HqDisplayQueriesHud.ThisWeekGrantChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("5,300").Or.Contain("8,220"));
        }

        [Test]
        public void last_week_grant_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 6_210;
            lab.Ledger.LastWeek.ChipSales = 7_910;
            string line = HqDisplayQueriesHud.LastWeekGrantChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("6,210").Or.Contain("7,910"));
        }

        [Test]
        public void lifetime_grant_chip_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 189_000;
            lab.Ledger.Lifetime.ChipSales = 114_000;
            string line = HqDisplayQueriesHud.LifetimeGrantChipSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("189,000").Or.Contain("114,000"));
        }

        [Test]
        public void this_week_fines_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -970;
            lab.Ledger.ThisWeek.ChipDevelopment = -4_120;
            string line = HqDisplayQueriesHud.ThisWeekFinesDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("970").Or.Contain("4,120"));
        }

        [Test]
        public void last_week_fines_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -980;
            lab.Ledger.LastWeek.ChipDevelopment = -4_000;
            string line = HqDisplayQueriesHud.LastWeekFinesDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("980").Or.Contain("4,000"));
        }

        [Test]
        public void lifetime_fines_dev_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Fines = -28_500;
            lab.Ledger.Lifetime.ChipDevelopment = -93_000;
            string line = HqDisplayQueriesHud.LifetimeFinesDevSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("28,500").Or.Contain("93,000"));
        }

        [Test]
        public void this_week_hardware_land_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -2_220;
            lab.Ledger.ThisWeek.Grants = 5_310;
            string line = HqDisplayQueriesHud.ThisWeekHardwareLandSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,220").Or.Contain("5,310"));
        }

        [Test]
        public void last_week_hardware_land_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -2_200;
            lab.Ledger.LastWeek.Revenue = 18_400;
            string line = HqDisplayQueriesHud.LastWeekHardwareLandSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("18,400"));
        }

        [Test]
        public void lifetime_hardware_land_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Construction = -86_500;
            lab.Ledger.Lifetime.Net = 121_000;
            string line = HqDisplayQueriesHud.LifetimeHardwareLandSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("86,500").Or.Contain("121,000"));
        }

        [Test]
        public void this_week_site_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 3_470;
            lab.Ledger.ThisWeek.Grants = 5_320;
            string line = HqDisplayQueriesHud.ThisWeekSiteCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,470").Or.Contain("5,320"));
        }

        [Test]
        public void last_week_site_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 3_480;
            lab.Ledger.LastWeek.Grants = 6_220;
            string line = HqDisplayQueriesHud.LastWeekSiteCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,480").Or.Contain("6,220"));
        }

        [Test]
        public void lifetime_site_capital_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 190_000;
            lab.Ledger.Lifetime.Revenue = 451_000;
            string line = HqDisplayQueriesHud.LifetimeSiteCapitalSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("190,000").Or.Contain("451,000"));
        }

        [Test]
        public void this_week_shady_launder_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_170;
            lab.Ledger.ThisWeek.Laundered = 3_510;
            string line = HqDisplayQueriesHud.ThisWeekShadyLaunderSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,170").Or.Contain("3,510"));
        }

        [Test]
        public void last_week_shady_launder_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_690;
            lab.Ledger.LastWeek.Laundered = 3_130;
            string line = HqDisplayQueriesHud.LastWeekShadyLaunderSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,690").Or.Contain("3,130"));
        }

        [Test]
        public void lifetime_shady_launder_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 53_500;
            lab.Ledger.Lifetime.Laundered = 33_200;
            string line = HqDisplayQueriesHud.LifetimeShadyLaunderSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("53,500").Or.Contain("33,200"));
        }

        [Test]
        public void this_week_chip_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 8_230;
            lab.Ledger.ThisWeek.Revenue = 18_850;
            string line = HqDisplayQueriesHud.ThisWeekChipRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("8,230").Or.Contain("18,850"));
        }

        [Test]
        public void last_week_chip_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 7_920;
            lab.Ledger.LastWeek.Revenue = 18_410;
            string line = HqDisplayQueriesHud.LastWeekChipRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("7,920").Or.Contain("18,410"));
        }

        [Test]
        public void lifetime_chip_revenue_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 115_000;
            lab.Ledger.Lifetime.Revenue = 452_000;
            string line = HqDisplayQueriesHud.LifetimeChipRevenueSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("115,000").Or.Contain("452,000"));
        }

        [Test]
        public void this_week_patent_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_330;
            lab.Ledger.ThisWeek.Fines = -990;
            string line = HqDisplayQueriesHud.ThisWeekPatentGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("5,330").Or.Contain("990"));
        }

        [Test]
        public void last_week_patent_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Grants = 6_230;
            lab.Ledger.LastWeek.PatentLicense = 4_210;
            string line = HqDisplayQueriesHud.LastWeekPatentGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("6,230").Or.Contain("4,210"));
        }

        [Test]
        public void lifetime_patent_grant_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 191_000;
            lab.Ledger.Lifetime.Fines = -28_600;
            string line = HqDisplayQueriesHud.LifetimePatentGrantSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("191,000").Or.Contain("28,600"));
        }

        [Test]
        public void this_week_grey_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_180;
            lab.Ledger.ThisWeek.Fines = -1_000;
            string line = HqDisplayQueriesHud.ThisWeekGreyFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,180").Or.Contain("1,000"));
        }

        [Test]
        public void last_week_grey_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 2_700;
            lab.Ledger.LastWeek.Fines = -1_010;
            string line = HqDisplayQueriesHud.LastWeekGreyFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("2,700").Or.Contain("1,010"));
        }

        [Test]
        public void lifetime_grey_fines_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.GreyRevenue = 53_600;
            lab.Ledger.Lifetime.Fines = -28_700;
            string line = HqDisplayQueriesHud.LifetimeGreyFinesSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("53,600").Or.Contain("28,700"));
        }

        [Test]
        public void this_week_net_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 3_490;
            lab.Ledger.ThisWeek.Training = -910;
            string line = HqDisplayQueriesHud.ThisWeekNetTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,490").Or.Contain("910"));
        }

        [Test]
        public void last_week_net_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Net = 3_500;
            lab.Ledger.LastWeek.Training = -890;
            string line = HqDisplayQueriesHud.LastWeekNetTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("3,500").Or.Contain("890"));
        }

        [Test]
        public void lifetime_net_training_spend_income_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 122_000;
            lab.Ledger.Lifetime.Training = -53_000;
            string line = HqDisplayQueriesHud.LifetimeNetTrainingSpendIncomeSummaryLine(lab);
            Assert.That(line, Does.Contain("122,000").Or.Contain("53,000"));
        }

    }
}