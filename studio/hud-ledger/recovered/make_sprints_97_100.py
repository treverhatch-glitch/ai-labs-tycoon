#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_93_96.py").read_text()
head = src[: src.index("F93_94")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F93_94)")]

DATA = r"""
F97_98 = [
    ("ThisWeekChipSpendSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekChipSpendSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeChipSpendSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekDevSpendSummaryLine", [
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekDevSpendSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeDevSpendSummaryLine", [
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekTrainingSpendSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekTrainingSpendSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeTrainingSpendSummaryLine", [
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekSalariesSpendSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSalariesSpendSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeSalariesSpendSummaryLine", [
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
]

F99_100 = [
    ("ThisWeekPowerSpendSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekPowerSpendSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimePowerSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRevenueSpendSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRevenueSpendSummaryLine", [
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeRevenueSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekConstructionSpendSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekConstructionSpendSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeConstructionSpendSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHiringSpendSummaryLine", [
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringSpendSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeHiringSpendSummaryLine", [
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
]

SPRINT97 = [
    ("ThisWeekChipSpendSummaryLine", "thisWeekChipSpend", "this week chip spend", "this_week_chip_spend_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_660;\n            lab.Ledger.ThisWeek.Fines = -700;", "18,660", "700"),
    ("LastWeekChipSpendSummaryLine", "lastWeekChipSpend", "last week chip spend", "last_week_chip_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_010;\n            lab.Ledger.LastWeek.Revenue = 18_090;", "6,010", "18,090"),
    ("LifetimeChipSpendSummaryLine", "lifetimeChipSpend", "lifetime chip spend", "lifetime_chip_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 103_500;\n            lab.Ledger.Lifetime.Fines = -26_700;", "103,500", "26,700"),
    ("ThisWeekDevSpendSummaryLine", "thisWeekDevSpend", "this week dev spend", "this_week_dev_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_220;\n            lab.Ledger.ThisWeek.Power = -2_860;", "3,220", "2,860"),
    ("LastWeekDevSpendSummaryLine", "lastWeekDevSpend", "last week dev spend", "last_week_dev_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -700;\n            lab.Ledger.LastWeek.Net = 2_880;", "700", "2,880"),
    ("LifetimeDevSpendSummaryLine", "lifetimeDevSpend", "lifetime dev spend", "lifetime_dev_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 162_000;\n            lab.Ledger.Lifetime.Net = 104_000;", "162,000", "104,000"),
]

SPRINT98 = [
    ("ThisWeekTrainingSpendSummaryLine", "thisWeekTrainingSpend", "this week training spend", "this_week_training_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_870;\n            lab.Ledger.ThisWeek.Grants = 4_970;", "2,870", "4,970"),
    ("LastWeekTrainingSpendSummaryLine", "lastWeekTrainingSpend", "last week training spend", "last_week_training_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_100;\n            lab.Ledger.LastWeek.Fines = -710;", "18,100", "710"),
    ("LifetimeTrainingSpendSummaryLine", "lifetimeTrainingSpend", "lifetime training spend", "lifetime_training_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 104_500;\n            lab.Ledger.Lifetime.Revenue = 430_000;", "104,500", "430,000"),
    ("ThisWeekSalariesSpendSummaryLine", "thisWeekSalariesSpend", "this week salaries spend", "this_week_salaries_spend_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_870;\n            lab.Ledger.ThisWeek.Grants = 4_980;", "7,870", "4,980"),
    ("LastWeekSalariesSpendSummaryLine", "lastWeekSalariesSpend", "last week salaries spend", "last_week_salaries_spend_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_090;\n            lab.Ledger.LastWeek.Net = 2_890;", "8,090", "2,890"),
    ("LifetimeSalariesSpendSummaryLine", "lifetimeSalariesSpend", "lifetime salaries spend", "lifetime_salaries_spend_summary_line",
     "lab.Ledger.Lifetime.Salaries = -418_000;\n            lab.Ledger.Lifetime.Grants = 163_000;", "418,000", "163,000"),
]

SPRINT99 = [
    ("ThisWeekPowerSpendSummaryLine", "thisWeekPowerSpend", "this week power spend", "this_week_power_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_880;\n            lab.Ledger.ThisWeek.Net = 3_230;", "2,880", "3,230"),
    ("LastWeekPowerSpendSummaryLine", "lastWeekPowerSpend", "last week power spend", "last_week_power_spend_summary_line",
     "lab.Ledger.LastWeek.Power = -2_420;\n            lab.Ledger.LastWeek.Fines = -720;", "2,420", "720"),
    ("LifetimePowerSpendSummaryLine", "lifetimePowerSpend", "lifetime power spend", "lifetime_power_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -163_500;\n            lab.Ledger.Lifetime.Net = 105_000;", "163,500", "105,000"),
    ("ThisWeekRevenueSpendSummaryLine", "thisWeekRevenueSpend", "this week revenue spend", "this_week_revenue_spend_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_670;\n            lab.Ledger.ThisWeek.Grants = 4_990;", "18,670", "4,990"),
    ("LastWeekRevenueSpendSummaryLine", "lastWeekRevenueSpend", "last week revenue spend", "last_week_revenue_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_110;\n            lab.Ledger.LastWeek.Net = 2_900;", "18,110", "2,900"),
    ("LifetimeRevenueSpendSummaryLine", "lifetimeRevenueSpend", "lifetime revenue spend", "lifetime_revenue_spend_summary_line",
     "lab.Ledger.Lifetime.Revenue = 431_000;\n            lab.Ledger.Lifetime.Fines = -26_800;", "431,000", "26,800"),
]

SPRINT100 = [
    ("ThisWeekConstructionSpendSummaryLine", "thisWeekConstructionSpend", "this week construction spend", "this_week_construction_spend_summary_line",
     "lab.Ledger.ThisWeek.Construction = -9_640;\n            lab.Ledger.ThisWeek.Power = -2_890;", "9,640", "2,890"),
    ("LastWeekConstructionSpendSummaryLine", "lastWeekConstructionSpend", "last week construction spend", "last_week_construction_spend_summary_line",
     "lab.Ledger.LastWeek.Construction = -10_540;\n            lab.Ledger.LastWeek.Revenue = 18_120;", "10,540", "18,120"),
    ("LifetimeConstructionSpendSummaryLine", "lifetimeConstructionSpend", "lifetime construction spend", "lifetime_construction_spend_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_200;\n            lab.Ledger.Lifetime.Net = 105_500;", "84,200", "105,500"),
    ("ThisWeekHiringSpendSummaryLine", "thisWeekHiringSpend", "this week hiring spend", "this_week_hiring_spend_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -3_160;\n            lab.Ledger.ThisWeek.Grants = 5_000;", "3,160", "5,000"),
    ("LastWeekHiringSpendSummaryLine", "lastWeekHiringSpend", "last week hiring spend", "last_week_hiring_spend_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_360;\n            lab.Ledger.LastWeek.Net = 2_910;", "3,360", "2,910"),
    ("LifetimeHiringSpendSummaryLine", "lifetimeHiringSpend", "lifetime hiring spend", "lifetime_hiring_spend_summary_line",
     "lab.Ledger.Lifetime.Hiring = -62_300;\n            lab.Ledger.Lifetime.Revenue = 432_000;", "62,300", "432,000"),
]

PA97 = '            string lifetimeGrantSpendPart = string.IsNullOrEmpty(lifetimeGrantSpend) ? "" : " · " + lifetimeGrantSpend;'
PA98 = '            string lifetimeDevSpendPart = string.IsNullOrEmpty(lifetimeDevSpend) ? "" : " · " + lifetimeDevSpend;'
PA99 = '            string lifetimeSalariesSpendPart = string.IsNullOrEmpty(lifetimeSalariesSpend) ? "" : " · " + lifetimeSalariesSpend;'
PA100 = '            string lifetimeRevenueSpendPart = string.IsNullOrEmpty(lifetimeRevenueSpend) ? "" : " · " + lifetimeRevenueSpend;'

"""

MAIN = """
insert_formatters(F97_98)
git_commit("feat(hud): sprint97-98 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT97, PA97, "lifetimeGrantSpendPart")
run_sprint(SPRINT98, PA98, "lifetimeChipSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F99_100)
git_commit("feat(hud): sprint99-100 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT99, PA99, "lifetimeDevSpendPart")
run_sprint(SPRINT100, PA100, "lifetimeSalariesSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_97_100.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_97_100.py", doraise=True)
print("wrote", len(out))