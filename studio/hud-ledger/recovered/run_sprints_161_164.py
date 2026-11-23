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

F161_162 = [
    ("ThisWeekRevenueNetSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekRevenueNetSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeRevenueNetSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekChipDevLandSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
    ]),
    ("LastWeekChipDevLandSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("LifetimeChipDevLandSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
    ]),
    ("ThisWeekConstructionGrantsSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekConstructionGrantsSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeConstructionGrantsSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekTrainingLaunderSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekTrainingLaunderSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeTrainingLaunderSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
]

F163_164 = [
    ("ThisWeekPowerChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekPowerChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimePowerChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("ThisWeekGrantHardwareSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
    ]),
    ("LastWeekGrantHardwareSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
    ]),
    ("LifetimeGrantHardwareSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
    ]),
    ("ThisWeekPowerFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPowerFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimePowerFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGrantsFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekGrantsFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeGrantsFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

SPRINT161 = [
    ("ThisWeekRevenueNetSpendIncomeSummaryLine", "thisWeekRevenueNetSpendIncome", "this week revenue net spend income", "this_week_revenue_net_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_910;\n            lab.Ledger.ThisWeek.Net = 3_640;", "18,910", "3,640"),
    ("LastWeekRevenueNetSpendIncomeSummaryLine", "lastWeekRevenueNetSpendIncome", "last week revenue net spend income", "last_week_revenue_net_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_500;\n            lab.Ledger.LastWeek.Net = 3_650;", "18,500", "3,650"),
    ("LifetimeRevenueNetSpendIncomeSummaryLine", "lifetimeRevenueNetSpendIncome", "lifetime revenue net spend income", "lifetime_revenue_net_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 458_000;\n            lab.Ledger.Lifetime.Net = 126_500;", "458,000", "126,500"),
    ("ThisWeekChipDevLandSpendIncomeSummaryLine", "thisWeekChipDevLandSpendIncome", "this week chip dev land spend income", "this_week_chip_dev_land_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -4_160;\n            lab.Ledger.ThisWeek.Land = -2_840;", "4,160", "2,840"),
    ("LastWeekChipDevLandSpendIncomeSummaryLine", "lastWeekChipDevLandSpendIncome", "last week chip dev land spend income", "last_week_chip_dev_land_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -4_040;\n            lab.Ledger.LastWeek.Land = -2_820;", "4,040", "2,820"),
    ("LifetimeChipDevLandSpendIncomeSummaryLine", "lifetimeChipDevLandSpendIncome", "lifetime chip dev land spend income", "lifetime_chip_dev_land_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -95_500;\n            lab.Ledger.Lifetime.Land = -64_500;", "95,500", "64,500"),
]

SPRINT162 = [
    ("ThisWeekConstructionGrantsSpendIncomeSummaryLine", "thisWeekConstructionGrantsSpendIncome", "this week construction grants spend income", "this_week_construction_grants_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_300;\n            lab.Ledger.ThisWeek.Grants = 5_550;", "2,300", "5,550"),
    ("LastWeekConstructionGrantsSpendIncomeSummaryLine", "lastWeekConstructionGrantsSpendIncome", "last week construction grants spend income", "last_week_construction_grants_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_280;\n            lab.Ledger.LastWeek.Grants = 6_420;", "2,280", "6,420"),
    ("LifetimeConstructionGrantsSpendIncomeSummaryLine", "lifetimeConstructionGrantsSpendIncome", "lifetime construction grants spend income", "lifetime_construction_grants_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -91_000;\n            lab.Ledger.Lifetime.Grants = 216_000;", "91,000", "216,000"),
    ("ThisWeekTrainingLaunderSpendIncomeSummaryLine", "thisWeekTrainingLaunderSpendIncome", "this week training launder spend income", "this_week_training_launder_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Training = -970;\n            lab.Ledger.ThisWeek.Laundered = 3_550;", "970", "3,550"),
    ("LastWeekTrainingLaunderSpendIncomeSummaryLine", "lastWeekTrainingLaunderSpendIncome", "last week training launder spend income", "last_week_training_launder_spend_income_summary_line",
     "lab.Ledger.LastWeek.Training = -960;\n            lab.Ledger.LastWeek.Laundered = 3_170;", "960", "3,170"),
    ("LifetimeTrainingLaunderSpendIncomeSummaryLine", "lifetimeTrainingLaunderSpendIncome", "lifetime training launder spend income", "lifetime_training_launder_spend_income_summary_line",
     "lab.Ledger.Lifetime.Training = -56_000;\n            lab.Ledger.Lifetime.Laundered = 33_800;", "56,000", "33,800"),
]

SPRINT163 = [
    ("ThisWeekPowerChipSpendIncomeSummaryLine", "thisWeekPowerChipSpendIncome", "this week power chip spend income", "this_week_power_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -3_080;\n            lab.Ledger.ThisWeek.ChipSales = 8_310;", "3,080", "8,310"),
    ("LastWeekPowerChipSpendIncomeSummaryLine", "lastWeekPowerChipSpendIncome", "last week power chip spend income", "last_week_power_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.Power = -2_530;\n            lab.Ledger.LastWeek.ChipSales = 7_990;", "2,530", "7,990"),
    ("LifetimePowerChipSpendIncomeSummaryLine", "lifetimePowerChipSpendIncome", "lifetime power chip spend income", "lifetime_power_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -179_000;\n            lab.Ledger.Lifetime.ChipSales = 122_000;", "179,000", "122,000"),
    ("ThisWeekGrantHardwareSpendIncomeSummaryLine", "thisWeekGrantHardwareSpendIncome", "this week grant hardware spend income", "this_week_grant_hardware_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_560;\n            lab.Ledger.ThisWeek.Construction = -2_310;", "5,560", "2,310"),
    ("LastWeekGrantHardwareSpendIncomeSummaryLine", "lastWeekGrantHardwareSpendIncome", "last week grant hardware spend income", "last_week_grant_hardware_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_430;\n            lab.Ledger.LastWeek.Construction = -2_290;", "6,430", "2,290"),
    ("LifetimeGrantHardwareSpendIncomeSummaryLine", "lifetimeGrantHardwareSpendIncome", "lifetime grant hardware spend income", "lifetime_grant_hardware_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 217_000;\n            lab.Ledger.Lifetime.Construction = -91_500;", "217,000", "91,500"),
]

SPRINT164 = [
    ("ThisWeekPowerFinesSpendIncomeSummaryLine", "thisWeekPowerFinesSpendIncome", "this week power fines spend income", "this_week_power_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -3_090;\n            lab.Ledger.ThisWeek.Fines = -1_170;", "3,090", "1,170"),
    ("LastWeekPowerFinesSpendIncomeSummaryLine", "lastWeekPowerFinesSpendIncome", "last week power fines spend income", "last_week_power_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Power = -2_540;\n            lab.Ledger.LastWeek.Fines = -1_180;", "2,540", "1,180"),
    ("LifetimePowerFinesSpendIncomeSummaryLine", "lifetimePowerFinesSpendIncome", "lifetime power fines spend income", "lifetime_power_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -179_500;\n            lab.Ledger.Lifetime.Fines = -29_600;", "179,500", "29,600"),
    ("ThisWeekGrantsFinesSpendIncomeSummaryLine", "thisWeekGrantsFinesSpendIncome", "this week grants fines spend income", "this_week_grants_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_570;\n            lab.Ledger.ThisWeek.Fines = -1_190;", "5,570", "1,190"),
    ("LastWeekGrantsFinesSpendIncomeSummaryLine", "lastWeekGrantsFinesSpendIncome", "last week grants fines spend income", "last_week_grants_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_440;\n            lab.Ledger.LastWeek.Fines = -1_200;", "6,440", "1,200"),
    ("LifetimeGrantsFinesSpendIncomeSummaryLine", "lifetimeGrantsFinesSpendIncome", "lifetime grants fines spend income", "lifetime_grants_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 218_000;\n            lab.Ledger.Lifetime.Fines = -29_700;", "218,000", "29,700"),
]

PA161 = '            string lifetimeHiringGreySpendIncomePart = string.IsNullOrEmpty(lifetimeHiringGreySpendIncome) ? "" : " · " + lifetimeHiringGreySpendIncome;'
PA162 = '            string lifetimeChipDevLandSpendIncomePart = string.IsNullOrEmpty(lifetimeChipDevLandSpendIncome) ? "" : " · " + lifetimeChipDevLandSpendIncome;'
PA163 = '            string lifetimeConstructionGrantsSpendIncomePart = string.IsNullOrEmpty(lifetimeConstructionGrantsSpendIncome) ? "" : " · " + lifetimeConstructionGrantsSpendIncome;'
PA164 = '            string lifetimePowerChipSpendIncomePart = string.IsNullOrEmpty(lifetimePowerChipSpendIncome) ? "" : " · " + lifetimePowerChipSpendIncome;'

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



insert_formatters(F161_162)
git_commit("feat(hud): sprint161-162 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT161, PA161, "lifetimeHiringGreySpendIncome")
run_sprint(SPRINT162, PA162, "lifetimeChipDevLandSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F163_164)
git_commit("feat(hud): sprint163-164 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT163, PA163, "lifetimeConstructionGrantsSpendIncome")
run_sprint(SPRINT164, PA164, "lifetimePowerChipSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
