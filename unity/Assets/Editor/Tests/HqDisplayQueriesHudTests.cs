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

    }
}