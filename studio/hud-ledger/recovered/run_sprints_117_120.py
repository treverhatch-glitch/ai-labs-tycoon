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
