#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_113_116.py").read_text()
head = src[: src.index("F113_114")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F113_114)")]

DATA = r"""
F117_118 = [
    ("ThisWeekSiteIncomeSpendSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekSiteIncomeSpendSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeSiteIncomeSpendSummaryLine", [
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekHardwareIncomeSpendSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekHardwareIncomeSpendSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeHardwareIncomeSpendSummaryLine", [
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekLaunderIncomeSpendSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekLaunderIncomeSpendSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeLaunderIncomeSpendSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekCapitalIncomeSpendSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekCapitalIncomeSpendSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeCapitalIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

F119_120 = [
    ("ThisWeekPatentIncomeSpendSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekPatentIncomeSpendSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimePatentIncomeSpendSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekUpkeepIncomeSpendSummaryLine", [
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekUpkeepIncomeSpendSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeUpkeepIncomeSpendSummaryLine", [
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekShadyIncomeSpendSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekShadyIncomeSpendSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeShadyIncomeSpendSummaryLine", [
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekHardwareSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
    ]),
    ("LastWeekHardwareSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
    ]),
    ("LifetimeHardwareSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

SPRINT117 = [
    ("ThisWeekSiteIncomeSpendSummaryLine", "thisWeekSiteIncomeSpend", "this week site income spend", "this_week_site_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_160;\n            lab.Ledger.ThisWeek.Net = 3_350;", "5,160", "3,350"),
    ("LastWeekSiteIncomeSpendSummaryLine", "lastWeekSiteIncomeSpend", "last week site income spend", "last_week_site_income_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_280;\n            lab.Ledger.LastWeek.Fines = -840;", "18,280", "840"),
    ("LifetimeSiteIncomeSpendSummaryLine", "lifetimeSiteIncomeSpend", "lifetime site income spend", "lifetime_site_income_spend_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_600;\n            lab.Ledger.Lifetime.Net = 114_000;", "84,600", "114,000"),
    ("ThisWeekHardwareIncomeSpendSummaryLine", "thisWeekHardwareIncomeSpend", "this week hardware income spend", "this_week_hardware_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_770;\n            lab.Ledger.ThisWeek.Power = -2_980;", "18,770", "2,980"),
    ("LastWeekHardwareIncomeSpendSummaryLine", "lastWeekHardwareIncomeSpend", "last week hardware income spend", "last_week_hardware_income_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_120;\n            lab.Ledger.LastWeek.Net = 3_070;", "6,120", "3,070"),
    ("LifetimeHardwareIncomeSpendSummaryLine", "lifetimeHardwareIncomeSpend", "lifetime hardware income spend", "lifetime_hardware_income_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_800;\n            lab.Ledger.Lifetime.Grants = 179_000;", "27,800", "179,000"),
]

SPRINT118 = [
    ("ThisWeekLaunderIncomeSpendSummaryLine", "thisWeekLaunderIncomeSpend", "this week launder income spend", "this_week_launder_income_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_120;\n            lab.Ledger.ThisWeek.Fines = -840;", "2,120", "840"),
    ("LastWeekLaunderIncomeSpendSummaryLine", "lastWeekLaunderIncomeSpend", "last week launder income spend", "last_week_launder_income_spend_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_460;\n            lab.Ledger.LastWeek.Revenue = 18_290;", "3,460", "18,290"),
    ("LifetimeLaunderIncomeSpendSummaryLine", "lifetimeLaunderIncomeSpend", "lifetime launder income spend", "lifetime_launder_income_spend_summary_line",
     "lab.Ledger.Lifetime.Laundered = 32_800;\n            lab.Ledger.Lifetime.Net = 114_500;", "32,800", "114,500"),
    ("ThisWeekCapitalIncomeSpendSummaryLine", "thisWeekCapitalIncomeSpend", "this week capital income spend", "this_week_capital_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_170;\n            lab.Ledger.ThisWeek.Revenue = 18_780;", "5,170", "18,780"),
    ("LastWeekCapitalIncomeSpendSummaryLine", "lastWeekCapitalIncomeSpend", "last week capital income spend", "last_week_capital_income_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_130;\n            lab.Ledger.LastWeek.Fines = -850;", "6,130", "850"),
    ("LifetimeCapitalIncomeSpendSummaryLine", "lifetimeCapitalIncomeSpend", "lifetime capital income spend", "lifetime_capital_income_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 180_000;\n            lab.Ledger.Lifetime.Revenue = 443_000;", "180,000", "443,000"),
]

SPRINT119 = [
    ("ThisWeekPatentIncomeSpendSummaryLine", "thisWeekPatentIncomeSpend", "this week patent income spend", "this_week_patent_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -850;\n            lab.Ledger.ThisWeek.Net = 3_360;", "850", "3,360"),
    ("LastWeekPatentIncomeSpendSummaryLine", "lastWeekPatentIncomeSpend", "last week patent income spend", "last_week_patent_income_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -850;\n            lab.Ledger.LastWeek.Net = 3_080;", "850", "3,080"),
    ("LifetimePatentIncomeSpendSummaryLine", "lifetimePatentIncomeSpend", "lifetime patent income spend", "lifetime_patent_income_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_900;\n            lab.Ledger.Lifetime.Net = 115_000;", "27,900", "115,000"),
    ("ThisWeekUpkeepIncomeSpendSummaryLine", "thisWeekUpkeepIncomeSpend", "this week upkeep income spend", "this_week_upkeep_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_990;\n            lab.Ledger.ThisWeek.Grants = 5_180;", "2,990", "5,180"),
    ("LastWeekUpkeepIncomeSpendSummaryLine", "lastWeekUpkeepIncomeSpend", "last week upkeep income spend", "last_week_upkeep_income_spend_summary_line",
     "lab.Ledger.LastWeek.Power = -2_440;\n            lab.Ledger.LastWeek.Revenue = 18_300;", "2,440", "18,300"),
    ("LifetimeUpkeepIncomeSpendSummaryLine", "lifetimeUpkeepIncomeSpend", "lifetime upkeep income spend", "lifetime_upkeep_income_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -166_000;\n            lab.Ledger.Lifetime.Grants = 181_000;", "166,000", "181,000"),
]

SPRINT120 = [
    ("ThisWeekShadyIncomeSpendSummaryLine", "thisWeekShadyIncomeSpend", "this week shady income spend", "this_week_shady_income_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_130;\n            lab.Ledger.ThisWeek.Revenue = 18_790;", "2,130", "18,790"),
    ("LastWeekShadyIncomeSpendSummaryLine", "lastWeekShadyIncomeSpend", "last week shady income spend", "last_week_shady_income_spend_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_650;\n            lab.Ledger.LastWeek.Grants = 6_140;", "2,650", "6,140"),
    ("LifetimeShadyIncomeSpendSummaryLine", "lifetimeShadyIncomeSpend", "lifetime shady income spend", "lifetime_shady_income_spend_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_100;\n            lab.Ledger.Lifetime.Net = 115_500;", "53,100", "115,500"),
    ("ThisWeekHardwareSpendIncomeSummaryLine", "thisWeekHardwareSpendIncome", "this week hardware spend income", "this_week_hardware_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_370;\n            lab.Ledger.ThisWeek.Fines = -860;", "3,370", "860"),
    ("LastWeekHardwareSpendIncomeSummaryLine", "lastWeekHardwareSpendIncome", "last week hardware spend income", "last_week_hardware_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_090;\n            lab.Ledger.LastWeek.Revenue = 18_310;", "3,090", "18,310"),
    ("LifetimeHardwareSpendIncomeSummaryLine", "lifetimeHardwareSpendIncome", "lifetime hardware spend income", "lifetime_hardware_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 444_000;\n            lab.Ledger.Lifetime.Fines = -28_000;", "444,000", "28,000"),
]

PA117 = '            string lifetimeRoomSpendIncomePart = string.IsNullOrEmpty(lifetimeRoomSpendIncome) ? "" : " · " + lifetimeRoomSpendIncome;'
PA118 = '            string lifetimeHardwareIncomeSpendPart = string.IsNullOrEmpty(lifetimeHardwareIncomeSpend) ? "" : " · " + lifetimeHardwareIncomeSpend;'
PA119 = '            string lifetimeCapitalIncomeSpendPart = string.IsNullOrEmpty(lifetimeCapitalIncomeSpend) ? "" : " · " + lifetimeCapitalIncomeSpend;'
PA120 = '            string lifetimeUpkeepIncomeSpendPart = string.IsNullOrEmpty(lifetimeUpkeepIncomeSpend) ? "" : " · " + lifetimeUpkeepIncomeSpend;'

"""

MAIN = """
insert_formatters(F117_118)
git_commit("feat(hud): sprint117-118 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT117, PA117, "lifetimeRoomSpendIncomePart")
run_sprint(SPRINT118, PA118, "lifetimeHardwareIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F119_120)
git_commit("feat(hud): sprint119-120 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT119, PA119, "lifetimeCapitalIncomeSpendPart")
run_sprint(SPRINT120, PA120, "lifetimeUpkeepIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_117_120.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_117_120.py", doraise=True)
print("wrote", len(out))