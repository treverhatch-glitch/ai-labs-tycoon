#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_69_72.py").read_text()
head = src[: src.index("F69_70")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F69_70)")]

DATA = r"""
F73_74 = [
    ("LastWeekDevLandSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekGrantDevSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeSpendChipSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekIncomeGrantSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeGreyPatentSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendChipSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeIncomeGrantSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekIncomeGrantSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeSpendPowerSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekDevGrantSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("ThisWeekPatentGrantSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekSpendPowerSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
]

F75_76 = [
    ("LifetimeDevPowerSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("LastWeekDevGrantSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimePatentPowerSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendGrantSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeLandPatentSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRentPatentSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRoomChipSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekPowerPatentSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekHardwarePatentSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSalariesPatentSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekConstructionPatentSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLaunderPatentSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
]

SPRINT73 = [
    ("LastWeekDevLandSummaryLine", "lastWeekDevLand", "last week dev land", "last_week_dev_land_summary_line",
     "lab.Ledger.LastWeek.Land = -12_050;\n            lab.Ledger.LastWeek.Net = 2_680;", "12,050", "2,680"),
    ("ThisWeekGrantDevSummaryLine", "thisWeekGrantDev", "this week grant dev", "this_week_grant_dev_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_480;\n            lab.Ledger.ThisWeek.ChipSales = 5_430;", "4,480", "5,430"),
    ("LifetimeSpendChipSummaryLine", "lifetimeSpendChip", "lifetime spend chip", "lifetime_spend_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 246_000;\n            lab.Ledger.Lifetime.Grants = 136_000;", "246,000", "136,000"),
    ("ThisWeekIncomeGrantSummaryLine", "thisWeekIncomeGrant", "this week income grant", "this_week_income_grant_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_500;\n            lab.Ledger.ThisWeek.Net = 3_080;", "4,500", "3,080"),
    ("LifetimeGreyPatentSummaryLine", "lifetimeGreyPatent", "lifetime grey patent", "lifetime_grey_patent_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 50_500;\n            lab.Ledger.Lifetime.Fines = -25_000;", "50,500", "25,000"),
    ("ThisWeekSpendChipSummaryLine", "thisWeekSpendChip", "this week spend chip", "this_week_spend_chip_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_450;\n            lab.Ledger.ThisWeek.Revenue = 18_300;", "5,450", "18,300"),
]

SPRINT74 = [
    ("LifetimeIncomeGrantSummaryLine", "lifetimeIncomeGrant", "lifetime income grant", "lifetime_income_grant_summary_line",
     "lab.Ledger.Lifetime.Revenue = 414_000;\n            lab.Ledger.Lifetime.Grants = 137_000;", "414,000", "137,000"),
    ("LastWeekIncomeGrantSummaryLine", "lastWeekIncomeGrant", "last week income grant", "last_week_income_grant_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_500;\n            lab.Ledger.LastWeek.CapitalIn = 42_800;", "5,500", "42,800"),
    ("LifetimeSpendPowerSummaryLine", "lifetimeSpendPower", "lifetime spend power", "lifetime_spend_power_summary_line",
     "lab.Ledger.Lifetime.Power = -152_000;\n            lab.Ledger.Lifetime.Fines = -25_200;", "152,000", "25,200"),
    ("ThisWeekDevGrantSummaryLine", "thisWeekDevGrant", "this week dev grant", "this_week_dev_grant_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_460;\n            lab.Ledger.ThisWeek.Grants = 4_520;", "5,460", "4,520"),
    ("ThisWeekPatentGrantSummaryLine", "thisWeekPatentGrant", "this week patent grant", "this_week_patent_grant_summary_line",
     "lab.Ledger.ThisWeek.Fines = -550;\n            lab.Ledger.ThisWeek.Grants = 4_540;", "550", "4,540"),
    ("LastWeekSpendPowerSummaryLine", "lastWeekSpendPower", "last week spend power", "last_week_spend_power_summary_line",
     "lab.Ledger.LastWeek.Power = -3_580;\n            lab.Ledger.LastWeek.Net = 2_720;", "3,580", "2,720"),
]

SPRINT75 = [
    ("LifetimeDevPowerSummaryLine", "lifetimeDevPower", "lifetime dev power", "lifetime_dev_power_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 247_000;\n            lab.Ledger.Lifetime.Power = -153_000;", "247,000", "153,000"),
    ("LastWeekDevGrantSummaryLine", "lastWeekDevGrant", "last week dev grant", "last_week_dev_grant_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_520;\n            lab.Ledger.LastWeek.Revenue = 17_850;", "5,520", "17,850"),
    ("LifetimePatentPowerSummaryLine", "lifetimePatentPower", "lifetime patent power", "lifetime_patent_power_summary_line",
     "lab.Ledger.Lifetime.Fines = -25_400;\n            lab.Ledger.Lifetime.Power = -154_000;", "25,400", "154,000"),
    ("ThisWeekSpendGrantSummaryLine", "thisWeekSpendGrant", "this week spend grant", "this_week_spend_grant_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_560;\n            lab.Ledger.ThisWeek.Net = 3_090;", "4,560", "3,090"),
    ("LifetimeLandPatentSummaryLine", "lifetimeLandPatent", "lifetime land patent", "lifetime_land_patent_summary_line",
     "lab.Ledger.Lifetime.Grants = 138_000;\n            lab.Ledger.Lifetime.Fines = -25_500;", "138,000", "25,500"),
    ("LastWeekRentPatentSummaryLine", "lastWeekRentPatent", "last week rent patent", "last_week_rent_patent_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_380;\n            lab.Ledger.LastWeek.Grants = 5_540;", "2,380", "5,540"),
]

SPRINT76 = [
    ("ThisWeekRoomChipSummaryLine", "thisWeekRoomChip", "this week room chip", "this_week_room_chip_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_470;\n            lab.Ledger.ThisWeek.Grants = 4_580;", "5,470", "4,580"),
    ("LastWeekPowerPatentSummaryLine", "lastWeekPowerPatent", "last week power patent", "last_week_power_patent_summary_line",
     "lab.Ledger.LastWeek.Power = -3_600;\n            lab.Ledger.LastWeek.Fines = -620;", "3,600", "620"),
    ("ThisWeekHardwarePatentSummaryLine", "thisWeekHardwarePatent", "this week hardware patent", "this_week_hardware_patent_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_120;\n            lab.Ledger.ThisWeek.Fines = -560;", "6,120", "560"),
    ("LastWeekSalariesPatentSummaryLine", "lastWeekSalariesPatent", "last week salaries patent", "last_week_salaries_patent_summary_line",
     "lab.Ledger.LastWeek.Salaries = -7_800;\n            lab.Ledger.LastWeek.Grants = 5_560;", "7,800", "5,560"),
    ("ThisWeekConstructionPatentSummaryLine", "thisWeekConstructionPatent", "this week construction patent", "this_week_construction_patent_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_130;\n            lab.Ledger.ThisWeek.Grants = 4_600;", "6,130", "4,600"),
    ("LastWeekLaunderPatentSummaryLine", "lastWeekLaunderPatent", "last week launder patent", "last_week_launder_patent_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_300;\n            lab.Ledger.LastWeek.GreyRevenue = 2_500;", "3,300", "2,500"),
]

PA73 = '            string thisWeekHiringChipPart = string.IsNullOrEmpty(thisWeekHiringChip) ? "" : " · " + thisWeekHiringChip;'
PA74 = '            string thisWeekSpendChipPart = string.IsNullOrEmpty(thisWeekSpendChip) ? "" : " · " + thisWeekSpendChip;'
PA75 = '            string lastWeekSpendPowerPart = string.IsNullOrEmpty(lastWeekSpendPower) ? "" : " · " + lastWeekSpendPower;'
PA76 = '            string lastWeekRentPatentPart = string.IsNullOrEmpty(lastWeekRentPatent) ? "" : " · " + lastWeekRentPatent;'

"""

MAIN = """
insert_formatters(F73_74)
git_commit("feat(hud): sprint73-74 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT73, PA73, "thisWeekHiringChipPart")
run_sprint(SPRINT74, PA74, "thisWeekSpendChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F75_76)
git_commit("feat(hud): sprint75-76 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT75, PA75, "lastWeekSpendPowerPart")
run_sprint(SPRINT76, PA76, "lastWeekRentPatentPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_73_76.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_73_76.py", doraise=True)
print("wrote", len(out))