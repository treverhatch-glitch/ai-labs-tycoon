#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_105_108.py").read_text()
head = src[: src.index("F105_106")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F105_106)")]

DATA = r"""
F109_110 = [
    ("ThisWeekTrainingIncomeSpendSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekTrainingIncomeSpendSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeTrainingIncomeSpendSummaryLine", [
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekPowerIncomeSpendSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekPowerIncomeSpendSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimePowerIncomeSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekRentIncomeSpendSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekRentIncomeSpendSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeRentIncomeSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekConstructionIncomeSpendSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekConstructionIncomeSpendSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeConstructionIncomeSpendSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

F111_112 = [
    ("ThisWeekSalariesIncomeSpendSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekSalariesIncomeSpendSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeSalariesIncomeSpendSummaryLine", [
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekHiringIncomeSpendSummaryLine", [
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekHiringIncomeSpendSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeHiringIncomeSpendSummaryLine", [
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekLandIncomeSpendSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekLandIncomeSpendSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeLandIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekRoomIncomeSpendSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekRoomIncomeSpendSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeRoomIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

SPRINT109 = [
    ("ThisWeekTrainingIncomeSpendSummaryLine", "thisWeekTrainingIncomeSpend", "this week training income spend", "this_week_training_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_940;\n            lab.Ledger.ThisWeek.Grants = 5_090;", "2,940", "5,090"),
    ("LastWeekTrainingIncomeSpendSummaryLine", "lastWeekTrainingIncomeSpend", "last week training income spend", "last_week_training_income_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_210;\n            lab.Ledger.LastWeek.Fines = -790;", "18,210", "790"),
    ("LifetimeTrainingIncomeSpendSummaryLine", "lifetimeTrainingIncomeSpend", "lifetime training income spend", "lifetime_training_income_spend_summary_line",
     "lab.Ledger.Lifetime.Training = -158_000;\n            lab.Ledger.Lifetime.Net = 110_500;", "158,000", "110,500"),
    ("ThisWeekPowerIncomeSpendSummaryLine", "thisWeekPowerIncomeSpend", "this week power income spend", "this_week_power_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_300;\n            lab.Ledger.ThisWeek.Revenue = 18_730;", "3,300", "18,730"),
    ("LastWeekPowerIncomeSpendSummaryLine", "lastWeekPowerIncomeSpend", "last week power income spend", "last_week_power_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 3_010;\n            lab.Ledger.LastWeek.Grants = 6_080;", "3,010", "6,080"),
    ("LifetimePowerIncomeSpendSummaryLine", "lifetimePowerIncomeSpend", "lifetime power income spend", "lifetime_power_income_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -164_000;\n            lab.Ledger.Lifetime.Grants = 172_000;", "164,000", "172,000"),
]

SPRINT110 = [
    ("ThisWeekRentIncomeSpendSummaryLine", "thisWeekRentIncomeSpend", "this week rent income spend", "this_week_rent_income_spend_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_180;\n            lab.Ledger.ThisWeek.Fines = -790;", "2,180", "790"),
    ("LastWeekRentIncomeSpendSummaryLine", "lastWeekRentIncomeSpend", "last week rent income spend", "last_week_rent_income_spend_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_420;\n            lab.Ledger.LastWeek.Revenue = 18_220;", "2,420", "18,220"),
    ("LifetimeRentIncomeSpendSummaryLine", "lifetimeRentIncomeSpend", "lifetime rent income spend", "lifetime_rent_income_spend_summary_line",
     "lab.Ledger.Lifetime.Revenue = 438_000;\n            lab.Ledger.Lifetime.Net = 111_000;", "438,000", "111,000"),
    ("ThisWeekConstructionIncomeSpendSummaryLine", "thisWeekConstructionIncomeSpend", "this week construction income spend", "this_week_construction_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Construction = -9_660;\n            lab.Ledger.ThisWeek.Grants = 5_100;", "9,660", "5,100"),
    ("LastWeekConstructionIncomeSpendSummaryLine", "lastWeekConstructionIncomeSpend", "last week construction income spend", "last_week_construction_income_spend_summary_line",
     "lab.Ledger.LastWeek.Construction = -10_560;\n            lab.Ledger.LastWeek.Net = 3_020;", "10,560", "3,020"),
    ("LifetimeConstructionIncomeSpendSummaryLine", "lifetimeConstructionIncomeSpend", "lifetime construction income spend", "lifetime_construction_income_spend_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_400;\n            lab.Ledger.Lifetime.Grants = 173_000;", "84,400", "173,000"),
]

SPRINT111 = [
    ("ThisWeekSalariesIncomeSpendSummaryLine", "thisWeekSalariesIncomeSpend", "this week salaries income spend", "this_week_salaries_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_880;\n            lab.Ledger.ThisWeek.Net = 3_310;", "7,880", "3,310"),
    ("LastWeekSalariesIncomeSpendSummaryLine", "lastWeekSalariesIncomeSpend", "last week salaries income spend", "last_week_salaries_income_spend_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_100;\n            lab.Ledger.LastWeek.Revenue = 18_230;", "8,100", "18,230"),
    ("LifetimeSalariesIncomeSpendSummaryLine", "lifetimeSalariesIncomeSpend", "lifetime salaries income spend", "lifetime_salaries_income_spend_summary_line",
     "lab.Ledger.Lifetime.Salaries = -419_000;\n            lab.Ledger.Lifetime.Net = 111_500;", "419,000", "111,500"),
    ("ThisWeekHiringIncomeSpendSummaryLine", "thisWeekHiringIncomeSpend", "this week hiring income spend", "this_week_hiring_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -3_170;\n            lab.Ledger.ThisWeek.Grants = 5_110;", "3,170", "5,110"),
    ("LastWeekHiringIncomeSpendSummaryLine", "lastWeekHiringIncomeSpend", "last week hiring income spend", "last_week_hiring_income_spend_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_370;\n            lab.Ledger.LastWeek.Fines = -800;", "3,370", "800"),
    ("LifetimeHiringIncomeSpendSummaryLine", "lifetimeHiringIncomeSpend", "lifetime hiring income spend", "lifetime_hiring_income_spend_summary_line",
     "lab.Ledger.Lifetime.Hiring = -62_400;\n            lab.Ledger.Lifetime.Revenue = 439_000;", "62,400", "439,000"),
]

SPRINT112 = [
    ("ThisWeekLandIncomeSpendSummaryLine", "thisWeekLandIncomeSpend", "this week land income spend", "this_week_land_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -800;\n            lab.Ledger.ThisWeek.Revenue = 18_740;", "800", "18,740"),
    ("LastWeekLandIncomeSpendSummaryLine", "lastWeekLandIncomeSpend", "last week land income spend", "last_week_land_income_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -800;\n            lab.Ledger.LastWeek.Grants = 6_090;", "800", "6,090"),
    ("LifetimeLandIncomeSpendSummaryLine", "lifetimeLandIncomeSpend", "lifetime land income spend", "lifetime_land_income_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_500;\n            lab.Ledger.Lifetime.Grants = 174_000;", "27,500", "174,000"),
    ("ThisWeekRoomIncomeSpendSummaryLine", "thisWeekRoomIncomeSpend", "this week room income spend", "this_week_room_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_950;\n            lab.Ledger.ThisWeek.Net = 3_320;", "2,950", "3,320"),
    ("LastWeekRoomIncomeSpendSummaryLine", "lastWeekRoomIncomeSpend", "last week room income spend", "last_week_room_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 3_030;\n            lab.Ledger.LastWeek.Revenue = 18_240;", "3,030", "18,240"),
    ("LifetimeRoomIncomeSpendSummaryLine", "lifetimeRoomIncomeSpend", "lifetime room income spend", "lifetime_room_income_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 112_000;\n            lab.Ledger.Lifetime.Revenue = 440_000;", "112,000", "440,000"),
]

PA109 = '            string lifetimeDevIncomeSpendPart = string.IsNullOrEmpty(lifetimeDevIncomeSpend) ? "" : " · " + lifetimeDevIncomeSpend;'
PA110 = '            string lifetimePowerIncomeSpendPart = string.IsNullOrEmpty(lifetimePowerIncomeSpend) ? "" : " · " + lifetimePowerIncomeSpend;'
PA111 = '            string lifetimeConstructionIncomeSpendPart = string.IsNullOrEmpty(lifetimeConstructionIncomeSpend) ? "" : " · " + lifetimeConstructionIncomeSpend;'
PA112 = '            string lifetimeHiringIncomeSpendPart = string.IsNullOrEmpty(lifetimeHiringIncomeSpend) ? "" : " · " + lifetimeHiringIncomeSpend;'

"""

MAIN = """
insert_formatters(F109_110)
git_commit("feat(hud): sprint109-110 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT109, PA109, "lifetimeDevIncomeSpendPart")
run_sprint(SPRINT110, PA110, "lifetimePowerIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F111_112)
git_commit("feat(hud): sprint111-112 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT111, PA111, "lifetimeConstructionIncomeSpendPart")
run_sprint(SPRINT112, PA112, "lifetimeHiringIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_109_112.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_109_112.py", doraise=True)
print("wrote", len(out))