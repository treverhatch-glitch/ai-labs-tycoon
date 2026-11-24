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















F121_122 = [
    ("ThisWeekSiteSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekSiteSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeSiteSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekLaunderSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekLaunderSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeLaunderSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekPatentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPatentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LifetimePatentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

F123_124 = [
    ("ThisWeekUpkeepSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LastWeekUpkeepSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LifetimeUpkeepSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekShadySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekShadySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeShadySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekNetSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekNetSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeNetSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT121 = [
    ("ThisWeekSiteSpendIncomeSummaryLine", "thisWeekSiteSpendIncome", "this week site spend income", "this_week_site_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_380;\n            lab.Ledger.ThisWeek.Grants = 5_190;", "3,380", "5,190"),
    ("LastWeekSiteSpendIncomeSummaryLine", "lastWeekSiteSpendIncome", "last week site spend income", "last_week_site_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_320;\n            lab.Ledger.LastWeek.Fines = -870;", "18,320", "870"),
    ("LifetimeSiteSpendIncomeSummaryLine", "lifetimeSiteSpendIncome", "lifetime site spend income", "lifetime_site_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 116_000;\n            lab.Ledger.Lifetime.Construction = -84_700;", "116,000", "84,700"),
    ("ThisWeekLaunderSpendIncomeSummaryLine", "thisWeekLaunderSpendIncome", "this week launder spend income", "this_week_launder_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 3_480;\n            lab.Ledger.ThisWeek.Revenue = 18_800;", "3,480", "18,800"),
    ("LastWeekLaunderSpendIncomeSummaryLine", "lastWeekLaunderSpendIncome", "last week launder spend income", "last_week_launder_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_100;\n            lab.Ledger.LastWeek.Grants = 6_150;", "3,100", "6,150"),
    ("LifetimeLaunderSpendIncomeSummaryLine", "lifetimeLaunderSpendIncome", "lifetime launder spend income", "lifetime_launder_spend_income_summary_line",
     "lab.Ledger.Lifetime.Laundered = 32_900;\n            lab.Ledger.Lifetime.Grants = 182_000;", "32,900", "182,000"),
]

SPRINT122 = [
    ("ThisWeekCapitalSpendIncomeSummaryLine", "thisWeekCapitalSpendIncome", "this week capital spend income", "this_week_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_200;\n            lab.Ledger.ThisWeek.Fines = -870;", "5,200", "870"),
    ("LastWeekCapitalSpendIncomeSummaryLine", "lastWeekCapitalSpendIncome", "last week capital spend income", "last_week_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_160;\n            lab.Ledger.LastWeek.Revenue = 18_330;", "6,160", "18,330"),
    ("LifetimeCapitalSpendIncomeSummaryLine", "lifetimeCapitalSpendIncome", "lifetime capital spend income", "lifetime_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 183_000;\n            lab.Ledger.Lifetime.Net = 116_500;", "183,000", "116,500"),
    ("ThisWeekPatentSpendIncomeSummaryLine", "thisWeekPatentSpendIncome", "this week patent spend income", "this_week_patent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -880;\n            lab.Ledger.ThisWeek.Net = 3_390;", "880", "3,390"),
    ("LastWeekPatentSpendIncomeSummaryLine", "lastWeekPatentSpendIncome", "last week patent spend income", "last_week_patent_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -880;\n            lab.Ledger.LastWeek.Net = 3_110;", "880", "3,110"),
    ("LifetimePatentSpendIncomeSummaryLine", "lifetimePatentSpendIncome", "lifetime patent spend income", "lifetime_patent_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -28_100;\n            lab.Ledger.Lifetime.Revenue = 445_000;", "28,100", "445,000"),
]

SPRINT123 = [
    ("ThisWeekUpkeepSpendIncomeSummaryLine", "thisWeekUpkeepSpendIncome", "this week upkeep spend income", "this_week_upkeep_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -3_000;\n            lab.Ledger.ThisWeek.Grants = 5_210;", "3,000", "5,210"),
    ("LastWeekUpkeepSpendIncomeSummaryLine", "lastWeekUpkeepSpendIncome", "last week upkeep spend income", "last_week_upkeep_spend_income_summary_line",
     "lab.Ledger.LastWeek.Power = -2_450;\n            lab.Ledger.LastWeek.Revenue = 18_340;", "2,450", "18,340"),
    ("LifetimeUpkeepSpendIncomeSummaryLine", "lifetimeUpkeepSpendIncome", "lifetime upkeep spend income", "lifetime_upkeep_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -167_000;\n            lab.Ledger.Lifetime.Net = 117_000;", "167,000", "117,000"),
    ("ThisWeekShadySpendIncomeSummaryLine", "thisWeekShadySpendIncome", "this week shady spend income", "this_week_shady_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_140;\n            lab.Ledger.ThisWeek.Fines = -880;", "2,140", "880"),
    ("LastWeekShadySpendIncomeSummaryLine", "lastWeekShadySpendIncome", "last week shady spend income", "last_week_shady_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_660;\n            lab.Ledger.LastWeek.Grants = 6_170;", "2,660", "6,170"),
    ("LifetimeShadySpendIncomeSummaryLine", "lifetimeShadySpendIncome", "lifetime shady spend income", "lifetime_shady_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_200;\n            lab.Ledger.Lifetime.Laundered = 33_000;", "53,200", "33,000"),
]

SPRINT124 = [
    ("ThisWeekNetSpendIncomeSummaryLine", "thisWeekNetSpendIncome", "this week net spend income", "this_week_net_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_400;\n            lab.Ledger.ThisWeek.Revenue = 18_810;", "3,400", "18,810"),
    ("LastWeekNetSpendIncomeSummaryLine", "lastWeekNetSpendIncome", "last week net spend income", "last_week_net_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_120;\n            lab.Ledger.LastWeek.Fines = -890;", "3,120", "890"),
    ("LifetimeNetSpendIncomeSummaryLine", "lifetimeNetSpendIncome", "lifetime net spend income", "lifetime_net_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 117_500;\n            lab.Ledger.Lifetime.Grants = 184_000;", "117,500", "184,000"),
    ("ThisWeekGreySpendIncomeSummaryLine", "thisWeekGreySpendIncome", "this week grey spend income", "this_week_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_150;\n            lab.Ledger.ThisWeek.Grants = 5_220;", "2,150", "5,220"),
    ("LastWeekGreySpendIncomeSummaryLine", "lastWeekGreySpendIncome", "last week grey spend income", "last_week_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_670;\n            lab.Ledger.LastWeek.Revenue = 18_350;", "2,670", "18,350"),
    ("LifetimeGreySpendIncomeSummaryLine", "lifetimeGreySpendIncome", "lifetime grey spend income", "lifetime_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_300;\n            lab.Ledger.Lifetime.Revenue = 446_000;", "53,300", "446,000"),
]

PA121 = '            string lifetimeHardwareSpendIncomePart = string.IsNullOrEmpty(lifetimeHardwareSpendIncome) ? "" : " · " + lifetimeHardwareSpendIncome;'
PA122 = '            string lifetimeLaunderSpendIncomePart = string.IsNullOrEmpty(lifetimeLaunderSpendIncome) ? "" : " · " + lifetimeLaunderSpendIncome;'
PA123 = '            string lifetimePatentSpendIncomePart = string.IsNullOrEmpty(lifetimePatentSpendIncome) ? "" : " · " + lifetimePatentSpendIncome;'
PA124 = '            string lifetimeShadySpendIncomePart = string.IsNullOrEmpty(lifetimeShadySpendIncome) ? "" : " · " + lifetimeShadySpendIncome;'


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


insert_formatters(F121_122)
git_commit("feat(hud): sprint121-122 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT121, PA121, "lifetimeHardwareSpendIncomePart")
run_sprint(SPRINT122, PA122, "lifetimeLaunderSpendIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F123_124)
git_commit("feat(hud): sprint123-124 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT123, PA123, "lifetimePatentSpendIncomePart")
run_sprint(SPRINT124, PA124, "lifetimeShadySpendIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
