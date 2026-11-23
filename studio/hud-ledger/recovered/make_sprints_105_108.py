#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_101_104.py").read_text()
head = src[: src.index("F101_102")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F101_102)")]

DATA = r"""
F105_106 = [
    ("ThisWeekUpkeepSpendSummaryLine", [
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekUpkeepSpendSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeUpkeepSpendSummaryLine", [
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekShadySpendSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekShadySpendSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeShadySpendSummaryLine", [
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekNetIncomeSpendSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekNetIncomeSpendSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeNetIncomeSpendSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekGreyIncomeSpendSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekGreyIncomeSpendSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeGreyIncomeSpendSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

F107_108 = [
    ("ThisWeekFinesIncomeSpendSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekFinesIncomeSpendSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeFinesIncomeSpendSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekGrantIncomeSpendSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekGrantIncomeSpendSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeGrantIncomeSpendSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekChipIncomeSpendSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekChipIncomeSpendSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeChipIncomeSpendSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekDevIncomeSpendSummaryLine", [
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekDevIncomeSpendSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeDevIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

SPRINT105 = [
    ("ThisWeekUpkeepSpendSummaryLine", "thisWeekUpkeepSpend", "this week upkeep spend", "this_week_upkeep_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_920;\n            lab.Ledger.ThisWeek.Net = 3_270;", "2,920", "3,270"),
    ("LastWeekUpkeepSpendSummaryLine", "lastWeekUpkeepSpend", "last week upkeep spend", "last_week_upkeep_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_180;\n            lab.Ledger.LastWeek.Fines = -760;", "18,180", "760"),
    ("LifetimeUpkeepSpendSummaryLine", "lifetimeUpkeepSpend", "lifetime upkeep spend", "lifetime_upkeep_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 108_000;\n            lab.Ledger.Lifetime.Fines = -27_200;", "108,000", "27,200"),
    ("ThisWeekShadySpendSummaryLine", "thisWeekShadySpend", "this week shady spend", "this_week_shady_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_100;\n            lab.Ledger.ThisWeek.Fines = -760;", "2,100", "760"),
    ("LastWeekShadySpendSummaryLine", "lastWeekShadySpend", "last week shady spend", "last_week_shady_spend_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_440;\n            lab.Ledger.LastWeek.Net = 2_960;", "3,440", "2,960"),
    ("LifetimeShadySpendSummaryLine", "lifetimeShadySpend", "lifetime shady spend", "lifetime_shady_spend_summary_line",
     "lab.Ledger.Lifetime.Laundered = 32_700;\n            lab.Ledger.Lifetime.Grants = 168_000;", "32,700", "168,000"),
]

SPRINT106 = [
    ("ThisWeekNetIncomeSpendSummaryLine", "thisWeekNetIncomeSpend", "this week net income spend", "this_week_net_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_280;\n            lab.Ledger.ThisWeek.Revenue = 18_700;", "3,280", "18,700"),
    ("LastWeekNetIncomeSpendSummaryLine", "lastWeekNetIncomeSpend", "last week net income spend", "last_week_net_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 2_970;\n            lab.Ledger.LastWeek.Grants = 6_050;", "2,970", "6,050"),
    ("LifetimeNetIncomeSpendSummaryLine", "lifetimeNetIncomeSpend", "lifetime net income spend", "lifetime_net_income_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 108_500;\n            lab.Ledger.Lifetime.Revenue = 436_000;", "108,500", "436,000"),
    ("ThisWeekGreyIncomeSpendSummaryLine", "thisWeekGreyIncomeSpend", "this week grey income spend", "this_week_grey_income_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_110;\n            lab.Ledger.ThisWeek.Grants = 5_060;", "2,110", "5,060"),
    ("LastWeekGreyIncomeSpendSummaryLine", "lastWeekGreyIncomeSpend", "last week grey income spend", "last_week_grey_income_spend_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_630;\n            lab.Ledger.LastWeek.Revenue = 18_190;", "2,630", "18,190"),
    ("LifetimeGreyIncomeSpendSummaryLine", "lifetimeGreyIncomeSpend", "lifetime grey income spend", "lifetime_grey_income_spend_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_000;\n            lab.Ledger.Lifetime.Grants = 169_000;", "53,000", "169,000"),
]

SPRINT107 = [
    ("ThisWeekFinesIncomeSpendSummaryLine", "thisWeekFinesIncomeSpend", "this week fines income spend", "this_week_fines_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -770;\n            lab.Ledger.ThisWeek.Grants = 5_070;", "770", "5,070"),
    ("LastWeekFinesIncomeSpendSummaryLine", "lastWeekFinesIncomeSpend", "last week fines income spend", "last_week_fines_income_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -770;\n            lab.Ledger.LastWeek.Net = 2_980;", "770", "2,980"),
    ("LifetimeFinesIncomeSpendSummaryLine", "lifetimeFinesIncomeSpend", "lifetime fines income spend", "lifetime_fines_income_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_300;\n            lab.Ledger.Lifetime.Net = 109_000;", "27,300", "109,000"),
    ("ThisWeekGrantIncomeSpendSummaryLine", "thisWeekGrantIncomeSpend", "this week grant income spend", "this_week_grant_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_080;\n            lab.Ledger.ThisWeek.Revenue = 18_710;", "5,080", "18,710"),
    ("LastWeekGrantIncomeSpendSummaryLine", "lastWeekGrantIncomeSpend", "last week grant income spend", "last_week_grant_income_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_060;\n            lab.Ledger.LastWeek.Revenue = 18_200;", "6,060", "18,200"),
    ("LifetimeGrantIncomeSpendSummaryLine", "lifetimeGrantIncomeSpend", "lifetime grant income spend", "lifetime_grant_income_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 170_000;\n            lab.Ledger.Lifetime.Revenue = 437_000;", "170,000", "437,000"),
]

SPRINT108 = [
    ("ThisWeekChipIncomeSpendSummaryLine", "thisWeekChipIncomeSpend", "this week chip income spend", "this_week_chip_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_720;\n            lab.Ledger.ThisWeek.Fines = -780;", "18,720", "780"),
    ("LastWeekChipIncomeSpendSummaryLine", "lastWeekChipIncomeSpend", "last week chip income spend", "last_week_chip_income_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_070;\n            lab.Ledger.LastWeek.Net = 2_990;", "6,070", "2,990"),
    ("LifetimeChipIncomeSpendSummaryLine", "lifetimeChipIncomeSpend", "lifetime chip income spend", "lifetime_chip_income_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 109_500;\n            lab.Ledger.Lifetime.Fines = -27_400;", "109,500", "27,400"),
    ("ThisWeekDevIncomeSpendSummaryLine", "thisWeekDevIncomeSpend", "this week dev income spend", "this_week_dev_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_290;\n            lab.Ledger.ThisWeek.Power = -2_930;", "3,290", "2,930"),
    ("LastWeekDevIncomeSpendSummaryLine", "lastWeekDevIncomeSpend", "last week dev income spend", "last_week_dev_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 3_000;\n            lab.Ledger.LastWeek.Fines = -780;", "3,000", "780"),
    ("LifetimeDevIncomeSpendSummaryLine", "lifetimeDevIncomeSpend", "lifetime dev income spend", "lifetime_dev_income_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 171_000;\n            lab.Ledger.Lifetime.Net = 110_000;", "171,000", "110,000"),
]

PA105 = '            string lifetimeIncomeSpendPart = string.IsNullOrEmpty(lifetimeIncomeSpend) ? "" : " · " + lifetimeIncomeSpend;'
PA106 = '            string lifetimeShadySpendPart = string.IsNullOrEmpty(lifetimeShadySpend) ? "" : " · " + lifetimeShadySpend;'
PA107 = '            string lifetimeGreyIncomeSpendPart = string.IsNullOrEmpty(lifetimeGreyIncomeSpend) ? "" : " · " + lifetimeGreyIncomeSpend;'
PA108 = '            string lifetimeGrantIncomeSpendPart = string.IsNullOrEmpty(lifetimeGrantIncomeSpend) ? "" : " · " + lifetimeGrantIncomeSpend;'

"""

MAIN = """
insert_formatters(F105_106)
git_commit("feat(hud): sprint105-106 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT105, PA105, "lifetimeIncomeSpendPart")
run_sprint(SPRINT106, PA106, "lifetimeShadySpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F107_108)
git_commit("feat(hud): sprint107-108 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT107, PA107, "lifetimeGreyIncomeSpendPart")
run_sprint(SPRINT108, PA108, "lifetimeGrantIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_105_108.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_105_108.py", doraise=True)
print("wrote", len(out))