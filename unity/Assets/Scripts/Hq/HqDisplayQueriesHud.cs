using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Unity.Hq
{
    /// <summary>One-line HUD formatters backed by <see cref="DisplayQueries"/>.</summary>
    public static class HqDisplayQueriesHud
    {
        public static string WeeklyPowerLine(Simulation sim)
        {
            if (sim == null) return "";
            var bills = DisplayQueries.WeeklyPowerBillParts(sim.Config, sim.State, sim.Player);
            if (bills == null || bills.Count == 0) return "";
            long total = 0;
            for (int i = 0; i < bills.Count; i++)
                total += bills[i].Total;
            if (total <= 0) return "";
            return "Power $" + total.ToString("N0") + "/wk";
        }


        public static string LastWeekHardwareHiringSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }

        public static string ThisWeekRevenueSalariesSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string LifetimeNetLaunderSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string LastWeekSiteTrainingSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string ThisWeekGreyChipSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LastWeekPowerNetSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekConstructionGreySummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LastWeekGpuLandSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }

        public static string LifetimePowerChipSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string ThisWeekGpuGreySummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string LastWeekPatentNetSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekTrainingGrantSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }


        public static string LastWeekCapitalHardwareSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string ThisWeekFinesNetSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeGrantsChipSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekLaunderPowerSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string ThisWeekSalariesConstructionSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekChipTrainingSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekGrantsPowerSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekRoomLandSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LifetimeFinesTrainingSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string ThisWeekNetRevenueSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekHiringSiteSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(patent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            return line;
        }}

        public static string ThisWeekChipRentSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}


        public static string LastWeekPatentGreySummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekRentNetSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LifetimeRevenueLaunderSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekConstructionPowerSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LastWeekSalariesGreySummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string ThisWeekGrantChipSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekHardwareSiteSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(land))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            return line;
        }}

        public static string ThisWeekFinesConstructionSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeGreyTrainingSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string LastWeekGrantLaunderSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekGrantPowerSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekTrainingGreySummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }}


        public static string LastWeekRevenueCapitalSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekNetGrantSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeLaunderNetSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekFinesHardwareSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string ThisWeekSalariesPowerSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekGrantSiteSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(land))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            return line;
        }}

        public static string ThisWeekConstructionChipSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekNetLandSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string LifetimeHardwareTrainingSummaryLine(LabState lab)
        {
            string training = LifetimeLedgerTrainingLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekGreyGrantSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekChipPowerSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }}

        public static string ThisWeekTrainingFinesSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}


        public static string LastWeekSalariesSiteSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string ThisWeekRevenueGreySummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LifetimePatentGrantsSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string LastWeekTrainingCapitalSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekGpuGrantSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekGreyLandSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string ThisWeekNetConstructionSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LastWeekRevenueLaunderSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeSiteChipSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string ThisWeekFinesGreySummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }}

        public static string LastWeekHardwareNetSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string construction = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(construction))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(construction))
                line = string.IsNullOrEmpty(line) ? construction : line + " · " + construction;
            return line;
        }}

        public static string ThisWeekGrantTrainingSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string chipSales = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(chipSales))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(chipSales))
                line = string.IsNullOrEmpty(line) ? chipSales : line + " · " + chipSales;
            return line;
        }}


        public static string LastWeekPatentPowerSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekChipGrantSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeNetChipSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekSiteGreySummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekRevenueFinesSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekGrantHardwareSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string ThisWeekRentGrantSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekFinesSiteSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string construction = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(construction))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(construction))
                line = string.IsNullOrEmpty(line) ? construction : line + " · " + construction;
            return line;
        }}

        public static string LifetimeGreyPowerSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekSalariesGrantSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekChipLandSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string LastWeekUpkeepGpuSummaryLine(LabState lab)
        {
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(upkeep)) line = upkeep;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}


        public static string LastWeekTrainingSiteSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string ThisWeekNetChipGrantSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeLaunderChipSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekRoomPatentSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekPowerGreySummaryLine(LabState lab)
        {
            string power = ThisWeekLedgerPowerLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string construction = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(construction))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(construction))
                line = string.IsNullOrEmpty(line) ? construction : line + " · " + construction;
            return line;
        }}

        public static string LastWeekRevenueHardwareSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string ThisWeekGrantConstructionSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeFinesChipSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekLaunderChipSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekCapitalLaunderSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekGrantChipNetSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekPatentTrainingSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }}


        public static string LastWeekGreyHardwareSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string ThisWeekNetPowerSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeRevenueNetSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string LastWeekGrantPatentSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekChipBuildSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekLaunderGreySummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekGrantRevenueSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekSiteRoomSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string LifetimeTrainingGrantsSummaryLine(LabState lab)
        {
            string training = LifetimeLedgerTrainingLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekCapitalSiteSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekRentChipSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LastWeekFinesGrantSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}


        public static string LastWeekPowerLandSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string construction = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(construction))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(construction))
                line = string.IsNullOrEmpty(line) ? construction : line + " · " + construction;
            return line;
        }}

        public static string ThisWeekGrantGreySummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LifetimeHardwareChipSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekRoomTrainingSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string ThisWeekTrainingNetSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekChipGrantSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string ThisWeekGpuPowerSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LifetimePatentChipSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekNetConstructionSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LastWeekHiringHardwareSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekSalariesChipSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LastWeekGreyPatentSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}


        public static string LastWeekLandGrantSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekFinesGrantSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeSiteGreySummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekGpuHardwareSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string ThisWeekChipGreyGrantSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekTrainingHiringSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string ThisWeekRevenueConstructionSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LifetimeGrantFinesSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekSalariesGreySummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekRevenueSiteSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekRentFinesSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekConstructionGrantSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}


        public static string ThisWeekNetSalariesSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LifetimeLaunderPowerSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekPatentLandSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string ThisWeekPowerConstructionSummaryLine(LabState lab)
        {
            string power = ThisWeekLedgerPowerLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekLaunderCapitalSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekGreySalariesSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeNetGrantSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekHardwareTrainingSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string ThisWeekGrantNetChipSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekSitePowerSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string ThisWeekGrantFinesSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekRoomNetSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}


        public static string ThisWeekChipDevSummaryLine(LabState lab)
        {
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chipDev)) line = chipDev;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekFinesGpuSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LifetimeRevenueTrainingSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string training = LifetimeLedgerTrainingSpendLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekHiringSalariesSummaryLine(LabState lab)
        {
            string hiring = ThisWeekLedgerHiringLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekGrantLandSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekPatentGreySummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekPowerRentSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string ThisWeekCapitalLandSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LastWeekGreyPowerSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekSiteRevenueSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekNetChipSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeConstructionGrantSummaryLine(LabState lab)
        {
            string build = LifetimeLedgerConstructionLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}


        public static string ThisWeekLaunderFinesSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekTrainingPowerSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(hardware))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            return line;
        }}

        public static string LifetimePatentRevenueSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekSalariesConstructionSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string ThisWeekChipDevGrantSummaryLine(LabState lab)
        {
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chipDev)) line = chipDev;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekRentRevenueSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekHardwareFinesSummaryLine(LabState lab)
        {
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LifetimeRoomGrantSummaryLine(LabState lab)
        {
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekChipFinesSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekLandTrainingSummaryLine(LabState lab)
        {
            string land = ThisWeekLedgerLandLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LastWeekGrantGpuSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LifetimeNetPowerSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}


        public static string ThisWeekCapitalTrainingSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LastWeekSiteChipSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LifetimeGreyRoomSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekGrantRevenueSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekLaunderGrantSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekPatentGrantSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekChipRoomSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekGreyConstructionSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string ThisWeekNetLandSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekSalariesRentSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string ThisWeekGrantLandSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string capital = ThisWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string LifetimeCapitalGreySummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}


        public static string LastWeekRoomFinesSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekGpuHardwareSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string upkeep = ThisWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LifetimeFinesRentSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string LastWeekCapitalConstructionSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string ThisWeekGreyLandSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekLandNetSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekChipPatentSummaryLine(LabState lab)
        {
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chipDev)) line = chipDev;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeHiringSalariesSummaryLine(LabState lab)
        {
            string hiring = LifetimeLedgerHiringLine(lab);
            string salaries = LifetimeLedgerSalariesLine(lab);
            string training = LifetimeLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekHardwareChipSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LifetimeTrainingGrantSummaryLine(LabState lab)
        {
            string training = LifetimeLedgerTrainingSpendLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekNetGrantSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}


        public static string ThisWeekSiteGrantSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekLaunderRoomSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekHiringChipSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeLandGrantSummaryLine(LabState lab)
        {
            string land = LifetimeLedgerLandLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekPowerSalariesSummaryLine(LabState lab)
        {
            string power = ThisWeekLedgerPowerLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekCapitalChipSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LifetimeRentGreySummaryLine(LabState lab)
        {
            string gpuRent = LifetimeLedgerGpuRentLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekPatentChipSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekGrantRoomSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string ThisWeekLaunderChipSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeSiteTrainingSummaryLine(LabState lab)
        {
            string site = LifetimeLedgerSiteBuildLine(lab);
            string training = LifetimeLedgerTrainingSpendLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekGpuGrantSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}


        public static string LastWeekConstructionNetSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekHardwareGrantSummaryLine(LabState lab)
        {
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LifetimeFinesLandSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekChipRoomSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekPatentLandSummaryLine(LabState lab)
        {
            string land = ThisWeekLedgerLandLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekRentLandSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekRoomTrainingSummaryLine(LabState lab)
        {
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeChipGrantSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekPowerGrantSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekRentLandSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekLaunderTrainingSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeNetRoomSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}


        public static string LifetimePowerGrantSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekGreyGrantSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekFinesLandSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeHardwareRentSummaryLine(LabState lab)
        {
            string hardware = LifetimeLedgerHardwareLine(lab);
            string gpuRent = LifetimeLedgerGpuRentLine(lab);
            string upkeep = LifetimeLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LastWeekCapitalPowerSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekPatentTrainingSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekLaunderPowerSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeGreyChipSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekRentConstructionSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LifetimeSalariesPowerSummaryLine(LabState lab)
        {
            string salaries = LifetimeLedgerSalariesLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string training = LifetimeLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekFinesRoomSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekLandChipSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}


        public static string ThisWeekHardwareRoomSummaryLine(LabState lab)
        {
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LifetimePatentGrantSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekGreyChipSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekCapitalGrantSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekTrainingChipSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string ThisWeekSiteFinesSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekGrantPatentSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekLaunderGrantSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekChipPowerSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeConstructionPowerSummaryLine(LabState lab)
        {
            string build = LifetimeLedgerConstructionLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekHiringGrantSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string ThisWeekGreyRentSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}


        public static string LastWeekSiteGrantSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekLaunderLandSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeRentGrantSummaryLine(LabState lab)
        {
            string gpuRent = LifetimeLedgerGpuRentLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekCapitalChipSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LifetimeGreyLandSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string ThisWeekConstructionLandSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string site = ThisWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LastWeekPatentRoomSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekGrantHardwareSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LastWeekGreyRentSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string ThisWeekChipLandSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekSiteNetSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekGrantChipSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}


        public static string LifetimeLaunderGrantSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekRentTrainingSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeRoomChipSummaryLine(LabState lab)
        {
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekFinesChipSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekCapitalPowerSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekSiteLandSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LastWeekLaunderLandSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekGrantSiteSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeChipLandSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekRentTrainingSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string LifetimeHiringPowerSummaryLine(LabState lab)
        {
            string hiring = LifetimeLedgerHiringLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string salaries = LifetimeLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string ThisWeekNetRoomSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}


        public static string ThisWeekLaunderRoomSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeRentLandSummaryLine(LabState lab)
        {
            string gpuRent = LifetimeLedgerGpuRentLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekSalariesGrantSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekSiteChipSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekCapitalConstructionSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string site = ThisWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LifetimeGrantLandSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekRoomGrantSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string ThisWeekPatentRentSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekFinesRentSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeChipRoomSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekConstructionFinesSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LifetimeSalariesGrantSummaryLine(LabState lab)
        {
            string salaries = LifetimeLedgerSalariesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}


        public static string LastWeekRentGrantSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekLandChipSummaryLine(LabState lab)
        {
            string land = ThisWeekLedgerLandLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekGreySiteSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekHardwareLandSummaryLine(LabState lab)
        {
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }}

        public static string LastWeekFinesLandSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekCapitalGrantSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekRoomGrantSummaryLine(LabState lab)
        {
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekConstructionRentSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string ThisWeekSalariesLandSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekNetRoomSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LifetimeGrantChipSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string ThisWeekGrantLaunderSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}


        public static string ThisWeekSiteTrainingSummaryLine(LabState lab)
        {
            string site = ThisWeekLedgerSiteBuildLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }}

        public static string LifetimeRentChipSummaryLine(LabState lab)
        {
            string gpuRent = LifetimeLedgerGpuRentLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeGreyGrantSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }}

        public static string LastWeekChipRentSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string ThisWeekGreyPatentSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeFinesGrantSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LastWeekLandConstructionSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string ThisWeekRentPowerSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekGreyFinesSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimePowerLandSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekHardwareGrantSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string LastWeekSiteRentSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}


        public static string LifetimeConstructionChipSummaryLine(LabState lab)
        {
            string build = LifetimeLedgerConstructionLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekGrantPowerSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekLandGrantSummaryLine(LabState lab)
        {
            string land = ThisWeekLedgerLandLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string capital = ThisWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string ThisWeekSalariesRentSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeLaunderLandSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekSiteFinesSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekCapitalRentSummaryLine(LabState lab)
        {
            string capital = ThisWeekLedgerCapitalInLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string LifetimeGrantRoomSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekRentChipSummaryLine(LabState lab)
        {
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeLaunderFinesSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string ThisWeekLaunderConstructionSummaryLine(LabState lab)
        {
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}


        public static string ThisWeekFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LifetimeFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekGrantSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LastWeekGrantSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string LifetimeGrantSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekChipSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LastWeekChipSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string LifetimeChipSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }}

        public static string ThisWeekDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LifetimeDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}


        public static string ThisWeekGpuSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string gpu = ThisWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(gpu))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(gpu))
                line = string.IsNullOrEmpty(line) ? gpu : line + " · " + gpu;
            return line;
        }}

        public static string LastWeekGpuSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string gpu = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(gpu))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(gpu))
                line = string.IsNullOrEmpty(line) ? gpu : line + " · " + gpu;
            return line;
        }}

        public static string LifetimeGpuSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string gpu = LifetimeLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(gpu))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(gpu))
                line = string.IsNullOrEmpty(line) ? gpu : line + " · " + gpu;
            return line;
        }}

        public static string ThisWeekRevenueSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LastWeekRevenueSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }}

        public static string LifetimeRevenueSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }}

        public static string ThisWeekGrantFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LastWeekGrantFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string LifetimeGrantFinesSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }}

        public static string ThisWeekChipDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string chipDev = ThisWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LastWeekChipDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}

        public static string LifetimeChipDevSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string chipDev = LifetimeLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }}


        public static string ThisWeekHardwareSiteSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string hardware = ThisWeekLedgerHardwareLine(lab);
            string site = ThisWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LastWeekHardwareSiteSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string LifetimeHardwareSiteSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string hardware = LifetimeLedgerHardwareLine(lab);
            string site = LifetimeLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }}

        public static string ThisWeekLandRoomSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string land = ThisWeekLedgerLandLine(lab);
            string room = ThisWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string LastWeekLandRoomSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string LifetimeLandRoomSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string land = LifetimeLedgerLandLine(lab);
            string room = LifetimeLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }}

        public static string ThisWeekRentTrainingSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string rent = ThisWeekLedgerGpuRentLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(rent))
                line = string.IsNullOrEmpty(line) ? rent : line + " · " + rent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LastWeekRentTrainingSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string rent = LastWeekLedgerGpuRentLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(rent))
                line = string.IsNullOrEmpty(line) ? rent : line + " · " + rent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string LifetimeRentTrainingSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string rent = LastWeekLedgerGpuRentLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(rent))
                line = string.IsNullOrEmpty(line) ? rent : line + " · " + rent;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }}

        public static string ThisWeekPowerConstructionSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LastWeekPowerConstructionSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}

        public static string LifetimePowerConstructionSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string build = LifetimeLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }}


        public static string ThisWeekSalariesHiringSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string LastWeekSalariesHiringSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string LifetimeSalariesHiringSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string salaries = LifetimeLedgerSalariesLine(lab);
            string hiring = LifetimeLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }}

        public static string ThisWeekLaunderCapitalSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string laundered = ThisWeekLedgerLaunderedLine(lab);
            string capital = ThisWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string LastWeekLaunderCapitalSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }}

        public static string LifetimeLaunderCapitalSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }}

        public static string ThisWeekPatentUpkeepSpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string upkeep = ThisWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LastWeekPatentUpkeepSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string LifetimePatentUpkeepSpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            string upkeep = LifetimeLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }}

        public static string ThisWeekNetGreySpendIncomeSummaryLine(LabState lab)
        {
            string spend = ThisWeekLedgerSpendSummaryLine(lab);
            string income = ThisWeekLedgerIncomeSummaryLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LastWeekNetGreySpendIncomeSummaryLine(LabState lab)
        {
            string spend = LastWeekLedgerSpendSummaryLine(lab);
            string income = LastWeekLedgerIncomeSummaryLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(income) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(income))
                line = string.IsNullOrEmpty(line) ? income : line + " · " + income;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string LifetimeNetGreySpendIncomeSummaryLine(LabState lab)
        {
            string spend = LifetimeLedgerSpendSummaryLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(spend) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(spend)) line = spend;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }}

        public static string BankruptLine(LabState lab)
        {
            if (lab == null || !lab.Bankrupt) return "";
            return "Bankrupt";
        }

        public static string LatestVendorCourtLine(GameState state, int tick)
        {
            return "";
        }

        public static string WeeklyComputeLine(GameConfig config, GameState state, LabState lab)
        {
            return "";
        }


        public static string ActiveDealsSummaryLine(LabState lab)
        {
            if (lab == null) return "";
            int ent = 0;
            int data = 0;
            if (lab.EnterpriseDeals != null)
            {
                for (int i = 0; i < lab.EnterpriseDeals.Count; i++)
                {
                    if (lab.EnterpriseDeals[i].Phase == EnterpriseDealPhase.Active) ent++;
                }
            }
            if (lab.DataLicenses != null)
            {
                for (int i = 0; i < lab.DataLicenses.Count; i++)
                {
                    if (lab.DataLicenses[i].Phase == DataLicensePhase.Active) data++;
                }
            }
            if (ent <= 0 && data <= 0) return "";
            return ent + " enterprise · " + data + " data deal(s)";
        }

        public static string {name}(LabState lab)
        {{
{decl}
            if ({empty})
                return "";
            string line = "";
            if (!string.IsNullOrEmpty({first})) line = {first};
""" + "\n".join(joins) + """
            return line;
        }}
