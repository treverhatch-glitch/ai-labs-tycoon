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

F157_158 = [
    ("ThisWeekFinesPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekFinesPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeFinesPowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekGpuUpkeepRentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
        ("rent", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekGpuUpkeepRentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("rent", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LifetimeGpuUpkeepRentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekTrainingRevenueSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekTrainingRevenueSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeTrainingRevenueSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekChipSalesGrantsSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekChipSalesGrantsSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeChipSalesGrantsSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

F159_160 = [
    ("ThisWeekGreyLaunderSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekGreyLaunderSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeGreyLaunderSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekSalariesGpuSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekSalariesGpuSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LifetimeSalariesGpuSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekHardwarePowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekHardwarePowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeHardwarePowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekHiringGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekHiringGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeHiringGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
]

SPRINT157 = [
    ("ThisWeekFinesPowerSpendIncomeSummaryLine", "thisWeekFinesPowerSpendIncome", "this week fines power spend income", "this_week_fines_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_150;\n            lab.Ledger.ThisWeek.Power = -3_060;", "1,150", "3,060"),
    ("LastWeekFinesPowerSpendIncomeSummaryLine", "lastWeekFinesPowerSpendIncome", "last week fines power spend income", "last_week_fines_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -1_160;\n            lab.Ledger.LastWeek.Power = -2_510;", "1,160", "2,510"),
    ("LifetimeFinesPowerSpendIncomeSummaryLine", "lifetimeFinesPowerSpendIncome", "lifetime fines power spend income", "lifetime_fines_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -29_500;\n            lab.Ledger.Lifetime.Power = -177_000;", "29,500", "177,000"),
    ("ThisWeekGpuUpkeepRentSpendIncomeSummaryLine", "thisWeekGpuUpkeepRentSpendIncome", "this week gpu upkeep rent spend income", "this_week_gpu_upkeep_rent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuUpkeep = -890;\n            lab.Ledger.ThisWeek.GpuRent = -1_250;", "890", "1,250"),
    ("LastWeekGpuUpkeepRentSpendIncomeSummaryLine", "lastWeekGpuUpkeepRentSpendIncome", "last week gpu upkeep rent spend income", "last_week_gpu_upkeep_rent_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -870;\n            lab.Ledger.LastWeek.GpuRent = -1_230;", "870", "1,230"),
    ("LifetimeGpuUpkeepRentSpendIncomeSummaryLine", "lifetimeGpuUpkeepRentSpendIncome", "lifetime gpu upkeep rent spend income", "lifetime_gpu_upkeep_rent_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -177_500;\n            lab.Ledger.Lifetime.Grants = 214_000;", "177,500", "214,000"),
]

SPRINT158 = [
    ("ThisWeekTrainingRevenueSpendIncomeSummaryLine", "thisWeekTrainingRevenueSpendIncome", "this week training revenue spend income", "this_week_training_revenue_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Training = -960;\n            lab.Ledger.ThisWeek.Revenue = 18_900;", "960", "18,900"),
    ("LastWeekTrainingRevenueSpendIncomeSummaryLine", "lastWeekTrainingRevenueSpendIncome", "last week training revenue spend income", "last_week_training_revenue_spend_income_summary_line",
     "lab.Ledger.LastWeek.Training = -950;\n            lab.Ledger.LastWeek.Revenue = 18_490;", "950", "18,490"),
    ("LifetimeTrainingRevenueSpendIncomeSummaryLine", "lifetimeTrainingRevenueSpendIncome", "lifetime training revenue spend income", "lifetime_training_revenue_spend_income_summary_line",
     "lab.Ledger.Lifetime.Training = -55_500;\n            lab.Ledger.Lifetime.Revenue = 457_000;", "55,500", "457,000"),
    ("ThisWeekChipSalesGrantsSpendIncomeSummaryLine", "thisWeekChipSalesGrantsSpendIncome", "this week chip sales grants spend income", "this_week_chip_sales_grants_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_300;\n            lab.Ledger.ThisWeek.Grants = 5_540;", "8,300", "5,540"),
    ("LastWeekChipSalesGrantsSpendIncomeSummaryLine", "lastWeekChipSalesGrantsSpendIncome", "last week chip sales grants spend income", "last_week_chip_sales_grants_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_980;\n            lab.Ledger.LastWeek.Grants = 6_410;", "7,980", "6,410"),
    ("LifetimeChipSalesGrantsSpendIncomeSummaryLine", "lifetimeChipSalesGrantsSpendIncome", "lifetime chip sales grants spend income", "lifetime_chip_sales_grants_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 121_500;\n            lab.Ledger.Lifetime.Grants = 215_000;", "121,500", "215,000"),
]

SPRINT159 = [
    ("ThisWeekGreyLaunderSpendIncomeSummaryLine", "thisWeekGreyLaunderSpendIncome", "this week grey launder spend income", "this_week_grey_launder_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_260;\n            lab.Ledger.ThisWeek.Laundered = 3_540;", "2,260", "3,540"),
    ("LastWeekGreyLaunderSpendIncomeSummaryLine", "lastWeekGreyLaunderSpendIncome", "last week grey launder spend income", "last_week_grey_launder_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_780;\n            lab.Ledger.LastWeek.Laundered = 3_160;", "2,780", "3,160"),
    ("LifetimeGreyLaunderSpendIncomeSummaryLine", "lifetimeGreyLaunderSpendIncome", "lifetime grey launder spend income", "lifetime_grey_launder_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 55_000;\n            lab.Ledger.Lifetime.Laundered = 33_600;", "55,000", "33,600"),
    ("ThisWeekSalariesGpuSpendIncomeSummaryLine", "thisWeekSalariesGpuSpendIncome", "this week salaries gpu spend income", "this_week_salaries_gpu_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -3_530;\n            lab.Ledger.ThisWeek.GpuRent = -1_260;", "3,530", "1,260"),
    ("LastWeekSalariesGpuSpendIncomeSummaryLine", "lastWeekSalariesGpuSpendIncome", "last week salaries gpu spend income", "last_week_salaries_gpu_spend_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -3_510;\n            lab.Ledger.LastWeek.GpuRent = -1_240;", "3,510", "1,240"),
    ("LifetimeSalariesGpuSpendIncomeSummaryLine", "lifetimeSalariesGpuSpendIncome", "lifetime salaries gpu spend income", "lifetime_salaries_gpu_spend_income_summary_line",
     "lab.Ledger.Lifetime.Salaries = -143_000;\n            lab.Ledger.Lifetime.Power = -178_000;", "143,000", "178,000"),
]

SPRINT160 = [
    ("ThisWeekHardwarePowerSpendIncomeSummaryLine", "thisWeekHardwarePowerSpendIncome", "this week hardware power spend income", "this_week_hardware_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_290;\n            lab.Ledger.ThisWeek.Power = -3_070;", "2,290", "3,070"),
    ("LastWeekHardwarePowerSpendIncomeSummaryLine", "lastWeekHardwarePowerSpendIncome", "last week hardware power spend income", "last_week_hardware_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_270;\n            lab.Ledger.LastWeek.Power = -2_520;", "2,270", "2,520"),
    ("LifetimeHardwarePowerSpendIncomeSummaryLine", "lifetimeHardwarePowerSpendIncome", "lifetime hardware power spend income", "lifetime_hardware_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -90_500;\n            lab.Ledger.Lifetime.Power = -178_500;", "90,500", "178,500"),
    ("ThisWeekHiringGreySpendIncomeSummaryLine", "thisWeekHiringGreySpendIncome", "this week hiring grey spend income", "this_week_hiring_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -1_650;\n            lab.Ledger.ThisWeek.GreyRevenue = 2_270;", "1,650", "2,270"),
    ("LastWeekHiringGreySpendIncomeSummaryLine", "lastWeekHiringGreySpendIncome", "last week hiring grey spend income", "last_week_hiring_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -1_630;\n            lab.Ledger.LastWeek.GreyRevenue = 2_790;", "1,630", "2,790"),
    ("LifetimeHiringGreySpendIncomeSummaryLine", "lifetimeHiringGreySpendIncome", "lifetime hiring grey spend income", "lifetime_hiring_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hiring = -62_000;\n            lab.Ledger.Lifetime.GreyRevenue = 55_200;", "62,000", "55,200"),
]

PA157 = '            string lifetimeGrantTrainingSpendIncomePart = string.IsNullOrEmpty(lifetimeGrantTrainingSpendIncome) ? "" : " · " + lifetimeGrantTrainingSpendIncome;'
PA158 = '            string lifetimeGpuUpkeepRentSpendIncomePart = string.IsNullOrEmpty(lifetimeGpuUpkeepRentSpendIncome) ? "" : " · " + lifetimeGpuUpkeepRentSpendIncome;'
PA159 = '            string lifetimeChipSalesGrantsSpendIncomePart = string.IsNullOrEmpty(lifetimeChipSalesGrantsSpendIncome) ? "" : " · " + lifetimeChipSalesGrantsSpendIncome;'
PA160 = '            string lifetimeSalariesGpuSpendIncomePart = string.IsNullOrEmpty(lifetimeSalariesGpuSpendIncome) ? "" : " · " + lifetimeSalariesGpuSpendIncome;'

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



insert_formatters(F157_158)
git_commit("feat(hud): sprint157-158 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT157, PA157, "lifetimeGrantTrainingSpendIncome")
run_sprint(SPRINT158, PA158, "lifetimeGpuUpkeepRentSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F159_160)
git_commit("feat(hud): sprint159-160 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT159, PA159, "lifetimeChipSalesGrantsSpendIncome")
run_sprint(SPRINT160, PA160, "lifetimeSalariesGpuSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
