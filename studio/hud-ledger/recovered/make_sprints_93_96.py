#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_89_92.py").read_text()
head = src[: src.index("F89_90")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F89_90)")]

DATA = r"""
F93_94 = [
    ("ThisWeekPatentIncomeSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekPatentIncomeSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimePatentIncomeSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekUpkeepIncomeSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekUpkeepIncomeSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeUpkeepIncomeSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHiringIncomeSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringIncomeSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHiringIncomeSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekNetSpendSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekNetSpendSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeNetSpendSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

F95_96 = [
    ("ThisWeekGreySpendSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekGreySpendSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeGreySpendSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekFinesSpendSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekFinesSpendSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeFinesSpendSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRentSpendSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRentSpendSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRentSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekGrantSpendSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantSpendSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGrantSpendSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT93 = [
    ("ThisWeekPatentIncomeSummaryLine", "thisWeekPatentIncome", "this week patent income", "this_week_patent_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -670;\n            lab.Ledger.ThisWeek.Grants = 4_920;", "670", "4,920"),
    ("LastWeekPatentIncomeSummaryLine", "lastWeekPatentIncome", "last week patent income", "last_week_patent_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_940;\n            lab.Ledger.LastWeek.Fines = -670;", "5,940", "670"),
    ("LifetimePatentIncomeSummaryLine", "lifetimePatentIncome", "lifetime patent income", "lifetime_patent_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_400;\n            lab.Ledger.Lifetime.Grants = 158_000;", "26,400", "158,000"),
    ("ThisWeekUpkeepIncomeSummaryLine", "thisWeekUpkeepIncome", "this week upkeep income", "this_week_upkeep_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_160;\n            lab.Ledger.ThisWeek.Net = 3_180;", "2,160", "3,180"),
    ("LastWeekUpkeepIncomeSummaryLine", "lastWeekUpkeepIncome", "last week upkeep income", "last_week_upkeep_income_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -2_020;\n            lab.Ledger.LastWeek.Revenue = 18_060;", "2,020", "18,060"),
    ("LifetimeUpkeepIncomeSummaryLine", "lifetimeUpkeepIncome", "lifetime upkeep income", "lifetime_upkeep_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 428_000;\n            lab.Ledger.Lifetime.Net = 101_000;", "428,000", "101,000"),
]

SPRINT94 = [
    ("ThisWeekHiringIncomeSummaryLine", "thisWeekHiringIncome", "this week hiring income", "this_week_hiring_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_860;\n            lab.Ledger.ThisWeek.Grants = 4_940;", "7,860", "4,940"),
    ("LastWeekHiringIncomeSummaryLine", "lastWeekHiringIncome", "last week hiring income", "last_week_hiring_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_150;\n            lab.Ledger.LastWeek.Grants = 5_960;", "3,150", "5,960"),
    ("LifetimeHiringIncomeSummaryLine", "lifetimeHiringIncome", "lifetime hiring income", "lifetime_hiring_income_summary_line",
     "lab.Ledger.Lifetime.Training = -157_000;\n            lab.Ledger.Lifetime.Grants = 159_000;", "157,000", "159,000"),
    ("ThisWeekNetSpendSummaryLine", "thisWeekNetSpend", "this week net spend", "this_week_net_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_190;\n            lab.Ledger.ThisWeek.Power = -2_850;", "3,190", "2,850"),
    ("LastWeekNetSpendSummaryLine", "lastWeekNetSpend", "last week net spend", "last_week_net_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 2_860;\n            lab.Ledger.LastWeek.Fines = -680;", "2,860", "680"),
    ("LifetimeNetSpendSummaryLine", "lifetimeNetSpend", "lifetime net spend", "lifetime_net_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 102_000;\n            lab.Ledger.Lifetime.Fines = -26_500;", "102,000", "26,500"),
]

SPRINT95 = [
    ("ThisWeekGreySpendSummaryLine", "thisWeekGreySpend", "this week grey spend", "this_week_grey_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_060;\n            lab.Ledger.ThisWeek.Fines = -680;", "2,060", "680"),
    ("LastWeekGreySpendSummaryLine", "lastWeekGreySpend", "last week grey spend", "last_week_grey_spend_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_580;\n            lab.Ledger.LastWeek.Laundered = 3_400;", "2,580", "3,400"),
    ("LifetimeGreySpendSummaryLine", "lifetimeGreySpend", "lifetime grey spend", "lifetime_grey_spend_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_800;\n            lab.Ledger.Lifetime.Laundered = 32_500;", "52,800", "32,500"),
    ("ThisWeekFinesSpendSummaryLine", "thisWeekFinesSpend", "this week fines spend", "this_week_fines_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -690;\n            lab.Ledger.ThisWeek.Net = 3_200;", "690", "3,200"),
    ("LastWeekFinesSpendSummaryLine", "lastWeekFinesSpend", "last week fines spend", "last_week_fines_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -690;\n            lab.Ledger.LastWeek.Net = 2_870;", "690", "2,870"),
    ("LifetimeFinesSpendSummaryLine", "lifetimeFinesSpend", "lifetime fines spend", "lifetime_fines_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_600;\n            lab.Ledger.Lifetime.Grants = 160_000;", "26,600", "160,000"),
]

SPRINT96 = [
    ("ThisWeekRentSpendSummaryLine", "thisWeekRentSpend", "this week rent spend", "this_week_rent_spend_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_170;\n            lab.Ledger.ThisWeek.Revenue = 18_650;", "2,170", "18,650"),
    ("LastWeekRentSpendSummaryLine", "lastWeekRentSpend", "last week rent spend", "last_week_rent_spend_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_410;\n            lab.Ledger.LastWeek.Grants = 5_980;", "2,410", "5,980"),
    ("LifetimeRentSpendSummaryLine", "lifetimeRentSpend", "lifetime rent spend", "lifetime_rent_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -162_000;\n            lab.Ledger.Lifetime.Revenue = 429_000;", "162,000", "429,000"),
    ("ThisWeekGrantSpendSummaryLine", "thisWeekGrantSpend", "this week grant spend", "this_week_grant_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_960;\n            lab.Ledger.ThisWeek.Net = 3_210;", "4,960", "3,210"),
    ("LastWeekGrantSpendSummaryLine", "lastWeekGrantSpend", "last week grant spend", "last_week_grant_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_000;\n            lab.Ledger.LastWeek.Revenue = 18_080;", "6,000", "18,080"),
    ("LifetimeGrantSpendSummaryLine", "lifetimeGrantSpend", "lifetime grant spend", "lifetime_grant_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 161_000;\n            lab.Ledger.Lifetime.Net = 103_000;", "161,000", "103,000"),
]

PA93 = '            string lifetimeRoomIncomePart = string.IsNullOrEmpty(lifetimeRoomIncome) ? "" : " · " + lifetimeRoomIncome;'
PA94 = '            string lifetimeUpkeepIncomePart = string.IsNullOrEmpty(lifetimeUpkeepIncome) ? "" : " · " + lifetimeUpkeepIncome;'
PA95 = '            string lifetimeNetSpendPart = string.IsNullOrEmpty(lifetimeNetSpend) ? "" : " · " + lifetimeNetSpend;'
PA96 = '            string lifetimeFinesSpendPart = string.IsNullOrEmpty(lifetimeFinesSpend) ? "" : " · " + lifetimeFinesSpend;'

"""

MAIN = """
insert_formatters(F93_94)
git_commit("feat(hud): sprint93-94 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT93, PA93, "lifetimeRoomIncomePart")
run_sprint(SPRINT94, PA94, "lifetimeUpkeepIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F95_96)
git_commit("feat(hud): sprint95-96 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT95, PA95, "lifetimeNetSpendPart")
run_sprint(SPRINT96, PA96, "lifetimeFinesSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_93_96.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_93_96.py", doraise=True)
print("wrote", len(out))