"""


def insert_formatters(batch):
    t = HUD_PATH.read_text()
    block = ""
    for item in batch:
        name = item[0]
        if name in t:
            continue
        block += formatter_body(name, item[1])
    if not block:
        return
    HUD_PATH.write_text(
        t.replace(
            "        public static string BankruptLine(LabState lab)",
            block + "\n        public static string BankruptLine(LabState lab)",
            1,
        )
    )


def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def cycle_def(t):
    method, var, slug, test_name, ledger, a, b = t
    test_body = f"""            var lab = new LabState {{ Id = 1 }};
            {ledger}
            string line = HqDisplayQueriesHud.{method}(lab);
            Assert.That(line, Does.Contain("{a}").Or.Contain("{b}"));"""
    return dict(method=method, var=var, slug=slug, test_name=test_name, test_body=test_body)


def wire_hud(c):
    t = HUD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                if (!string.IsNullOrEmpty({c['var']}))
                    status = string.IsNullOrEmpty(status) ? {c['var']} : status + " · " + {c['var']};
"""
    b = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    HUD.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_train(c):
    t = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                    string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                    if (!string.IsNullOrEmpty({c['var']}))
                        _summary.text += " · " + {c['var']};
"""
    b = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    TRAIN.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_staff(c, part_anchor, tail_token):
    t = STAFF.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*)", t)
    t = t.replace(m.group(1), m.group(1) + f" + {c['var']}Part", 1)
    STAFF.write_text(t)
    return True


def wire_mascot(c, part_anchor, tail_token):
    t = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(sim.Player)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(sim.Player);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*);", t)
    t = t.replace(m.group(1) + ";", m.group(1) + f" + {c['var']}Part;", 1)
    MASCOT.write_text(t)
    return True


def wire_build(c):
    t = BUILD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(_host.Simulation.Player)" in t:
        return False
    b = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{c["method"]}(_host.Simulation.Player)\n'
    BUILD.write_text(t.replace(b, line + b, 1))
    return True


def add_test(c):
    t = TESTS.read_text()
    if c["test_name"] in t:
        return False
    block = f"\n        [Test]\n        public void {c['test_name']}()\n        {{\n{c['test_body']}\n        }}\n"
    at = t.rfind("\n    }\n}")
    TESTS.write_text(t[:at] + block + t[at:])
    return True


def run_sprint(cycles, part_anchor, tail_token):
    for i, raw in enumerate(cycles, 1):
        c = cycle_def(raw)
        if len(raw) == 6:  # sprint52 lifetimeNetRoom only one assert value
            pass
        if wire_hud(c):
            git_commit(f"feat(hud): wire hud view {c['slug']}", [HUD])
        if wire_train(c):
            git_commit(f"feat(hud): wire training {c['slug']}", [TRAIN])
        if wire_staff(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire staff gpu {c['slug']}", [STAFF])
        if wire_mascot(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire build mascot {c['slug']}", [MASCOT])
        if wire_build(c):
            git_commit(f"feat(hud): wire build panel {c['slug']}", [BUILD])
        if add_test(c):
            git_commit(f"test(hud): {c['slug']} summary line", [TESTS])
        if i in {2, 4}:
            subprocess.check_call(
                ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
                cwd=ROOT,
            )


insert_formatters(F49_50)
git_commit("feat(hud): sprint49-50 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT49, PA49, "lastWeekNetGrantPart")
run_sprint(SPRINT50, PA50, "thisWeekCapitalChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F51_52)
git_commit("feat(hud): sprint51-52 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT51, PA51, "lastWeekGpuGrantPart")
run_sprint(SPRINT52, PA52, "lastWeekRentLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

        public static string BenchmarkSlaSummaryLine(LabState lab)
        {
            string bench = PublicBenchmarksLiveLine(lab);
            string sla = SlaPledgeLine(lab);
            if (string.IsNullOrEmpty(bench) && string.IsNullOrEmpty(sla)) return "";
            if (!string.IsNullOrEmpty(bench) && !string.IsNullOrEmpty(sla))
                return bench + " · " + sla;
            return string.IsNullOrEmpty(bench) ? sla : bench;
        }

        public static string ChipShortageSummaryLine(GameState state, int tick)
        {
            string crunch = ChipShortageLine(state);
            string ends = ChipShortageWeeksLeftLine(state, tick);
            if (string.IsNullOrEmpty(crunch) && string.IsNullOrEmpty(ends)) return "";
            if (!string.IsNullOrEmpty(crunch) && !string.IsNullOrEmpty(ends))
                return crunch + " · " + ends;
            return string.IsNullOrEmpty(crunch) ? ends : crunch;
        }

        public static string ComputeServingSummaryLine(GameConfig config, GameState state, LabState lab)
        {
            string weekly = WeeklyComputeLine(config, state, lab);
            string serving = WeeklyServingComputeLine(config, state, lab);
            if (string.IsNullOrEmpty(weekly) && string.IsNullOrEmpty(serving)) return "";
            if (!string.IsNullOrEmpty(weekly) && !string.IsNullOrEmpty(serving))
                return weekly + " · " + serving;
            return string.IsNullOrEmpty(weekly) ? serving : weekly;
        }

        public static string DealStrikesSummaryLine(LabState lab)
        {
            string data = DataLicenseStrikesLine(lab);
            string ent = EnterpriseDealStrikesLine(lab);
            if (string.IsNullOrEmpty(data) && string.IsNullOrEmpty(ent)) return "";
            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(ent))
                return data + " · " + ent;
            return string.IsNullOrEmpty(data) ? ent : data;
        }

        public static string DealsRiskSummaryLine(LabState lab)
        {
            string ent = EnterpriseDealsAtRiskLine(lab);
            string data = DataLicensesAtRiskLine(lab);
            if (string.IsNullOrEmpty(ent) && string.IsNullOrEmpty(data)) return "";
            if (!string.IsNullOrEmpty(ent) && !string.IsNullOrEmpty(data))
                return ent + " · " + data;
            return string.IsNullOrEmpty(ent) ? data : ent;
        }

        public static string ForbearanceSummaryLine(LabState lab, int tick)
        {
            string debt = DebtForbearanceLine(lab, tick);
            string peer = PeerForbearanceActiveLine(lab, tick);
            string landlord = LandlordPeerForbearanceLine(lab, tick);
            if (string.IsNullOrEmpty(debt) && string.IsNullOrEmpty(peer) && string.IsNullOrEmpty(landlord)) return "";
            var parts = new System.Collections.Generic.List<string>(3);
            if (!string.IsNullOrEmpty(debt)) parts.Add(debt);
            if (!string.IsNullOrEmpty(peer)) parts.Add(peer);
            if (!string.IsNullOrEmpty(landlord)) parts.Add(landlord);
            return string.Join(" · ", parts);
        }

        public static string FrontierResearchSummaryLine(LabState lab)
        {
            if (lab == null) return "";
            bool hasFrontier = lab.FrontierLevel > 0;
            bool hasRp = lab.ResearchPoints > 0;
            if (!hasFrontier && !hasRp) return "";
            if (hasFrontier && hasRp)
                return "Frontier L" + lab.FrontierLevel + " · " + lab.ResearchPoints.ToString("N0") + " RP";
            if (hasFrontier) return "Frontier L" + lab.FrontierLevel;
            return lab.ResearchPoints.ToString("N0") + " RP";
        }

        public static string GrantLedgerSummaryLine(LabState lab, int tick)
        {
            string grantWeeks = GrantWeeksRemainingLine(lab, tick);
            string grantAmt = ActiveGrantAmountLine(lab, tick);
            string monthNet = ThisMonthLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grantWeeks) && string.IsNullOrEmpty(grantAmt) && string.IsNullOrEmpty(monthNet))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grantWeeks)) line = grantWeeks;
            if (!string.IsNullOrEmpty(grantAmt))
                line = string.IsNullOrEmpty(line) ? grantAmt : line + " · " + grantAmt;
            if (!string.IsNullOrEmpty(monthNet))
                line = string.IsNullOrEmpty(line) ? monthNet : line + " · " + monthNet;
            return line;
        }

        public static string GreyComputeSummaryLine(Simulation sim)
            string weekly = GreyComputeWeeklyLine(sim);
            string billed = GreyComputeBilledKwLine(sim);
            if (string.IsNullOrEmpty(weekly) && string.IsNullOrEmpty(billed)) return "";
            if (!string.IsNullOrEmpty(weekly) && !string.IsNullOrEmpty(billed))
                return weekly + " · " + billed;
            return string.IsNullOrEmpty(weekly) ? billed : weekly;
        }

        public static string HeatwaveSummaryLine(GameConfig config, Simulation sim)
            if (sim == null) return "";
            string sites = HeatwaveSitesLine(sim);
            string throttle = HeatwaveThrottleLine(config, sim.Player);
            if (string.IsNullOrEmpty(sites) && string.IsNullOrEmpty(throttle)) return "";
            if (!string.IsNullOrEmpty(sites) && !string.IsNullOrEmpty(throttle))
                return sites + " · " + throttle;
            return string.IsNullOrEmpty(sites) ? throttle : sites;

        public static string LastWeekCapitalGreySummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekCapitalPatentSummaryLine(LabState lab)
        {
            string capital = LastWeekLedgerCapitalInLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(capital)) line = capital;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LastWeekChipDevLandSummaryLine(LabState lab)
        {
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            if (string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(hardware))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chipDev)) line = chipDev;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            return line;
        }

        public static string LastWeekChipDevSalesSummaryLine(LabState lab)
        {
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            string chipSales = LastWeekLedgerChipSalesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chipSales) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chipDev)) line = chipDev;
            if (!string.IsNullOrEmpty(chipSales))
                line = string.IsNullOrEmpty(line) ? chipSales : line + " · " + chipSales;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LastWeekChipGreySummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chipDev))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            return line;
        }

        public static string LastWeekChipPatentSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }

        public static string LastWeekChipSalesGrantsSummaryLine(LabState lab)
        {
            string chips = LastWeekLedgerChipSalesLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LastWeekConstructionChipSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            string chipSales = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chipSales))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            if (!string.IsNullOrEmpty(chipSales))
                line = string.IsNullOrEmpty(line) ? chipSales : line + " · " + chipSales;
            return line;
        }

        public static string LastWeekConstructionGreySummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekConstructionLandSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(land) && string.IsNullOrEmpty(hardware))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            return line;
        }

        public static string LastWeekConstructionPatentSummaryLine(LabState lab)
        {
            string build = LastWeekLedgerConstructionLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(hiring))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            return line;
        }

        public static string LastWeekFinesCapitalSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string LastWeekFinesGreySummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekFinesTrainingSummaryLine(LabState lab)
        {
            string fines = LastWeekLedgerFinesLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }

        public static string LastWeekGpuFinesSummaryLine(LabState lab)
        {
            string rent = LastWeekLedgerGpuRentLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(rent)) line = rent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }

        public static string LastWeekGpuGreySummaryLine(LabState lab)
        {
            string rent = LastWeekLedgerGpuRentLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(rent)) line = rent;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }

        public static string LastWeekGpuTrainingSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string gpuRent = LastWeekLedgerGpuRentLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }

        public static string LastWeekGpuUpkeepRentSummaryLine(LabState lab)
        {
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            string rent = LastWeekLedgerGpuRentLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(rent) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(upkeep)) line = upkeep;
            if (!string.IsNullOrEmpty(rent))
                line = string.IsNullOrEmpty(line) ? rent : line + " · " + rent;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string LastWeekGrantTrainingSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LastWeekGrantsCapitalSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string LastWeekGrantsRevenueSummaryLine(LabState lab)
        {
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string LastWeekGreyGrantsSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string LastWeekGreyLaunderSummaryLine(LabState lab)
        {
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string LastWeekHardwarePowerSummaryLine(LabState lab)
        {
            string hardware = LastWeekLedgerHardwareLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hardware)) line = hardware;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string LastWeekHiringCapitalSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }

        public static string LastWeekHiringGreySummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekHiringPatentSummaryLine(LabState lab)
        {
            string hiring = LastWeekLedgerHiringLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            if (string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(capital))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(hiring)) line = hiring;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            return line;
        }

        public static string LastWeekLandHardwareSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }

        public static string LastWeekLandPatentSummaryLine(LabState lab)
        {
            string land = LastWeekLedgerLandLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            if (string.IsNullOrEmpty(land) && string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(site))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(land)) line = land;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            return line;
        }

        public static string LastWeekLaunderFinesSummaryLine(LabState lab)
        {
            string laundered = LastWeekLedgerLaunderedLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string LastWeekLedgerCapitalInLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.CapitalIn <= 0) return "";
            return "Last wk capital in $" + lab.Ledger.LastWeek.CapitalIn.ToString("N0");
        }

        public static string LastWeekLedgerChipDevelopmentLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.ChipDevelopment >= 0) return "";
            return "Last wk chip dev $" + (-lab.Ledger.LastWeek.ChipDevelopment).ToString("N0");
        }

        public static string LastWeekLedgerChipSalesLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.ChipSales <= 0) return "";
            return "Last wk chip sales $" + lab.Ledger.LastWeek.ChipSales.ToString("N0");
        }

        public static string LastWeekLedgerConstructionLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Construction >= 0) return "";
            return "Last wk construction $" + (-lab.Ledger.LastWeek.Construction).ToString("N0");
        }

        public static string LastWeekLedgerFinesLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Fines >= 0) return "";
            return "Last wk fines $" + (-lab.Ledger.LastWeek.Fines).ToString("N0");
        }

        public static string LastWeekLedgerGpuRentLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.GpuRent >= 0) return "";
            return "Last wk GPU rent $" + (-lab.Ledger.LastWeek.GpuRent).ToString("N0");
        }

        public static string LastWeekLedgerGpuUpkeepLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.GpuUpkeep >= 0) return "";
            return "Last wk GPU upkeep $" + (-lab.Ledger.LastWeek.GpuUpkeep).ToString("N0");
        }

        public static string LastWeekLedgerGrantsLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Grants <= 0) return "";
            return "Last wk grants $" + lab.Ledger.LastWeek.Grants.ToString("N0");
        }

        public static string LastWeekLedgerGreyRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.GreyRevenue <= 0) return "";
            return "Last wk grey $" + lab.Ledger.LastWeek.GreyRevenue.ToString("N0");
        }

        public static string LastWeekLedgerHardwareLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Hardware >= 0) return "";
            return "Last wk hardware $" + (-lab.Ledger.LastWeek.Hardware).ToString("N0");
        }

        public static string LastWeekLedgerHiringLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Hiring >= 0) return "";
            return "Last wk hiring $" + (-lab.Ledger.LastWeek.Hiring).ToString("N0");
        }

        public static string LastWeekLedgerIncomeSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string LastWeekLedgerLandLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Land >= 0) return "";
            return "Last wk land $" + (-lab.Ledger.LastWeek.Land).ToString("N0");
        }

        public static string LastWeekLedgerLaunderedLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Laundered <= 0) return "";
            return "Last wk laundered $" + lab.Ledger.LastWeek.Laundered.ToString("N0");
        }

        public static string LastWeekLedgerNetLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null) return "";
            long net = lab.Ledger.LastWeek.Net;
            if (net == 0) return "";
            if (net > 0) return "Last wk ledger net +$" + net.ToString("N0");
            return "Last wk ledger net -$" + (-net).ToString("N0");
        }

        public static string LastWeekLedgerPatentLicenseLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.PatentLicense == 0) return "";
            long amount = lab.Ledger.LastWeek.PatentLicense;
            if (amount > 0) return "Last wk patent license +$" + amount.ToString("N0");
            return "Last wk patent license -$" + (-amount).ToString("N0");
        }

        public static string LastWeekLedgerPowerLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Power >= 0) return "";
            return "Last wk ledger power $" + (-lab.Ledger.LastWeek.Power).ToString("N0");
        }

        public static string LastWeekLedgerRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Revenue <= 0) return "";
            return "Last wk ledger revenue $" + lab.Ledger.LastWeek.Revenue.ToString("N0");
        }

        public static string LastWeekLedgerRoomUpkeepLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.RoomUpkeep >= 0) return "";
            return "Last wk room upkeep $" + (-lab.Ledger.LastWeek.RoomUpkeep).ToString("N0");
        }

        public static string LastWeekLedgerSalariesLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Salaries >= 0) return "";
            return "Last wk salaries $" + (-lab.Ledger.LastWeek.Salaries).ToString("N0");
        }

        public static string LastWeekLedgerSiteBuildLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.SiteBuild >= 0) return "";
            return "Last wk site build $" + (-lab.Ledger.LastWeek.SiteBuild).ToString("N0");
        }

        public static string LastWeekLedgerSpendSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string LastWeekLedgerTrainingSpendLine(LabState lab)
        {
            if (lab?.Ledger?.LastWeek == null || lab.Ledger.LastWeek.Training >= 0) return "";
            return "Last wk training $" + (-lab.Ledger.LastWeek.Training).ToString("N0");
        }

        public static string LastWeekNetFinesSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string LastWeekNetGreySummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekNetPowerSummaryLine(LabState lab)
        {
            string net = LastWeekLedgerNetLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string LastWeekPatentHiringSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            string upkeep = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(upkeep))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            return line;
        }

        public static string LastWeekPatentSiteSummaryLine(LabState lab)
        {
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            string site = LastWeekLedgerSiteBuildLine(lab);
            string land = LastWeekLedgerLandLine(lab);
            if (string.IsNullOrEmpty(patent) && string.IsNullOrEmpty(site) && string.IsNullOrEmpty(land))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(patent)) line = patent;
            if (!string.IsNullOrEmpty(site))
                line = string.IsNullOrEmpty(line) ? site : line + " · " + site;
            if (!string.IsNullOrEmpty(land))
                line = string.IsNullOrEmpty(line) ? land : line + " · " + land;
            return line;
        }

        public static string LastWeekPowerChipDevSummaryLine(LabState lab)
        {
            string power = LastWeekLedgerPowerLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            string chipSales = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chipSales))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            if (!string.IsNullOrEmpty(chipSales))
                line = string.IsNullOrEmpty(line) ? chipSales : line + " · " + chipSales;
            return line;
        }

        public static string LastWeekRevenueChipSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string chips = LastWeekLedgerChipSalesLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LastWeekRevenueNetSummaryLine(LabState lab)
        {
            string revenue = LastWeekLedgerRevenueLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            string grants = LastWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LastWeekRoomChipSummaryLine(LabState lab)
        {
            string upkeep = LastWeekLedgerRoomUpkeepLine(lab);
            string chipDev = LastWeekLedgerChipDevelopmentLine(lab);
            string chipSales = LastWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(chipDev) && string.IsNullOrEmpty(chipSales))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(upkeep)) line = upkeep;
            if (!string.IsNullOrEmpty(chipDev))
                line = string.IsNullOrEmpty(line) ? chipDev : line + " · " + chipDev;
            if (!string.IsNullOrEmpty(chipSales))
                line = string.IsNullOrEmpty(line) ? chipSales : line + " · " + chipSales;
            return line;
        }

        public static string LastWeekRoomGreySummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string grey = LastWeekLedgerGreyRevenueLine(lab);
            string laundered = LastWeekLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LastWeekRoomHiringSummaryLine(LabState lab)
        {
            string upkeep = LastWeekLedgerRoomUpkeepLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            string salaries = LastWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(upkeep)) line = upkeep;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }

        public static string LastWeekRoomPowerSummaryLine(LabState lab)
        {
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string fines = LastWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(room) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(room)) line = room;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string LastWeekSalariesHiringSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string hiring = LastWeekLedgerHiringLine(lab);
            string room = LastWeekLedgerRoomUpkeepLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(hiring) && string.IsNullOrEmpty(room))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(hiring))
                line = string.IsNullOrEmpty(line) ? hiring : line + " · " + hiring;
            if (!string.IsNullOrEmpty(room))
                line = string.IsNullOrEmpty(line) ? room : line + " · " + room;
            return line;
        }

        public static string LastWeekSalariesPowerSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            string training = LastWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string LastWeekSalariesRoomSummaryLine(LabState lab)
        {
            string salaries = LastWeekLedgerSalariesLine(lab);
            string upkeep = LastWeekLedgerRoomUpkeepLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string LastWeekSiteCapitalSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string capital = LastWeekLedgerCapitalInLine(lab);
            string patent = LastWeekLedgerPatentLicenseLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(capital) && string.IsNullOrEmpty(patent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(capital))
                line = string.IsNullOrEmpty(line) ? capital : line + " · " + capital;
            if (!string.IsNullOrEmpty(patent))
                line = string.IsNullOrEmpty(line) ? patent : line + " · " + patent;
            return line;
        }

        public static string LastWeekSiteHardwareSummaryLine(LabState lab)
        {
            string site = LastWeekLedgerSiteBuildLine(lab);
            string hardware = LastWeekLedgerHardwareLine(lab);
            string construction = LastWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(site) && string.IsNullOrEmpty(hardware) && string.IsNullOrEmpty(construction))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(site)) line = site;
            if (!string.IsNullOrEmpty(hardware))
                line = string.IsNullOrEmpty(line) ? hardware : line + " · " + hardware;
            if (!string.IsNullOrEmpty(construction))
                line = string.IsNullOrEmpty(line) ? construction : line + " · " + construction;
            return line;
        }

        public static string LastWeekTrainingConstructionSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string build = LastWeekLedgerConstructionLine(lab);
            string power = LastWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string LastWeekTrainingGpuUpkeepSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string upkeep = LastWeekLedgerGpuUpkeepLine(lab);
            string rent = LastWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(upkeep) && string.IsNullOrEmpty(rent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(upkeep))
                line = string.IsNullOrEmpty(line) ? upkeep : line + " · " + upkeep;
            if (!string.IsNullOrEmpty(rent))
                line = string.IsNullOrEmpty(line) ? rent : line + " · " + rent;
            return line;
        }

        public static string LastWeekTrainingRevenueSummaryLine(LabState lab)
        {
            string training = LastWeekLedgerTrainingSpendLine(lab);
            string revenue = LastWeekLedgerRevenueLine(lab);
            string net = LastWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LatestVendorCourtLine(GameState state, int tick)
        {
            DecisionEntry entry = NewestVendorLogEntry(state, DecisionAction.Courted);
            if (entry == null) return "";
            int weeks = tick - entry.Tick;
            if (weeks < 0) weeks = 0;
            return "Envydia courted L" + entry.Ref + " " + weeks + " wk ago";
        }

        public static string LedgerHistoryMonthsLine(LabState lab)
        {
            if (lab?.Ledger?.History == null || lab.Ledger.History.Count == 0) return "";
            return lab.Ledger.History.Count + " mo ledger history";
        }

        public static string LedgerMonthIncomeSummaryLine(LabState lab)
        {
            string grants = ThisMonthLedgerGrantsLine(lab);
            string grey = ThisMonthLedgerGreyRevenueLine(lab);
            string chips = ThisMonthLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string LedgerMonthSpendSummaryLine(LabState lab)
        {
            string power = ThisMonthLedgerPowerLine(lab);
            string salaries = ThisMonthLedgerSalariesLine(lab);
            string build = ThisMonthLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string LedgerMonthSummaryLine(LabState lab)
        {
            string revenue = ThisMonthLedgerRevenueLine(lab);
            string net = ThisMonthLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net)) return "";
            if (!string.IsNullOrEmpty(revenue) && !string.IsNullOrEmpty(net))
                return revenue + " · " + net;
            return string.IsNullOrEmpty(revenue) ? net : revenue;
        }

        public static string LedgerNetWeeksLine(LabState lab)
        {
            string last = LastWeekLedgerNetLine(lab);
            string current = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(last) && string.IsNullOrEmpty(current)) return "";
            if (!string.IsNullOrEmpty(last) && !string.IsNullOrEmpty(current))
                return last + " · " + current;
            return string.IsNullOrEmpty(last) ? current : last;
        }

        public static string LedgerStartCashLine(LabState lab)
        {
            if (lab?.Ledger == null || lab.Ledger.StartCash <= 0) return "";
            return "Started $" + lab.Ledger.StartCash.ToString("N0");
        }

        public static string LifetimeChipDevSummaryLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null) return "";
            long chipDev = lab.Ledger.Lifetime.ChipDevelopment;
            string chips = LifetimeLedgerChipSalesLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (chipDev >= 0 && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (chipDev < 0)
                line = "Lifetime chip dev $" + (-chipDev).ToString("N0");
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string LifetimeChipGreySummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LifetimeChipPowerSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string LifetimeChipTrainingSummaryLine(LabState lab)
        {
            string chips = LifetimeLedgerChipSalesLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string LifetimeFinesGreySummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LifetimeFinesPowerSummaryLine(LabState lab)
        {
            string fines = LifetimeLedgerFinesLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LifetimeGrantsLaunderSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LifetimeGrantsNetSummaryLine(LabState lab)
        {
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string LifetimeGreyNetSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LifetimeLaunderGrantsSummaryLine(LabState lab)
        {
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(laundered)) line = laundered;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string LifetimeLedgerChipSalesLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.ChipSales <= 0) return "";
            return "Lifetime chip sales $" + lab.Ledger.Lifetime.ChipSales.ToString("N0");
        }

        public static string LifetimeLedgerFinesLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Fines >= 0) return "";
            return "Lifetime fines $" + (-lab.Ledger.Lifetime.Fines).ToString("N0");
        }

        public static string LifetimeLedgerGrantsLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Grants <= 0) return "";
            return "Lifetime grants $" + lab.Ledger.Lifetime.Grants.ToString("N0");
        }

        public static string LifetimeLedgerGreyRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.GreyRevenue <= 0) return "";
            return "Lifetime grey $" + lab.Ledger.Lifetime.GreyRevenue.ToString("N0");
        }

        public static string LifetimeLedgerLaunderedLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Laundered <= 0) return "";
            return "Lifetime laundered $" + lab.Ledger.Lifetime.Laundered.ToString("N0");
        }

        public static string LifetimeLedgerNetLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null) return "";
            long net = lab.Ledger.Lifetime.Net;
            if (net == 0) return "";
            if (net > 0) return "Lifetime net +$" + net.ToString("N0");
            return "Lifetime net -$" + (-net).ToString("N0");
        }

        public static string LifetimeLedgerPowerLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Power >= 0) return "";
            return "Lifetime power $" + (-lab.Ledger.Lifetime.Power).ToString("N0");
        }

        public static string LifetimeLedgerRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Revenue <= 0) return "";
            return "Lifetime revenue $" + lab.Ledger.Lifetime.Revenue.ToString("N0");
        }

        public static string LifetimeLedgerSpendSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string LifetimeLedgerSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LifetimeLedgerTrainingLine(LabState lab)
        {
            if (lab?.Ledger?.Lifetime == null || lab.Ledger.Lifetime.Training >= 0) return "";
            return "Lifetime training $" + (-lab.Ledger.Lifetime.Training).ToString("N0");
        }

        public static string LifetimeNetTrainingSummaryLine(LabState lab)
        {
            string net = LifetimeLedgerNetLine(lab);
            string training = LifetimeLedgerTrainingLine(lab);
            string revenue = LifetimeLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string LifetimePowerFinesSummaryLine(LabState lab)
        {
            string power = LifetimeLedgerPowerLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string LifetimeRevenueChipSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(laundered))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            return line;
        }

        public static string LifetimeRevenueGrantsSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string grants = LifetimeLedgerGrantsLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LifetimeRevenuePowerSummaryLine(LabState lab)
        {
            string revenue = LifetimeLedgerRevenueLine(lab);
            string power = LifetimeLedgerPowerLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LifetimeShadyLedgerSummaryLine(LabState lab)
        {
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string fines = LifetimeLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string LifetimeTrainingChipSummaryLine(LabState lab)
        {
            string training = LifetimeLedgerTrainingLine(lab);
            string chips = LifetimeLedgerChipSalesLine(lab);
            string net = LifetimeLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string LifetimeTrainingLaunderSummaryLine(LabState lab)
        {
            string training = LifetimeLedgerTrainingLine(lab);
            string laundered = LifetimeLedgerLaunderedLine(lab);
            string grey = LifetimeLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(laundered) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(laundered))
                line = string.IsNullOrEmpty(line) ? laundered : line + " · " + laundered;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string MascotMoodSummaryLine(Simulation sim)
        {
            if (sim == null) return "";
            string stressed = StressedMascotsLine(sim);
            string healthy = HealthyMascotsLine(sim);
            if (string.IsNullOrEmpty(stressed) && string.IsNullOrEmpty(healthy)) return "";
            if (!string.IsNullOrEmpty(stressed) && !string.IsNullOrEmpty(healthy))
                return stressed + " · " + healthy;
            return string.IsNullOrEmpty(stressed) ? healthy : stressed;
        }

        public static string OpenWeightsSummaryLine(GameConfig config, LabState lab)
        {
            string released = OpenWeightsReleasedCountLine(lab);
            string appeal = OpenWeightsAppealLine(config, lab);
            if (string.IsNullOrEmpty(released) && string.IsNullOrEmpty(appeal)) return "";
            if (!string.IsNullOrEmpty(released) && !string.IsNullOrEmpty(appeal))
                return released + " · " + appeal;
            return string.IsNullOrEmpty(released) ? appeal : released;
        }

        public static string ReleaseReviewSummaryLine(LabState lab)
        {
            string open = OpenReleaseReviewsLine(lab);
            string revision = ReleaseReviewNeedsRevisionLine(lab);
            string expedited = ReleaseReviewExpeditedOpenLine(lab);
            if (string.IsNullOrEmpty(open) && string.IsNullOrEmpty(revision) && string.IsNullOrEmpty(expedited))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(open)) line = open;
            if (!string.IsNullOrEmpty(revision))
                line = string.IsNullOrEmpty(line) ? revision : line + " · " + revision;
            if (!string.IsNullOrEmpty(expedited))
                line = string.IsNullOrEmpty(line) ? expedited : line + " · " + expedited;
            return line;
        }

            if (lab == null || !lab.Bankrupt) return "";
            return "Bankrupt";
        }

        public static string ShadyExposureSummaryLine(GameConfig config, GameState state, LabState lab)
        {
            string heat = ShadyHeatLine(lab);
            string conviction = ShadyConvictionRiskLine(config, state, lab);
            string stash = ShadyStashLine(lab);
            if (string.IsNullOrEmpty(heat) && string.IsNullOrEmpty(conviction) && string.IsNullOrEmpty(stash)) return "";
            string line = "";
            if (!string.IsNullOrEmpty(heat)) line = heat;
            if (!string.IsNullOrEmpty(conviction))
                line = string.IsNullOrEmpty(line) ? conviction : line + " · " + conviction;
            if (!string.IsNullOrEmpty(stash))
                line = string.IsNullOrEmpty(line) ? stash : line + " · " + stash;
            return line;
        }

        public static string ThisMonthLedgerChipSalesLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.ChipSales <= 0) return "";
            return "This mo chip sales $" + lab.Ledger.ThisMonth.ChipSales.ToString("N0");
        }

        public static string ThisMonthLedgerConstructionLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.Construction >= 0) return "";
            return "This mo construction $" + (-lab.Ledger.ThisMonth.Construction).ToString("N0");
        }

        public static string ThisMonthLedgerGrantsLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.Grants <= 0) return "";
            return "This mo grants $" + lab.Ledger.ThisMonth.Grants.ToString("N0");
        }

        public static string ThisMonthLedgerGreyRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.GreyRevenue <= 0) return "";
            return "This mo grey $" + lab.Ledger.ThisMonth.GreyRevenue.ToString("N0");
        }

        public static string ThisMonthLedgerNetLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null) return "";
            long net = lab.Ledger.ThisMonth.Net;
            if (net == 0) return "";
            if (net > 0) return "This mo net +$" + net.ToString("N0");
            return "This mo net -$" + (-net).ToString("N0");
        }

        public static string ThisMonthLedgerPowerLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.Power >= 0) return "";
            return "This mo power $" + (-lab.Ledger.ThisMonth.Power).ToString("N0");
        }

        public static string ThisMonthLedgerRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.Revenue <= 0) return "";
            return "This mo revenue $" + lab.Ledger.ThisMonth.Revenue.ToString("N0");
        }

        public static string ThisMonthLedgerSalariesLine(LabState lab)
        {
            if (lab?.Ledger?.ThisMonth == null || lab.Ledger.ThisMonth.Salaries >= 0) return "";
            return "This mo salaries $" + (-lab.Ledger.ThisMonth.Salaries).ToString("N0");
        }

        public static string ThisWeekChipConstructionSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekChipFinesSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekChipGpuSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string ThisWeekChipRevenueSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekChipTrainingSummaryLine(LabState lab)
        {
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(chips)) line = chips;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekConstructionGrantsSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string ThisWeekConstructionNetSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string ThisWeekConstructionPowerSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string ThisWeekConstructionSalariesSummaryLine(LabState lab)
        {
            string build = ThisWeekLedgerConstructionLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(build) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(build)) line = build;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string ThisWeekFinesChipSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekFinesPowerSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string ThisWeekFinesTrainingSummaryLine(LabState lab)
        {
            string fines = ThisWeekLedgerFinesLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(fines)) line = fines;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string ThisWeekGpuConstructionSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string ThisWeekGpuFinesGreySummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string ThisWeekGpuNetSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(training))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            return line;
        }

        public static string ThisWeekGpuSalariesSummaryLine(LabState lab)
        {
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(gpuRent)) line = gpuRent;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string ThisWeekGpuTrainingSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string ThisWeekGrantNetSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekGrantsChipSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string ThisWeekGrantsFinesSummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string ThisWeekGrantsGreySummaryLine(LabState lab)
        {
            string grants = ThisWeekLedgerGrantsLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grants)) line = grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string ThisWeekGreyConstructionSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(build) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string ThisWeekGreyNetSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekGreyPowerSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(fines))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            return line;
        }

        public static string ThisWeekGreyTrainingSummaryLine(LabState lab)
        {
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            if (string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(gpuRent))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(grey)) line = grey;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            return line;
        }

        public static string ThisWeekLedgerChipSalesLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.ChipSales <= 0) return "";
            return "This wk chip sales $" + lab.Ledger.ThisWeek.ChipSales.ToString("N0");
        }

        public static string ThisWeekLedgerConstructionLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Construction >= 0) return "";
            return "This wk construction $" + (-lab.Ledger.ThisWeek.Construction).ToString("N0");
        }

        public static string ThisWeekLedgerFinesLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Fines >= 0) return "";
            return "This wk fines $" + (-lab.Ledger.ThisWeek.Fines).ToString("N0");
        }

        public static string ThisWeekLedgerGpuRentLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.GpuRent >= 0) return "";
            return "This wk GPU rent $" + (-lab.Ledger.ThisWeek.GpuRent).ToString("N0");
        }

        public static string ThisWeekLedgerGrantsLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Grants <= 0) return "";
            return "This wk grants $" + lab.Ledger.ThisWeek.Grants.ToString("N0");
        }

        public static string ThisWeekLedgerGreyRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.GreyRevenue <= 0) return "";
            return "This wk grey $" + lab.Ledger.ThisWeek.GreyRevenue.ToString("N0");
        }

        public static string ThisWeekLedgerIncomeSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string ThisWeekLedgerNetLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null) return "";
            long net = lab.Ledger.ThisWeek.Net;
            if (net == 0) return "";
            if (net > 0) return "This wk ledger net +$" + net.ToString("N0");
            return "This wk ledger net -$" + (-net).ToString("N0");
        }

        public static string ThisWeekLedgerPowerLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Power >= 0) return "";
            return "This wk power $" + (-lab.Ledger.ThisWeek.Power).ToString("N0");
        }

        public static string ThisWeekLedgerRevenueLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Revenue <= 0) return "";
            return "This wk revenue $" + lab.Ledger.ThisWeek.Revenue.ToString("N0");
        }

        public static string ThisWeekLedgerSalariesLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Salaries >= 0) return "";
            return "This wk salaries $" + (-lab.Ledger.ThisWeek.Salaries).ToString("N0");
        }

        public static string ThisWeekLedgerSpendSummaryLine(LabState lab)
        {
            string power = ThisWeekLedgerPowerLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string ThisWeekLedgerTrainingSpendLine(LabState lab)
        {
            if (lab?.Ledger?.ThisWeek == null || lab.Ledger.ThisWeek.Training >= 0) return "";
            return "This wk training $" + (-lab.Ledger.ThisWeek.Training).ToString("N0");
        }

        public static string ThisWeekNetChipSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(grants))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            return line;
        }

        public static string ThisWeekNetTrainingSummaryLine(LabState lab)
        {
            string net = ThisWeekLedgerNetLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(net) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(net)) line = net;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekPowerChipSummaryLine(LabState lab)
        {
            string power = ThisWeekLedgerPowerLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(power)) line = power;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string ThisWeekRevenueGrantsSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(grey))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            return line;
        }

        public static string ThisWeekRevenuePowerSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string salaries = ThisWeekLedgerSalariesLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(salaries))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(salaries))
                line = string.IsNullOrEmpty(line) ? salaries : line + " · " + salaries;
            return line;
        }

        public static string ThisWeekRevenueTrainingSummaryLine(LabState lab)
        {
            string revenue = ThisWeekLedgerRevenueLine(lab);
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(revenue) && string.IsNullOrEmpty(training) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(revenue)) line = revenue;
            if (!string.IsNullOrEmpty(training))
                line = string.IsNullOrEmpty(line) ? training : line + " · " + training;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string ThisWeekSalariesFinesSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string fines = ThisWeekLedgerFinesLine(lab);
            string revenue = ThisWeekLedgerRevenueLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(fines) && string.IsNullOrEmpty(revenue))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(fines))
                line = string.IsNullOrEmpty(line) ? fines : line + " · " + fines;
            if (!string.IsNullOrEmpty(revenue))
                line = string.IsNullOrEmpty(line) ? revenue : line + " · " + revenue;
            return line;
        }

        public static string ThisWeekSalariesGpuSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string gpuRent = ThisWeekLedgerGpuRentLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(gpuRent) && string.IsNullOrEmpty(chips))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(gpuRent))
                line = string.IsNullOrEmpty(line) ? gpuRent : line + " · " + gpuRent;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            return line;
        }

        public static string ThisWeekSalariesGrantsSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string grants = ThisWeekLedgerGrantsLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(grants) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(grants))
                line = string.IsNullOrEmpty(line) ? grants : line + " · " + grants;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string ThisWeekSalariesNetSummaryLine(LabState lab)
        {
            string salaries = ThisWeekLedgerSalariesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            if (string.IsNullOrEmpty(salaries) && string.IsNullOrEmpty(net) && string.IsNullOrEmpty(power))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(salaries)) line = salaries;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            return line;
        }

        public static string ThisWeekTrainingChipSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string chips = ThisWeekLedgerChipSalesLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(chips) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(chips))
                line = string.IsNullOrEmpty(line) ? chips : line + " · " + chips;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string ThisWeekTrainingGreySummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string grey = ThisWeekLedgerGreyRevenueLine(lab);
            string net = ThisWeekLedgerNetLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(grey) && string.IsNullOrEmpty(net))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(grey))
                line = string.IsNullOrEmpty(line) ? grey : line + " · " + grey;
            if (!string.IsNullOrEmpty(net))
                line = string.IsNullOrEmpty(line) ? net : line + " · " + net;
            return line;
        }

        public static string ThisWeekTrainingPowerSummaryLine(LabState lab)
        {
            string training = ThisWeekLedgerTrainingSpendLine(lab);
            string power = ThisWeekLedgerPowerLine(lab);
            string build = ThisWeekLedgerConstructionLine(lab);
            if (string.IsNullOrEmpty(training) && string.IsNullOrEmpty(power) && string.IsNullOrEmpty(build))
                return "";
            string line = "";
            if (!string.IsNullOrEmpty(training)) line = training;
            if (!string.IsNullOrEmpty(power))
                line = string.IsNullOrEmpty(line) ? power : line + " · " + power;
            if (!string.IsNullOrEmpty(build))
                line = string.IsNullOrEmpty(line) ? build : line + " · " + build;
            return line;
        }

        public static string WeeklyComputeLine(GameConfig config, GameState state, LabState lab)
        {
            if (config == null || lab == null) return "";
            long compute = LabQueries.WeeklyCompute(config, state, lab);
            if (compute <= 0) return "";
            return compute.ToString("N0") + " compute/wk";
        }

    }
}
