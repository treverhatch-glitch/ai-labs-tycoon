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

F837_838 = [
    ("ThisWeekStumpDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekStumpDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeStumpDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekDevShawlSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
    ]),
    ("LastWeekDevShawlSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
    ]),
    ("LifetimeDevShawlSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
    ]),
    ("ThisWeekShawlDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekShawlDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeShawlDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekDevRegentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekDevRegentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeDevRegentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("capital", "LifetimeLedgerCapitalInLine(lab)"),
    ]),
]

F839_840 = [
    ("ThisWeekRegentDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekRegentDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeRegentDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("capital", "LifetimeLedgerCapitalInLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekDevKindleSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LastWeekDevKindleSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LifetimeDevKindleSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
    ]),
    ("ThisWeekKindleDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekKindleDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeKindleDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekDevCitadelSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekDevCitadelSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeDevCitadelSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
    ]),
]

SPRINT837 = [
    ("ThisWeekStumpDevSpendIncomeSummaryLine", "thisWeekStumpDevSpendIncome", "this week stump dev spend income", "this_week_stump_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -930;\n            lab.Ledger.ThisWeek.ChipDevelopment = -11_530;", "930", "11,530"),
    ("LastWeekStumpDevSpendIncomeSummaryLine", "lastWeekStumpDevSpendIncome", "last week stump dev spend income", "last_week_stump_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -910;\n            lab.Ledger.LastWeek.ChipDevelopment = -11_330;", "910", "11,330"),
    ("LifetimeStumpDevSpendIncomeSummaryLine", "lifetimeStumpDevSpendIncome", "lifetime stump dev spend income", "lifetime_stump_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -44_200;\n            lab.Ledger.Lifetime.ChipDevelopment = -178_100;", "44,200", "178,100"),
    ("ThisWeekDevShawlSpendIncomeSummaryLine", "thisWeekDevShawlSpendIncome", "this week dev shawl spend income", "this_week_dev_shawl_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -11_540;\n            lab.Ledger.ThisWeek.Hardware = -4_350;", "11,540", "4,350"),
    ("LastWeekDevShawlSpendIncomeSummaryLine", "lastWeekDevShawlSpendIncome", "last week dev shawl spend income", "last_week_dev_shawl_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -11_340;\n            lab.Ledger.LastWeek.Hardware = -4_330;", "11,340", "4,330"),
    ("LifetimeDevShawlSpendIncomeSummaryLine", "lifetimeDevShawlSpendIncome", "lifetime dev shawl spend income", "lifetime_dev_shawl_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -178_300;\n            lab.Ledger.Lifetime.Hardware = -613_200;", "178,300", "613,200"),
]

SPRINT838 = [
    ("ThisWeekShawlDevSpendIncomeSummaryLine", "thisWeekShawlDevSpendIncome", "this week shawl dev spend income", "this_week_shawl_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_360;\n            lab.Ledger.ThisWeek.ChipDevelopment = -11_550;", "4,360", "11,550"),
    ("LastWeekShawlDevSpendIncomeSummaryLine", "lastWeekShawlDevSpendIncome", "last week shawl dev spend income", "last_week_shawl_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_340;\n            lab.Ledger.LastWeek.ChipDevelopment = -11_350;", "4,340", "11,350"),
    ("LifetimeShawlDevSpendIncomeSummaryLine", "lifetimeShawlDevSpendIncome", "lifetime shawl dev spend income", "lifetime_shawl_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hardware = -613_400;\n            lab.Ledger.Lifetime.ChipDevelopment = -178_500;", "613,400", "178,500"),
    ("ThisWeekDevRegentSpendIncomeSummaryLine", "thisWeekDevRegentSpendIncome", "this week dev regent spend income", "this_week_dev_regent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -11_560;\n            lab.Ledger.ThisWeek.CapitalIn = 18_460;", "11,560", "18,460"),
    ("LastWeekDevRegentSpendIncomeSummaryLine", "lastWeekDevRegentSpendIncome", "last week dev regent spend income", "last_week_dev_regent_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -11_360;\n            lab.Ledger.LastWeek.CapitalIn = 18_260;", "11,360", "18,260"),
    ("LifetimeDevRegentSpendIncomeSummaryLine", "lifetimeDevRegentSpendIncome", "lifetime dev regent spend income", "lifetime_dev_regent_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -178_700;\n            lab.Ledger.Lifetime.CapitalIn = 2_413_200;", "178,700", "2,413,200"),
]

