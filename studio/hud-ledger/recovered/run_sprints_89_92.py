#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD_PATH = UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"







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


def formatter_body(name, parts):
    decl = "\n".join(f"            string {v} = {call};" for v, call in parts)
    names = [p[0] for p in parts]
    empty = " && ".join(f"string.IsNullOrEmpty({n})" for n in names)
    first = names[0]
    joins = [
        f"""            if (!string.IsNullOrEmpty({n}))
                line = string.IsNullOrEmpty(line) ? {n} : line + " · " + {n};"""
        for n in names[1:]
    ]
    return f"""
        public static string {name}(LabState lab)
        {{
{decl}
            if ({empty})
                return "";
            string line = "";
            if (!string.IsNullOrEmpty({first})) line = {first};
""" + "\n".join(joins) + """
            return line;
        }}
"""


def insert_formatters(batch):
    t = HUD_PATH.read_text()
    block = ""
    for item in batch:
        name = item[0]
        if name in t:
            continue
        block += formatter_body(name, item[1])
    if not block:
        return
    HUD_PATH.write_text(
        t.replace(
            "        public static string BankruptLine(LabState lab)",
            block + "\n        public static string BankruptLine(LabState lab)",
            1,
        )
    )


def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode == 0:
        return
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def cycle_def(t):
    method, var, slug, test_name, ledger, a, b = t
    test_body = f"""            var lab = new LabState {{ Id = 1 }};
            {ledger}
            string line = HqDisplayQueriesHud.{method}(lab);
            Assert.That(line, Does.Contain("{a}").Or.Contain("{b}"));"""
    return dict(method=method, var=var, slug=slug, test_name=test_name, test_body=test_body)


def wire_hud(c):
    t = HUD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                if (!string.IsNullOrEmpty({c['var']}))
                    status = string.IsNullOrEmpty(status) ? {c['var']} : status + " · " + {c['var']};
"""
    b = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    HUD.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_train(c):
    t = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                    string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                    if (!string.IsNullOrEmpty({c['var']}))
                        _summary.text += " · " + {c['var']};
"""
    b = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    TRAIN.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_staff(c, part_anchor, tail_token):
    t = STAFF.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*)", t)
    if not m:
        return False
    t = t.replace(m.group(1), m.group(1) + f" + {c['var']}Part", 1)
    STAFF.write_text(t)
    return True


def wire_mascot(c, part_anchor, tail_token):
    t = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(sim.Player)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(sim.Player);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*);", t)
    if not m:
        return False
    t = t.replace(m.group(1) + ";", m.group(1) + f" + {c['var']}Part;", 1)
    MASCOT.write_text(t)
    return True


def wire_build(c):
    t = BUILD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(_host.Simulation.Player)" in t:
        return False
    b = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{c["method"]}(_host.Simulation.Player)\n'
    BUILD.write_text(t.replace(b, line + b, 1))
    return True


def add_test(c):
    t = TESTS.read_text()
    if c["test_name"] in t:
        return False
    block = f"\n        [Test]\n        public void {c['test_name']}()\n        {{\n{c['test_body']}\n        }}\n"
    at = t.rfind("\n    }\n}")
    TESTS.write_text(t[:at] + block + t[at:])
    return True


def run_sprint(cycles, part_anchor, tail_token):
    for i, raw in enumerate(cycles, 1):
        c = cycle_def(raw)
        if wire_hud(c):
            git_commit(f"feat(hud): wire hud view {c['slug']}", [HUD])
        if wire_train(c):
            git_commit(f"feat(hud): wire training {c['slug']}", [TRAIN])
        if wire_staff(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire staff gpu {c['slug']}", [STAFF])
        if wire_mascot(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire build mascot {c['slug']}", [MASCOT])
        if wire_build(c):
            git_commit(f"feat(hud): wire build panel {c['slug']}", [BUILD])
        if add_test(c):
            git_commit(f"test(hud): {c['slug']} summary line", [TESTS])
        if i in {2, 4}:
            subprocess.check_call(
                ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
                cwd=ROOT,
            )


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
