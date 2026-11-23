#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_109_112.py").read_text()
head = src[: src.index("F109_110")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F109_110)")]

DATA = r"""
F113_114 = [
    ("ThisWeekTrainingSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekTrainingSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeTrainingSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
    ]),
    ("ThisWeekPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimePowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekRentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekRentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LifetimeRentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekConstructionSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekConstructionSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LifetimeConstructionSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
    ]),
]

F115_116 = [
    ("ThisWeekSalariesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekSalariesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeSalariesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekHiringSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
    ]),
    ("LastWeekHiringSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("LifetimeHiringSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekLandSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
    ]),
    ("LastWeekLandSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("LifetimeLandSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRoomSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LastWeekRoomSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LifetimeRoomSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT113 = [
    ("ThisWeekTrainingSpendIncomeSummaryLine", "thisWeekTrainingSpendIncome", "this week training spend income", "this_week_training_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_120;\n            lab.Ledger.ThisWeek.Fines = -810;", "5,120", "810"),
    ("LastWeekTrainingSpendIncomeSummaryLine", "lastWeekTrainingSpendIncome", "last week training spend income", "last_week_training_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_250;\n            lab.Ledger.LastWeek.Net = 3_040;", "18,250", "3,040"),
    ("LifetimeTrainingSpendIncomeSummaryLine", "lifetimeTrainingSpendIncome", "lifetime training spend income", "lifetime_training_spend_income_summary_line",
     "lab.Ledger.Lifetime.Training = -159_000;\n            lab.Ledger.Lifetime.Grants = 175_000;", "159,000", "175,000"),
    ("ThisWeekPowerSpendIncomeSummaryLine", "thisWeekPowerSpendIncome", "this week power spend income", "this_week_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_960;\n            lab.Ledger.ThisWeek.Net = 3_330;", "2,960", "3,330"),
    ("LastWeekPowerSpendIncomeSummaryLine", "lastWeekPowerSpendIncome", "last week power spend income", "last_week_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.Power = -2_430;\n            lab.Ledger.LastWeek.Grants = 6_100;", "2,430", "6,100"),
    ("LifetimePowerSpendIncomeSummaryLine", "lifetimePowerSpendIncome", "lifetime power spend income", "lifetime_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -165_000;\n            lab.Ledger.Lifetime.Net = 112_500;", "165,000", "112,500"),
]

SPRINT114 = [
    ("ThisWeekRentSpendIncomeSummaryLine", "thisWeekRentSpendIncome", "this week rent spend income", "this_week_rent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_190;\n            lab.Ledger.ThisWeek.Revenue = 18_750;", "2,190", "18,750"),
    ("LastWeekRentSpendIncomeSummaryLine", "lastWeekRentSpendIncome", "last week rent spend income", "last_week_rent_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_430;\n            lab.Ledger.LastWeek.Fines = -810;", "2,430", "810"),
    ("LifetimeRentSpendIncomeSummaryLine", "lifetimeRentSpendIncome", "lifetime rent spend income", "lifetime_rent_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 441_000;\n            lab.Ledger.Lifetime.Fines = -27_600;", "441,000", "27,600"),
    ("ThisWeekConstructionSpendIncomeSummaryLine", "thisWeekConstructionSpendIncome", "this week construction spend income", "this_week_construction_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -9_670;\n            lab.Ledger.ThisWeek.Grants = 5_130;", "9,670", "5,130"),
    ("LastWeekConstructionSpendIncomeSummaryLine", "lastWeekConstructionSpendIncome", "last week construction spend income", "last_week_construction_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -10_570;\n            lab.Ledger.LastWeek.Revenue = 18_260;", "10,570", "18,260"),
    ("LifetimeConstructionSpendIncomeSummaryLine", "lifetimeConstructionSpendIncome", "lifetime construction spend income", "lifetime_construction_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_500;\n            lab.Ledger.Lifetime.Grants = 176_000;", "84,500", "176,000"),
]

SPRINT115 = [
    ("ThisWeekSalariesSpendIncomeSummaryLine", "thisWeekSalariesSpendIncome", "this week salaries spend income", "this_week_salaries_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_890;\n            lab.Ledger.ThisWeek.Grants = 5_140;", "7,890", "5,140"),
    ("LastWeekSalariesSpendIncomeSummaryLine", "lastWeekSalariesSpendIncome", "last week salaries spend income", "last_week_salaries_spend_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_110;\n            lab.Ledger.LastWeek.Net = 3_050;", "8,110", "3,050"),
    ("LifetimeSalariesSpendIncomeSummaryLine", "lifetimeSalariesSpendIncome", "lifetime salaries spend income", "lifetime_salaries_spend_income_summary_line",
     "lab.Ledger.Lifetime.Salaries = -420_000;\n            lab.Ledger.Lifetime.Net = 113_000;", "420,000", "113,000"),
    ("ThisWeekHiringSpendIncomeSummaryLine", "thisWeekHiringSpendIncome", "this week hiring spend income", "this_week_hiring_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -3_180;\n            lab.Ledger.ThisWeek.Revenue = 18_760;", "3,180", "18,760"),
    ("LastWeekHiringSpendIncomeSummaryLine", "lastWeekHiringSpendIncome", "last week hiring spend income", "last_week_hiring_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_380;\n            lab.Ledger.LastWeek.Grants = 6_110;", "3,380", "6,110"),
    ("LifetimeHiringSpendIncomeSummaryLine", "lifetimeHiringSpendIncome", "lifetime hiring spend income", "lifetime_hiring_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hiring = -62_500;\n            lab.Ledger.Lifetime.Grants = 177_000;", "62,500", "177,000"),
]

SPRINT116 = [
    ("ThisWeekLandSpendIncomeSummaryLine", "thisWeekLandSpendIncome", "this week land spend income", "this_week_land_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -820;\n            lab.Ledger.ThisWeek.Net = 3_340;", "820", "3,340"),
    ("LastWeekLandSpendIncomeSummaryLine", "lastWeekLandSpendIncome", "last week land spend income", "last_week_land_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -820;\n            lab.Ledger.LastWeek.Revenue = 18_270;", "820", "18,270"),
    ("LifetimeLandSpendIncomeSummaryLine", "lifetimeLandSpendIncome", "lifetime land spend income", "lifetime_land_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_700;\n            lab.Ledger.Lifetime.Net = 113_500;", "27,700", "113,500"),
    ("ThisWeekRoomSpendIncomeSummaryLine", "thisWeekRoomSpendIncome", "this week room spend income", "this_week_room_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_970;\n            lab.Ledger.ThisWeek.Grants = 5_150;", "2,970", "5,150"),
    ("LastWeekRoomSpendIncomeSummaryLine", "lastWeekRoomSpendIncome", "last week room spend income", "last_week_room_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_060;\n            lab.Ledger.LastWeek.Fines = -830;", "3,060", "830"),
    ("LifetimeRoomSpendIncomeSummaryLine", "lifetimeRoomSpendIncome", "lifetime room spend income", "lifetime_room_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 442_000;\n            lab.Ledger.Lifetime.Grants = 178_000;", "442,000", "178,000"),
]

PA113 = '            string lifetimeRoomIncomeSpendPart = string.IsNullOrEmpty(lifetimeRoomIncomeSpend) ? "" : " · " + lifetimeRoomIncomeSpend;'
PA114 = '            string lifetimePowerSpendIncomePart = string.IsNullOrEmpty(lifetimePowerSpendIncome) ? "" : " · " + lifetimePowerSpendIncome;'
PA115 = '            string lifetimeConstructionSpendIncomePart = string.IsNullOrEmpty(lifetimeConstructionSpendIncome) ? "" : " · " + lifetimeConstructionSpendIncome;'
PA116 = '            string lifetimeHiringSpendIncomePart = string.IsNullOrEmpty(lifetimeHiringSpendIncome) ? "" : " · " + lifetimeHiringSpendIncome;'

"""

MAIN = """
insert_formatters(F113_114)
git_commit("feat(hud): sprint113-114 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT113, PA113, "lifetimeRoomIncomeSpendPart")
run_sprint(SPRINT114, PA114, "lifetimePowerSpendIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F115_116)
git_commit("feat(hud): sprint115-116 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT115, PA115, "lifetimeConstructionSpendIncomePart")
run_sprint(SPRINT116, PA116, "lifetimeHiringSpendIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_113_116.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_113_116.py", doraise=True)
print("wrote", len(out))