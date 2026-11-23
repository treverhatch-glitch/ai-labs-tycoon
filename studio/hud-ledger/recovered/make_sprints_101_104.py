#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_97_100.py").read_text()
head = src[: src.index("F97_98")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F97_98)")]

DATA = r"""
F101_102 = [
    ("ThisWeekLandSpendSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLandSpendSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeLandSpendSummaryLine", [
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekRoomSpendSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekRoomSpendSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeRoomSpendSummaryLine", [
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSiteSpendSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekSiteSpendSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeSiteSpendSummaryLine", [
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekHardwareSpendSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekHardwareSpendSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHardwareSpendSummaryLine", [
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

F103_104 = [
    ("ThisWeekLaunderSpendSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekLaunderSpendSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeLaunderSpendSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekCapitalSpendSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekCapitalSpendSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeCapitalSpendSummaryLine", [
        ("capital", "LifetimeLedgerCapitalInLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekPatentSpendSummaryLine", [
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPatentSpendSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimePatentSpendSummaryLine", [
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekIncomeSpendSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekIncomeSpendSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeIncomeSpendSummaryLine", [
        ("income", "LifetimeLedgerIncomeSummaryLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT101 = [
    ("ThisWeekLandSpendSummaryLine", "thisWeekLandSpend", "this week land spend", "this_week_land_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_010;\n            lab.Ledger.ThisWeek.Fines = -730;", "5,010", "730"),
    ("LastWeekLandSpendSummaryLine", "lastWeekLandSpend", "last week land spend", "last_week_land_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_130;\n            lab.Ledger.LastWeek.Fines = -730;", "18,130", "730"),
    ("LifetimeLandSpendSummaryLine", "lifetimeLandSpend", "lifetime land spend", "lifetime_land_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 106_000;\n            lab.Ledger.Lifetime.Fines = -26_900;", "106,000", "26,900"),
    ("ThisWeekRoomSpendSummaryLine", "thisWeekRoomSpend", "this week room spend", "this_week_room_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_900;\n            lab.Ledger.ThisWeek.Net = 3_240;", "2,900", "3,240"),
    ("LastWeekRoomSpendSummaryLine", "lastWeekRoomSpend", "last week room spend", "last_week_room_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 2_920;\n            lab.Ledger.LastWeek.Grants = 6_020;", "2,920", "6,020"),
    ("LifetimeRoomSpendSummaryLine", "lifetimeRoomSpend", "lifetime room spend", "lifetime_room_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 164_000;\n            lab.Ledger.Lifetime.Revenue = 433_000;", "164,000", "433,000"),
]

SPRINT102 = [
    ("ThisWeekSiteSpendSummaryLine", "thisWeekSiteSpend", "this week site spend", "this_week_site_spend_summary_line",
     "lab.Ledger.ThisWeek.Construction = -9_650;\n            lab.Ledger.ThisWeek.Grants = 5_020;", "9,650", "5,020"),
    ("LastWeekSiteSpendSummaryLine", "lastWeekSiteSpend", "last week site spend", "last_week_site_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -740;\n            lab.Ledger.LastWeek.Revenue = 18_140;", "740", "18,140"),
    ("LifetimeSiteSpendSummaryLine", "lifetimeSiteSpend", "lifetime site spend", "lifetime_site_spend_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_300;\n            lab.Ledger.Lifetime.Net = 106_500;", "84,300", "106,500"),
    ("ThisWeekHardwareSpendSummaryLine", "thisWeekHardwareSpend", "this week hardware spend", "this_week_hardware_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_250;\n            lab.Ledger.ThisWeek.Revenue = 18_680;", "3,250", "18,680"),
    ("LastWeekHardwareSpendSummaryLine", "lastWeekHardwareSpend", "last week hardware spend", "last_week_hardware_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 2_930;\n            lab.Ledger.LastWeek.Grants = 6_030;", "2,930", "6,030"),
    ("LifetimeHardwareSpendSummaryLine", "lifetimeHardwareSpend", "lifetime hardware spend", "lifetime_hardware_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_000;\n            lab.Ledger.Lifetime.Grants = 165_000;", "27,000", "165,000"),
]

SPRINT103 = [
    ("ThisWeekLaunderSpendSummaryLine", "thisWeekLaunderSpend", "this week launder spend", "this_week_launder_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_080;\n            lab.Ledger.ThisWeek.Fines = -740;", "2,080", "740"),
    ("LastWeekLaunderSpendSummaryLine", "lastWeekLaunderSpend", "last week launder spend", "last_week_launder_spend_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_420;\n            lab.Ledger.LastWeek.Revenue = 18_150;", "3,420", "18,150"),
    ("LifetimeLaunderSpendSummaryLine", "lifetimeLaunderSpend", "lifetime launder spend", "lifetime_launder_spend_summary_line",
     "lab.Ledger.Lifetime.Laundered = 32_600;\n            lab.Ledger.Lifetime.Net = 107_000;", "32,600", "107,000"),
    ("ThisWeekCapitalSpendSummaryLine", "thisWeekCapitalSpend", "this week capital spend", "this_week_capital_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_030;\n            lab.Ledger.ThisWeek.Net = 3_260;", "5,030", "3,260"),
    ("LastWeekCapitalSpendSummaryLine", "lastWeekCapitalSpend", "last week capital spend", "last_week_capital_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_040;\n            lab.Ledger.LastWeek.Net = 2_940;", "6,040", "2,940"),
    ("LifetimeCapitalSpendSummaryLine", "lifetimeCapitalSpend", "lifetime capital spend", "lifetime_capital_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 166_000;\n            lab.Ledger.Lifetime.Revenue = 434_000;", "166,000", "434,000"),
]

SPRINT104 = [
    ("ThisWeekPatentSpendSummaryLine", "thisWeekPatentSpend", "this week patent spend", "this_week_patent_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -750;\n            lab.Ledger.ThisWeek.Grants = 5_040;", "750", "5,040"),
    ("LastWeekPatentSpendSummaryLine", "lastWeekPatentSpend", "last week patent spend", "last_week_patent_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -750;\n            lab.Ledger.LastWeek.Revenue = 18_160;", "750", "18,160"),
    ("LifetimePatentSpendSummaryLine", "lifetimePatentSpend", "lifetime patent spend", "lifetime_patent_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_100;\n            lab.Ledger.Lifetime.Net = 107_500;", "27,100", "107,500"),
    ("ThisWeekIncomeSpendSummaryLine", "thisWeekIncomeSpend", "this week income spend", "this_week_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_690;\n            lab.Ledger.ThisWeek.Power = -2_910;", "18,690", "2,910"),
    ("LastWeekIncomeSpendSummaryLine", "lastWeekIncomeSpend", "last week income spend", "last_week_income_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_170;\n            lab.Ledger.LastWeek.Net = 2_950;", "18,170", "2,950"),
    ("LifetimeIncomeSpendSummaryLine", "lifetimeIncomeSpend", "lifetime income spend", "lifetime_income_spend_summary_line",
     "lab.Ledger.Lifetime.Revenue = 435_000;\n            lab.Ledger.Lifetime.Grants = 167_000;", "435,000", "167,000"),
]

PA101 = '            string lifetimeHiringSpendPart = string.IsNullOrEmpty(lifetimeHiringSpend) ? "" : " · " + lifetimeHiringSpend;'
PA102 = '            string lifetimeRoomSpendPart = string.IsNullOrEmpty(lifetimeRoomSpend) ? "" : " · " + lifetimeRoomSpend;'
PA103 = '            string lifetimeHardwareSpendPart = string.IsNullOrEmpty(lifetimeHardwareSpend) ? "" : " · " + lifetimeHardwareSpend;'
PA104 = '            string lifetimeCapitalSpendPart = string.IsNullOrEmpty(lifetimeCapitalSpend) ? "" : " · " + lifetimeCapitalSpend;'

"""

MAIN = """
insert_formatters(F101_102)
git_commit("feat(hud): sprint101-102 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT101, PA101, "lifetimeHiringSpendPart")
run_sprint(SPRINT102, PA102, "lifetimeRoomSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F103_104)
git_commit("feat(hud): sprint103-104 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT103, PA103, "lifetimeHardwareSpendPart")
run_sprint(SPRINT104, PA104, "lifetimeCapitalSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_101_104.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_101_104.py", doraise=True)
print("wrote", len(out))