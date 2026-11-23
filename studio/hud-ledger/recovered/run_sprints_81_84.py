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





F81_82 = [
    ("ThisWeekSpendPatentSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSpendPatentSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSpendPatentSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLaunderPatentSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekIncomeNetSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekIncomeNetSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeIncomeNetSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendGreySummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekSpendGreySummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeSpendGreySummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekIncomePowerSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekIncomeDevSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
]

F83_84 = [
    ("LifetimeIncomePowerSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHiringGrantSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekTrainingGrantSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeHardwareGrantSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekConstructionGrantSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPowerChipSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekDevIncomeSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekDevIncomeSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeDevIncomeSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekNetIncomeSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekNetIncomeSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeNetIncomeSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT81 = [
    ("ThisWeekSpendPatentSummaryLine", "thisWeekSpendPatent", "this week spend patent", "this_week_spend_patent_summary_line",
     "lab.Ledger.ThisWeek.Fines = -620;\n            lab.Ledger.ThisWeek.Grants = 4_700;", "620", "4,700"),
    ("LastWeekSpendPatentSummaryLine", "lastWeekSpendPatent", "last week spend patent", "last_week_spend_patent_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_640;\n            lab.Ledger.LastWeek.Fines = -650;", "5,640", "650"),
    ("LifetimeSpendPatentSummaryLine", "lifetimeSpendPatent", "lifetime spend patent", "lifetime_spend_patent_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_000;\n            lab.Ledger.Lifetime.Grants = 145_000;", "26,000", "145,000"),
    ("ThisWeekLaunderPatentSummaryLine", "thisWeekLaunderPatent", "this week launder patent", "this_week_launder_patent_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_980;\n            lab.Ledger.ThisWeek.Fines = -630;", "1,980", "630"),
    ("ThisWeekIncomeNetSummaryLine", "thisWeekIncomeNet", "this week income net", "this_week_income_net_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_120;\n            lab.Ledger.ThisWeek.Revenue = 18_450;", "3,120", "18,450"),
    ("LastWeekIncomeNetSummaryLine", "lastWeekIncomeNet", "last week income net", "last_week_income_net_summary_line",
     "lab.Ledger.LastWeek.Net = 2_780;\n            lab.Ledger.LastWeek.Grants = 5_660;", "2,780", "5,660"),
]

SPRINT82 = [
    ("LifetimeIncomeNetSummaryLine", "lifetimeIncomeNet", "lifetime income net", "lifetime_income_net_summary_line",
     "lab.Ledger.Lifetime.Revenue = 419_000;\n            lab.Ledger.Lifetime.Net = 95_000;", "419,000", "95,000"),
    ("ThisWeekSpendGreySummaryLine", "thisWeekSpendGrey", "this week spend grey", "this_week_spend_grey_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_000;\n            lab.Ledger.ThisWeek.Fines = -640;", "2,000", "640"),
    ("LastWeekSpendGreySummaryLine", "lastWeekSpendGrey", "last week spend grey", "last_week_spend_grey_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_520;\n            lab.Ledger.LastWeek.Laundered = 3_350;", "2,520", "3,350"),
    ("LifetimeSpendGreySummaryLine", "lifetimeSpendGrey", "lifetime spend grey", "lifetime_spend_grey_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_000;\n            lab.Ledger.Lifetime.Fines = -26_100;", "52,000", "26,100"),
    ("ThisWeekIncomePowerSummaryLine", "thisWeekIncomePower", "this week income power", "this_week_income_power_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_820;\n            lab.Ledger.ThisWeek.Revenue = 18_470;", "2,820", "18,470"),
    ("LastWeekIncomeDevSummaryLine", "lastWeekIncomeDev", "last week income dev", "last_week_income_dev_summary_line",
     "lab.Ledger.LastWeek.Revenue = 17_950;\n            lab.Ledger.LastWeek.Grants = 5_680;", "17,950", "5,680"),
]

SPRINT83 = [
    ("LifetimeIncomePowerSummaryLine", "lifetimeIncomePower", "lifetime income power", "lifetime_income_power_summary_line",
     "lab.Ledger.Lifetime.Revenue = 420_000;\n            lab.Ledger.Lifetime.Power = -158_000;", "420,000", "158,000"),
    ("ThisWeekHiringGrantSummaryLine", "thisWeekHiringGrant", "this week hiring grant", "this_week_hiring_grant_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_820;\n            lab.Ledger.ThisWeek.Grants = 4_720;", "7,820", "4,720"),
    ("LastWeekTrainingGrantSummaryLine", "lastWeekTrainingGrant", "last week training grant", "last_week_training_grant_summary_line",
     "lab.Ledger.LastWeek.Training = -9_100;\n            lab.Ledger.LastWeek.Grants = 5_700;", "9,100", "5,700"),
    ("LifetimeHardwareGrantSummaryLine", "lifetimeHardwareGrant", "lifetime hardware grant", "lifetime_hardware_grant_summary_line",
     "lab.Ledger.Lifetime.Power = -159_000;\n            lab.Ledger.Lifetime.Grants = 146_000;", "159,000", "146,000"),
    ("ThisWeekConstructionGrantSummaryLine", "thisWeekConstructionGrant", "this week construction grant", "this_week_construction_grant_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_150;\n            lab.Ledger.ThisWeek.Grants = 4_740;", "6,150", "4,740"),
    ("LastWeekPowerChipSummaryLine", "lastWeekPowerChip", "last week power chip", "last_week_power_chip_summary_line",
     "lab.Ledger.LastWeek.Power = -3_640;\n            lab.Ledger.LastWeek.ChipSales = 7_280;", "3,640", "7,280"),
]

SPRINT84 = [
    ("ThisWeekDevIncomeSummaryLine", "thisWeekDevIncome", "this week dev income", "this_week_dev_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_510;\n            lab.Ledger.ThisWeek.Revenue = 18_490;", "5,510", "18,490"),
    ("LastWeekDevIncomeSummaryLine", "lastWeekDevIncome", "last week dev income", "last_week_dev_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_300;\n            lab.Ledger.LastWeek.Grants = 5_720;", "7,300", "5,720"),
    ("LifetimeDevIncomeSummaryLine", "lifetimeDevIncome", "lifetime dev income", "lifetime_dev_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 249_000;\n            lab.Ledger.Lifetime.Revenue = 421_000;", "249,000", "421,000"),
    ("ThisWeekNetIncomeSummaryLine", "thisWeekNetIncome", "this week net income", "this_week_net_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_130;\n            lab.Ledger.ThisWeek.Grants = 4_760;", "3,130", "4,760"),
    ("LastWeekNetIncomeSummaryLine", "lastWeekNetIncome", "last week net income", "last_week_net_income_summary_line",
     "lab.Ledger.LastWeek.Net = 2_800;\n            lab.Ledger.LastWeek.Revenue = 17_980;", "2,800", "17,980"),
    ("LifetimeNetIncomeSummaryLine", "lifetimeNetIncome", "lifetime net income", "lifetime_net_income_summary_line",
     "lab.Ledger.Lifetime.Net = 96_000;\n            lab.Ledger.Lifetime.Grants = 147_000;", "96,000", "147,000"),
]

PA81 = '            string lifetimeIncomePatentPart = string.IsNullOrEmpty(lifetimeIncomePatent) ? "" : " · " + lifetimeIncomePatent;'
PA82 = '            string lastWeekIncomeNetPart = string.IsNullOrEmpty(lastWeekIncomeNet) ? "" : " · " + lastWeekIncomeNet;'
PA83 = '            string lastWeekIncomeDevPart = string.IsNullOrEmpty(lastWeekIncomeDev) ? "" : " · " + lastWeekIncomeDev;'
PA84 = '            string lastWeekPowerChipPart = string.IsNullOrEmpty(lastWeekPowerChip) ? "" : " · " + lastWeekPowerChip;'


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


insert_formatters(F81_82)
git_commit("feat(hud): sprint81-82 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT81, PA81, "lifetimeIncomePatentPart")
run_sprint(SPRINT82, PA82, "lastWeekIncomeNetPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F83_84)
git_commit("feat(hud): sprint83-84 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT83, PA83, "lastWeekIncomeDevPart")
run_sprint(SPRINT84, PA84, "lastWeekPowerChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
