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
