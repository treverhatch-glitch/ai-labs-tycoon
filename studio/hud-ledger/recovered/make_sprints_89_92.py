#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_85_88.py").read_text()
head = src[: src.index("F85_86")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F85_86)")]

DATA = r"""
F89_90 = [
    ("ThisWeekLaunderIncomeSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLaunderIncomeSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeLaunderIncomeSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("ThisWeekCapitalIncomeSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekCapitalIncomeSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeCapitalIncomeSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekSiteIncomeSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekSiteIncomeSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSiteIncomeSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHardwareIncomeSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekHardwareIncomeSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeHardwareIncomeSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

F91_92 = [
    ("ThisWeekConstructionIncomeSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekConstructionIncomeSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeConstructionIncomeSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekSalariesIncomeSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekSalariesIncomeSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSalariesIncomeSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLandIncomeSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLandIncomeSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeLandIncomeSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRoomIncomeSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRoomIncomeSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRoomIncomeSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT89 = [
    ("ThisWeekLaunderIncomeSummaryLine", "thisWeekLaunderIncome", "this week launder income", "this_week_launder_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_040;\n            lab.Ledger.ThisWeek.Grants = 4_840;", "2,040", "4,840"),
    ("LastWeekLaunderIncomeSummaryLine", "lastWeekLaunderIncome", "last week launder income", "last_week_launder_income_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_380;\n            lab.Ledger.LastWeek.GreyRevenue = 2_560;", "3,380", "2,560"),
    ("LifetimeLaunderIncomeSummaryLine", "lifetimeLaunderIncome", "lifetime launder income", "lifetime_launder_income_summary_line",
     "lab.Ledger.Lifetime.Laundered = 32_000;\n            lab.Ledger.Lifetime.GreyRevenue = 52_600;", "32,000", "52,600"),
    ("ThisWeekCapitalIncomeSummaryLine", "thisWeekCapitalIncome", "this week capital income", "this_week_capital_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_590;\n            lab.Ledger.ThisWeek.Grants = 4_860;", "18,590", "4,860"),
    ("LastWeekCapitalIncomeSummaryLine", "lastWeekCapitalIncome", "last week capital income", "last_week_capital_income_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 42_900;\n            lab.Ledger.LastWeek.Grants = 5_860;", "42,900", "5,860"),
    ("LifetimeCapitalIncomeSummaryLine", "lifetimeCapitalIncome", "lifetime capital income", "lifetime_capital_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 425_000;\n            lab.Ledger.Lifetime.Net = 99_000;", "425,000", "99,000"),
]

SPRINT90 = [
    ("ThisWeekSiteIncomeSummaryLine", "thisWeekSiteIncome", "this week site income", "this_week_site_income_summary_line",
     "lab.Ledger.ThisWeek.Training = -9_520;\n            lab.Ledger.ThisWeek.Revenue = 18_610;", "9,520", "18,610"),
    ("LastWeekSiteIncomeSummaryLine", "lastWeekSiteIncome", "last week site income", "last_week_site_income_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_580;\n            lab.Ledger.LastWeek.Grants = 5_880;", "7,580", "5,880"),
    ("LifetimeSiteIncomeSummaryLine", "lifetimeSiteIncome", "lifetime site income", "lifetime_site_income_summary_line",
     "lab.Ledger.Lifetime.Training = -155_000;\n            lab.Ledger.Lifetime.Grants = 154_000;", "155,000", "154,000"),
    ("ThisWeekHardwareIncomeSummaryLine", "thisWeekHardwareIncome", "this week hardware income", "this_week_hardware_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_170;\n            lab.Ledger.ThisWeek.Power = -2_840;", "6,170", "2,840"),
    ("LastWeekHardwareIncomeSummaryLine", "lastWeekHardwareIncome", "last week hardware income", "last_week_hardware_income_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_620;\n            lab.Ledger.LastWeek.Power = -3_680;", "4,620", "3,680"),
    ("LifetimeHardwareIncomeSummaryLine", "lifetimeHardwareIncome", "lifetime hardware income", "lifetime_hardware_income_summary_line",
     "lab.Ledger.Lifetime.Power = -161_000;\n            lab.Ledger.Lifetime.Grants = 155_000;", "161,000", "155,000"),
]

SPRINT91 = [
    ("ThisWeekConstructionIncomeSummaryLine", "thisWeekConstructionIncome", "this week construction income", "this_week_construction_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_180;\n            lab.Ledger.ThisWeek.Grants = 4_880;", "6,180", "4,880"),
    ("LastWeekConstructionIncomeSummaryLine", "lastWeekConstructionIncome", "last week construction income", "last_week_construction_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -6_120;\n            lab.Ledger.LastWeek.Revenue = 18_040;", "6,120", "18,040"),
    ("LifetimeConstructionIncomeSummaryLine", "lifetimeConstructionIncome", "lifetime construction income", "lifetime_construction_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 426_000;\n            lab.Ledger.Lifetime.Fines = -26_300;", "426,000", "26,300"),
    ("ThisWeekSalariesIncomeSummaryLine", "thisWeekSalariesIncome", "this week salaries income", "this_week_salaries_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_840;\n            lab.Ledger.ThisWeek.Net = 3_170;", "7,840", "3,170"),
    ("LastWeekSalariesIncomeSummaryLine", "lastWeekSalariesIncome", "last week salaries income", "last_week_salaries_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -7_820;\n            lab.Ledger.LastWeek.Grants = 5_900;", "7,820", "5,900"),
    ("LifetimeSalariesIncomeSummaryLine", "lifetimeSalariesIncome", "lifetime salaries income", "lifetime_salaries_income_summary_line",
     "lab.Ledger.Lifetime.Training = -156_000;\n            lab.Ledger.Lifetime.Grants = 156_000;", "156,000", "156,000"),
]

SPRINT92 = [
    ("ThisWeekLandIncomeSummaryLine", "thisWeekLandIncome", "this week land income", "this_week_land_income_summary_line",
     "lab.Ledger.ThisWeek.Land = -11_350;\n            lab.Ledger.ThisWeek.Grants = 4_900;", "11,350", "4,900"),
    ("LastWeekLandIncomeSummaryLine", "lastWeekLandIncome", "last week land income", "last_week_land_income_summary_line",
     "lab.Ledger.LastWeek.Land = -12_100;\n            lab.Ledger.LastWeek.Net = 2_850;", "12,100", "2,850"),
    ("LifetimeLandIncomeSummaryLine", "lifetimeLandIncome", "lifetime land income", "lifetime_land_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_700;\n            lab.Ledger.Lifetime.Grants = 157_000;", "52,700", "157,000"),
    ("ThisWeekRoomIncomeSummaryLine", "thisWeekRoomIncome", "this week room income", "this_week_room_income_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -2_280;\n            lab.Ledger.ThisWeek.Revenue = 18_630;", "2,280", "18,630"),
    ("LastWeekRoomIncomeSummaryLine", "lastWeekRoomIncome", "last week room income", "last_week_room_income_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_260;\n            lab.Ledger.LastWeek.Grants = 5_920;", "2,260", "5,920"),
    ("LifetimeRoomIncomeSummaryLine", "lifetimeRoomIncome", "lifetime room income", "lifetime_room_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 427_000;\n            lab.Ledger.Lifetime.Net = 100_000;", "427,000", "100,000"),
]

PA89 = '            string lifetimeTrainingIncomePart = string.IsNullOrEmpty(lifetimeTrainingIncome) ? "" : " · " + lifetimeTrainingIncome;'
PA90 = '            string lifetimeCapitalIncomePart = string.IsNullOrEmpty(lifetimeCapitalIncome) ? "" : " · " + lifetimeCapitalIncome;'
PA91 = '            string lifetimeHardwareIncomePart = string.IsNullOrEmpty(lifetimeHardwareIncome) ? "" : " · " + lifetimeHardwareIncome;'
PA92 = '            string lifetimeSalariesIncomePart = string.IsNullOrEmpty(lifetimeSalariesIncome) ? "" : " · " + lifetimeSalariesIncome;'

"""

MAIN = """
insert_formatters(F89_90)
git_commit("feat(hud): sprint89-90 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT89, PA89, "lifetimeTrainingIncomePart")
run_sprint(SPRINT90, PA90, "lifetimeCapitalIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F91_92)
git_commit("feat(hud): sprint91-92 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT91, PA91, "lifetimeHardwareIncomePart")
run_sprint(SPRINT92, PA92, "lifetimeSalariesIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_89_92.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_89_92.py", doraise=True)
print("wrote", len(out))