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

F869_870 = [
    ("ThisWeekSnarlDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekSnarlDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeSnarlDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekDevFrockSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
    ]),
    ("LastWeekDevFrockSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
    ]),
    ("LifetimeDevFrockSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
    ]),
    ("ThisWeekFrockDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekFrockDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeFrockDevSpendIncomeSummaryLine", [
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

F871_872 = [
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

SPRINT869 = [
    ("ThisWeekSnarlDevSpendIncomeSummaryLine", "thisWeekSnarlDevSpendIncome", "this week snarl dev spend income", "this_week_snarl_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -930;\n            lab.Ledger.ThisWeek.ChipDevelopment = -51_530;", "930", "11,530"),
    ("LastWeekSnarlDevSpendIncomeSummaryLine", "lastWeekSnarlDevSpendIncome", "last week snarl dev spend income", "last_week_snarl_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -910;\n            lab.Ledger.LastWeek.ChipDevelopment = -51_330;", "910", "11,330"),
    ("LifetimeSnarlDevSpendIncomeSummaryLine", "lifetimeSnarlDevSpendIncome", "lifetime snarl dev spend income", "lifetime_snarl_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -44_200;\n            lab.Ledger.Lifetime.ChipDevelopment = -218_100;", "44,200", "178,100"),
    ("ThisWeekDevFrockSpendIncomeSummaryLine", "thisWeekDevFrockSpendIncome", "this week dev frock spend income", "this_week_dev_frock_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -51_540;\n            lab.Ledger.ThisWeek.Hardware = -4_350;", "11,540", "4,350"),
    ("LastWeekDevFrockSpendIncomeSummaryLine", "lastWeekDevFrockSpendIncome", "last week dev frock spend income", "last_week_dev_frock_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -51_340;\n            lab.Ledger.LastWeek.Hardware = -4_330;", "11,340", "4,330"),
    ("LifetimeDevFrockSpendIncomeSummaryLine", "lifetimeDevFrockSpendIncome", "lifetime dev frock spend income", "lifetime_dev_frock_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -218_300;\n            lab.Ledger.Lifetime.Hardware = -613_200;", "178,300", "613,200"),
]

SPRINT870 = [
    ("ThisWeekFrockDevSpendIncomeSummaryLine", "thisWeekFrockDevSpendIncome", "this week frock dev spend income", "this_week_frock_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_360;\n            lab.Ledger.ThisWeek.ChipDevelopment = -51_550;", "4,360", "11,550"),
    ("LastWeekFrockDevSpendIncomeSummaryLine", "lastWeekFrockDevSpendIncome", "last week frock dev spend income", "last_week_frock_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_340;\n            lab.Ledger.LastWeek.ChipDevelopment = -51_350;", "4,340", "11,350"),
    ("LifetimeFrockDevSpendIncomeSummaryLine", "lifetimeFrockDevSpendIncome", "lifetime frock dev spend income", "lifetime_frock_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hardware = -613_400;\n            lab.Ledger.Lifetime.ChipDevelopment = -218_500;", "613,400", "178,500"),
    ("ThisWeekDevRegentSpendIncomeSummaryLine", "thisWeekDevRegentSpendIncome", "this week dev regent spend income", "this_week_dev_regent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -51_560;\n            lab.Ledger.ThisWeek.CapitalIn = 18_460;", "11,560", "18,460"),
    ("LastWeekDevRegentSpendIncomeSummaryLine", "lastWeekDevRegentSpendIncome", "last week dev regent spend income", "last_week_dev_regent_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -51_360;\n            lab.Ledger.LastWeek.CapitalIn = 18_260;", "11,360", "18,260"),
    ("LifetimeDevRegentSpendIncomeSummaryLine", "lifetimeDevRegentSpendIncome", "lifetime dev regent spend income", "lifetime_dev_regent_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -218_700;\n            lab.Ledger.Lifetime.CapitalIn = 2_413_200;", "178,700", "2,413,200"),
]

SPRINT871 = [
    ("ThisWeekRegentDevSpendIncomeSummaryLine", "thisWeekRegentDevSpendIncome", "this week regent dev spend income", "this_week_regent_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 18_470;\n            lab.Ledger.ThisWeek.ChipDevelopment = -51_570;", "18,470", "11,570"),
    ("LastWeekRegentDevSpendIncomeSummaryLine", "lastWeekRegentDevSpendIncome", "last week regent dev spend income", "last_week_regent_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 18_270;\n            lab.Ledger.LastWeek.ChipDevelopment = -51_370;", "18,270", "11,370"),
    ("LifetimeRegentDevSpendIncomeSummaryLine", "lifetimeRegentDevSpendIncome", "lifetime regent dev spend income", "lifetime_regent_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.CapitalIn = 2_413_400;\n            lab.Ledger.Lifetime.ChipDevelopment = -218_900;", "2,413,400", "178,900"),
    ("ThisWeekDevKindleSpendIncomeSummaryLine", "thisWeekDevKindleSpendIncome", "this week dev kindle spend income", "this_week_dev_kindle_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -51_580;\n            lab.Ledger.ThisWeek.PatentLicense = -2_490;", "11,580", "2,490"),
    ("LastWeekDevKindleSpendIncomeSummaryLine", "lastWeekDevKindleSpendIncome", "last week dev kindle spend income", "last_week_dev_kindle_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -51_380;\n            lab.Ledger.LastWeek.PatentLicense = -2_470;", "11,380", "2,470"),
    ("LifetimeDevKindleSpendIncomeSummaryLine", "lifetimeDevKindleSpendIncome", "lifetime dev kindle spend income", "lifetime_dev_kindle_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -219_100;\n            lab.Ledger.Lifetime.PatentLicense = -39_600;", "179,100", "39,600"),
]

SPRINT872 = [
    ("ThisWeekKindleDevSpendIncomeSummaryLine", "thisWeekKindleDevSpendIncome", "this week kindle dev spend income", "this_week_kindle_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.PatentLicense = -2_500;\n            lab.Ledger.ThisWeek.ChipDevelopment = -51_590;", "2,500", "11,590"),
    ("LastWeekKindleDevSpendIncomeSummaryLine", "lastWeekKindleDevSpendIncome", "last week kindle dev spend income", "last_week_kindle_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = -2_480;\n            lab.Ledger.LastWeek.ChipDevelopment = -51_390;", "2,480", "11,390"),
    ("LifetimeKindleDevSpendIncomeSummaryLine", "lifetimeKindleDevSpendIncome", "lifetime kindle dev spend income", "lifetime_kindle_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.PatentLicense = -39_800;\n            lab.Ledger.Lifetime.ChipDevelopment = -219_300;", "39,800", "179,300"),
    ("ThisWeekDevCitadelSpendIncomeSummaryLine", "thisWeekDevCitadelSpendIncome", "this week dev citadel spend income", "this_week_dev_citadel_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -51_600;\n            lab.Ledger.ThisWeek.SiteBuild = -6_900;", "11,600", "6,900"),
    ("LastWeekDevCitadelSpendIncomeSummaryLine", "lastWeekDevCitadelSpendIncome", "last week dev citadel spend income", "last_week_dev_citadel_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -51_400;\n            lab.Ledger.LastWeek.SiteBuild = -6_880;", "11,400", "6,880"),
    ("LifetimeDevCitadelSpendIncomeSummaryLine", "lifetimeDevCitadelSpendIncome", "lifetime dev citadel spend income", "lifetime_dev_citadel_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -219_500;\n            lab.Ledger.Lifetime.SiteBuild = -389_600;", "179,500", "389,600"),
]

PA869 = '            string lifetimeDevSnarlSpendIncomePart = string.IsNullOrEmpty(lifetimeDevSnarlSpendIncome) ? "" : " · " + lifetimeDevSnarlSpendIncome;'
PA870 = '            string lifetimeDevFrockSpendIncomePart = string.IsNullOrEmpty(lifetimeDevFrockSpendIncome) ? "" : " · " + lifetimeDevFrockSpendIncome;'
PA871 = '            string lifetimeDevRegentSpendIncomePart = string.IsNullOrEmpty(lifetimeDevRegentSpendIncome) ? "" : " · " + lifetimeDevRegentSpendIncome;'
PA872 = '            string lifetimeDevKindleSpendIncomePart = string.IsNullOrEmpty(lifetimeDevKindleSpendIncome) ? "" : " · " + lifetimeDevKindleSpendIncome;'

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



insert_formatters(F869_870)
git_commit("feat(hud): sprint869-870 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT869, PA869, "lifetimeDevSnarlSpendIncome")
run_sprint(SPRINT870, PA870, "lifetimeDevFrockSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F871_872)
git_commit("feat(hud): sprint871-872 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT871, PA871, "lifetimeDevRegentSpendIncome")
run_sprint(SPRINT872, PA872, "lifetimeDevKindleSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
