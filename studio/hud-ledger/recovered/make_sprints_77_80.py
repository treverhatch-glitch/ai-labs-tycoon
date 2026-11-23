#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_73_76.py").read_text()
head = src[: src.index("F73_74")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F73_74)")]

DATA = r"""
F77_78 = [
    ("LifetimeLaunderPatentSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekNetPatentSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSitePatentSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekNetPatentSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeConstructionPatentSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSitePatentSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHardwarePatentSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSitePatentSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRoomPatentSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("ThisWeekRentPatentSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHardwarePatentSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekCapitalPatentSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
]

F79_80 = [
    ("ThisWeekFinesPatentSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeCapitalPatentSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("LastWeekFinesPatentSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeFinesPatentSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGrantPatentSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekDevPatentSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekDevPatentSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeDevPatentSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekIncomePatentSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekIncomePatentSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekPowerGrantSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeIncomePatentSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT77 = [
    ("LifetimeLaunderPatentSummaryLine", "lifetimeLaunderPatent", "lifetime launder patent", "lifetime_launder_patent_summary_line",
     "lab.Ledger.Lifetime.Laundered = 31_000;\n            lab.Ledger.Lifetime.Fines = -25_600;", "31,000", "25,600"),
    ("ThisWeekNetPatentSummaryLine", "thisWeekNetPatent", "this week net patent", "this_week_net_patent_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_110;\n            lab.Ledger.ThisWeek.Fines = -570;", "3,110", "570"),
    ("LifetimeSitePatentSummaryLine", "lifetimeSitePatent", "lifetime site patent", "lifetime_site_patent_summary_line",
     "lab.Ledger.Lifetime.Training = -152_000;\n            lab.Ledger.Lifetime.Grants = 139_000;", "152,000", "139,000"),
    ("LastWeekNetPatentSummaryLine", "lastWeekNetPatent", "last week net patent", "last_week_net_patent_summary_line",
     "lab.Ledger.LastWeek.Net = 2_740;\n            lab.Ledger.LastWeek.Fines = -630;", "2,740", "630"),
    ("LifetimeConstructionPatentSummaryLine", "lifetimeConstructionPatent", "lifetime construction patent", "lifetime_construction_patent_summary_line",
     "lab.Ledger.Lifetime.Power = -155_000;\n            lab.Ledger.Lifetime.Grants = 140_000;", "155,000", "140,000"),
    ("ThisWeekSitePatentSummaryLine", "thisWeekSitePatent", "this week site patent", "this_week_site_patent_summary_line",
     "lab.Ledger.ThisWeek.Training = -9_460;\n            lab.Ledger.ThisWeek.Fines = -580;", "9,460", "580"),
]

SPRINT78 = [
    ("LifetimeHardwarePatentSummaryLine", "lifetimeHardwarePatent", "lifetime hardware patent", "lifetime_hardware_patent_summary_line",
     "lab.Ledger.Lifetime.Power = -156_000;\n            lab.Ledger.Lifetime.Fines = -25_700;", "156,000", "25,700"),
    ("LastWeekSitePatentSummaryLine", "lastWeekSitePatent", "last week site patent", "last_week_site_patent_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_550;\n            lab.Ledger.LastWeek.Grants = 5_580;", "7,550", "5,580"),
    ("LifetimeRoomPatentSummaryLine", "lifetimeRoomPatent", "lifetime room patent", "lifetime_room_patent_summary_line",
     "lab.Ledger.Lifetime.Revenue = 416_000;\n            lab.Ledger.Lifetime.GreyRevenue = 51_000;", "416,000", "51,000"),
    ("ThisWeekRentPatentSummaryLine", "thisWeekRentPatent", "this week rent patent", "this_week_rent_patent_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_130;\n            lab.Ledger.ThisWeek.Grants = 4_620;", "2,130", "4,620"),
    ("LastWeekHardwarePatentSummaryLine", "lastWeekHardwarePatent", "last week hardware patent", "last_week_hardware_patent_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_600;\n            lab.Ledger.LastWeek.Power = -3_620;", "4,600", "3,620"),
    ("ThisWeekCapitalPatentSummaryLine", "thisWeekCapitalPatent", "this week capital patent", "this_week_capital_patent_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_350;\n            lab.Ledger.ThisWeek.Fines = -590;", "18,350", "590"),
]

SPRINT79 = [
    ("ThisWeekFinesPatentSummaryLine", "thisWeekFinesPatent", "this week fines patent", "this_week_fines_patent_summary_line",
     "lab.Ledger.ThisWeek.Fines = -600;\n            lab.Ledger.ThisWeek.Grants = 4_640;", "600", "4,640"),
    ("LifetimeCapitalPatentSummaryLine", "lifetimeCapitalPatent", "lifetime capital patent", "lifetime_capital_patent_summary_line",
     "lab.Ledger.Lifetime.Revenue = 417_000;\n            lab.Ledger.Lifetime.Grants = 141_000;", "417,000", "141,000"),
    ("LastWeekFinesPatentSummaryLine", "lastWeekFinesPatent", "last week fines patent", "last_week_fines_patent_summary_line",
     "lab.Ledger.LastWeek.Fines = -640;\n            lab.Ledger.LastWeek.Net = 2_760;", "640", "2,760"),
    ("LifetimeFinesPatentSummaryLine", "lifetimeFinesPatent", "lifetime fines patent", "lifetime_fines_patent_summary_line",
     "lab.Ledger.Lifetime.Fines = -25_800;\n            lab.Ledger.Lifetime.Grants = 142_000;", "25,800", "142,000"),
    ("LifetimeGrantPatentSummaryLine", "lifetimeGrantPatent", "lifetime grant patent", "lifetime_grant_patent_summary_line",
     "lab.Ledger.Lifetime.Grants = 143_000;\n            lab.Ledger.Lifetime.Power = -157_000;", "143,000", "157,000"),
    ("ThisWeekDevPatentSummaryLine", "thisWeekDevPatent", "this week dev patent", "this_week_dev_patent_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_490;\n            lab.Ledger.ThisWeek.Fines = -610;", "5,490", "610"),
]

SPRINT80 = [
    ("LastWeekDevPatentSummaryLine", "lastWeekDevPatent", "last week dev patent", "last_week_dev_patent_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_260;\n            lab.Ledger.LastWeek.Grants = 5_600;", "7,260", "5,600"),
    ("LifetimeDevPatentSummaryLine", "lifetimeDevPatent", "lifetime dev patent", "lifetime_dev_patent_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 248_000;\n            lab.Ledger.Lifetime.Training = -153_000;", "248,000", "153,000"),
    ("ThisWeekIncomePatentSummaryLine", "thisWeekIncomePatent", "this week income patent", "this_week_income_patent_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_400;\n            lab.Ledger.ThisWeek.Grants = 4_660;", "18,400", "4,660"),
    ("LastWeekIncomePatentSummaryLine", "lastWeekIncomePatent", "last week income patent", "last_week_income_patent_summary_line",
     "lab.Ledger.LastWeek.Revenue = 17_900;\n            lab.Ledger.LastWeek.Grants = 5_620;", "17,900", "5,620"),
    ("ThisWeekPowerGrantSummaryLine", "thisWeekPowerGrant", "this week power grant", "this_week_power_grant_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_810;\n            lab.Ledger.ThisWeek.Grants = 4_680;", "2,810", "4,680"),
    ("LifetimeIncomePatentSummaryLine", "lifetimeIncomePatent", "lifetime income patent", "lifetime_income_patent_summary_line",
     "lab.Ledger.Lifetime.Revenue = 418_000;\n            lab.Ledger.Lifetime.Grants = 144_000;", "418,000", "144,000"),
]

PA77 = '            string lastWeekLaunderPatentPart = string.IsNullOrEmpty(lastWeekLaunderPatent) ? "" : " · " + lastWeekLaunderPatent;'
PA78 = '            string thisWeekSitePatentPart = string.IsNullOrEmpty(thisWeekSitePatent) ? "" : " · " + thisWeekSitePatent;'
PA79 = '            string thisWeekCapitalPatentPart = string.IsNullOrEmpty(thisWeekCapitalPatent) ? "" : " · " + thisWeekCapitalPatent;'
PA80 = '            string thisWeekDevPatentPart = string.IsNullOrEmpty(thisWeekDevPatent) ? "" : " · " + thisWeekDevPatent;'

"""

MAIN = """
insert_formatters(F77_78)
git_commit("feat(hud): sprint77-78 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT77, PA77, "lastWeekLaunderPatentPart")
run_sprint(SPRINT78, PA78, "thisWeekSitePatentPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F79_80)
git_commit("feat(hud): sprint79-80 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT79, PA79, "thisWeekCapitalPatentPart")
run_sprint(SPRINT80, PA80, "thisWeekDevPatentPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_77_80.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_77_80.py", doraise=True)
print("wrote", len(out))