SPRINT839 = [
    ("ThisWeekRegentDevSpendIncomeSummaryLine", "thisWeekRegentDevSpendIncome", "this week regent dev spend income", "this_week_regent_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 18_470;\n            lab.Ledger.ThisWeek.ChipDevelopment = -11_570;", "18,470", "11,570"),
    ("LastWeekRegentDevSpendIncomeSummaryLine", "lastWeekRegentDevSpendIncome", "last week regent dev spend income", "last_week_regent_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 18_270;\n            lab.Ledger.LastWeek.ChipDevelopment = -11_370;", "18,270", "11,370"),
    ("LifetimeRegentDevSpendIncomeSummaryLine", "lifetimeRegentDevSpendIncome", "lifetime regent dev spend income", "lifetime_regent_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.CapitalIn = 2_413_400;\n            lab.Ledger.Lifetime.ChipDevelopment = -178_900;", "2,413,400", "178,900"),
    ("ThisWeekDevKindleSpendIncomeSummaryLine", "thisWeekDevKindleSpendIncome", "this week dev kindle spend income", "this_week_dev_kindle_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -11_580;\n            lab.Ledger.ThisWeek.PatentLicense = -2_490;", "11,580", "2,490"),
    ("LastWeekDevKindleSpendIncomeSummaryLine", "lastWeekDevKindleSpendIncome", "last week dev kindle spend income", "last_week_dev_kindle_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -11_380;\n            lab.Ledger.LastWeek.PatentLicense = -2_470;", "11,380", "2,470"),
    ("LifetimeDevKindleSpendIncomeSummaryLine", "lifetimeDevKindleSpendIncome", "lifetime dev kindle spend income", "lifetime_dev_kindle_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -179_100;\n            lab.Ledger.Lifetime.PatentLicense = -39_600;", "179,100", "39,600"),
]

SPRINT840 = [
    ("ThisWeekKindleDevSpendIncomeSummaryLine", "thisWeekKindleDevSpendIncome", "this week kindle dev spend income", "this_week_kindle_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.PatentLicense = -2_500;\n            lab.Ledger.ThisWeek.ChipDevelopment = -11_590;", "2,500", "11,590"),
    ("LastWeekKindleDevSpendIncomeSummaryLine", "lastWeekKindleDevSpendIncome", "last week kindle dev spend income", "last_week_kindle_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = -2_480;\n            lab.Ledger.LastWeek.ChipDevelopment = -11_390;", "2,480", "11,390"),
    ("LifetimeKindleDevSpendIncomeSummaryLine", "lifetimeKindleDevSpendIncome", "lifetime kindle dev spend income", "lifetime_kindle_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.PatentLicense = -39_800;\n            lab.Ledger.Lifetime.ChipDevelopment = -179_300;", "39,800", "179,300"),
    ("ThisWeekDevCitadelSpendIncomeSummaryLine", "thisWeekDevCitadelSpendIncome", "this week dev citadel spend income", "this_week_dev_citadel_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -11_600;\n            lab.Ledger.ThisWeek.SiteBuild = -6_900;", "11,600", "6,900"),
    ("LastWeekDevCitadelSpendIncomeSummaryLine", "lastWeekDevCitadelSpendIncome", "last week dev citadel spend income", "last_week_dev_citadel_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -11_400;\n            lab.Ledger.LastWeek.SiteBuild = -6_880;", "11,400", "6,880"),
    ("LifetimeDevCitadelSpendIncomeSummaryLine", "lifetimeDevCitadelSpendIncome", "lifetime dev citadel spend income", "lifetime_dev_citadel_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -179_500;\n            lab.Ledger.Lifetime.SiteBuild = -389_600;", "179,500", "389,600"),
]

PA837 = '            string lifetimeDevStumpSpendIncomePart = string.IsNullOrEmpty(lifetimeDevStumpSpendIncome) ? "" : " · " + lifetimeDevStumpSpendIncome;'
PA838 = '            string lifetimeDevShawlSpendIncomePart = string.IsNullOrEmpty(lifetimeDevShawlSpendIncome) ? "" : " · " + lifetimeDevShawlSpendIncome;'
PA839 = '            string lifetimeDevRegentSpendIncomePart = string.IsNullOrEmpty(lifetimeDevRegentSpendIncome) ? "" : " · " + lifetimeDevRegentSpendIncome;'
PA840 = '            string lifetimeDevKindleSpendIncomePart = string.IsNullOrEmpty(lifetimeDevKindleSpendIncome) ? "" : " · " + lifetimeDevKindleSpendIncome;'

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



insert_formatters(F837_838)
git_commit("feat(hud): sprint837-838 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT837, PA837, "lifetimeDevStumpSpendIncome")
run_sprint(SPRINT838, PA838, "lifetimeDevShawlSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F839_840)
git_commit("feat(hud): sprint839-840 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT839, PA839, "lifetimeDevRegentSpendIncome")
run_sprint(SPRINT840, PA840, "lifetimeDevKindleSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
