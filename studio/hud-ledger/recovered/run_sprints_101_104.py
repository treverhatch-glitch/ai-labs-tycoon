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
