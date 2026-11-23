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

F125_126 = [
    ("ThisWeekFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimeChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("ThisWeekDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
]

F127_128 = [
    ("ThisWeekGpuSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekGpuSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LifetimeGpuSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("gpu", "LifetimeLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekRevenueSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRevenueSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeRevenueSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekGrantFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekGrantFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeGrantFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekChipDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekChipDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeChipDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
]

SPRINT125 = [
    ("ThisWeekFinesSpendIncomeSummaryLine", "thisWeekFinesSpendIncome", "this week fines spend income", "this_week_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -890;\n            lab.Ledger.ThisWeek.Grants = 5_230;", "890", "5,230"),
    ("LastWeekFinesSpendIncomeSummaryLine", "lastWeekFinesSpendIncome", "last week fines spend income", "last_week_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -900;\n            lab.Ledger.LastWeek.Revenue = 18_360;", "900", "18,360"),
    ("LifetimeFinesSpendIncomeSummaryLine", "lifetimeFinesSpendIncome", "lifetime fines spend income", "lifetime_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -28_200;\n            lab.Ledger.Lifetime.Net = 118_000;", "28,200", "118,000"),
    ("ThisWeekGrantSpendIncomeSummaryLine", "thisWeekGrantSpendIncome", "this week grant spend income", "this_week_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_240;\n            lab.Ledger.ThisWeek.Net = 3_410;", "5,240", "3,410"),
    ("LastWeekGrantSpendIncomeSummaryLine", "lastWeekGrantSpendIncome", "last week grant spend income", "last_week_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_180;\n            lab.Ledger.LastWeek.Fines = -910;", "6,180", "910"),
    ("LifetimeGrantSpendIncomeSummaryLine", "lifetimeGrantSpendIncome", "lifetime grant spend income", "lifetime_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 185_000;\n            lab.Ledger.Lifetime.Revenue = 447_000;", "185,000", "447,000"),
]

SPRINT126 = [
    ("ThisWeekChipSpendIncomeSummaryLine", "thisWeekChipSpendIncome", "this week chip spend income", "this_week_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_200;\n            lab.Ledger.ThisWeek.Grants = 5_250;", "8,200", "5,250"),
    ("LastWeekChipSpendIncomeSummaryLine", "lastWeekChipSpendIncome", "last week chip spend income", "last_week_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_890;\n            lab.Ledger.LastWeek.Revenue = 18_370;", "7,890", "18,370"),
    ("LifetimeChipSpendIncomeSummaryLine", "lifetimeChipSpendIncome", "lifetime chip spend income", "lifetime_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 112_000;\n            lab.Ledger.Lifetime.Net = 118_500;", "112,000", "118,500"),
    ("ThisWeekDevSpendIncomeSummaryLine", "thisWeekDevSpendIncome", "this week dev spend income", "this_week_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -4_100;\n            lab.Ledger.ThisWeek.Fines = -920;", "4,100", "920"),
    ("LastWeekDevSpendIncomeSummaryLine", "lastWeekDevSpendIncome", "last week dev spend income", "last_week_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -3_980;\n            lab.Ledger.LastWeek.Grants = 6_190;", "3,980", "6,190"),
    ("LifetimeDevSpendIncomeSummaryLine", "lifetimeDevSpendIncome", "lifetime dev spend income", "lifetime_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -92_000;\n            lab.Ledger.Lifetime.Grants = 186_000;", "92,000", "186,000"),
]

SPRINT127 = [
    ("ThisWeekGpuSpendIncomeSummaryLine", "thisWeekGpuSpendIncome", "this week gpu spend income", "this_week_gpu_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_200;\n            lab.Ledger.ThisWeek.Revenue = 18_820;", "1,200", "18,820"),
    ("LastWeekGpuSpendIncomeSummaryLine", "lastWeekGpuSpendIncome", "last week gpu spend income", "last_week_gpu_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_180;\n            lab.Ledger.LastWeek.Net = 3_420;", "1,180", "3,420"),
    ("LifetimeGpuSpendIncomeSummaryLine", "lifetimeGpuSpendIncome", "lifetime gpu spend income", "lifetime_gpu_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -168_000;\n            lab.Ledger.Lifetime.Revenue = 448_000;", "168,000", "448,000"),
    ("ThisWeekRevenueSpendIncomeSummaryLine", "thisWeekRevenueSpendIncome", "this week revenue spend income", "this_week_revenue_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_830;\n            lab.Ledger.ThisWeek.Grants = 5_260;", "18,830", "5,260"),
    ("LastWeekRevenueSpendIncomeSummaryLine", "lastWeekRevenueSpendIncome", "last week revenue spend income", "last_week_revenue_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_380;\n            lab.Ledger.LastWeek.Fines = -930;", "18,380", "930"),
    ("LifetimeRevenueSpendIncomeSummaryLine", "lifetimeRevenueSpendIncome", "lifetime revenue spend income", "lifetime_revenue_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 449_000;\n            lab.Ledger.Lifetime.Net = 119_000;", "449,000", "119,000"),
]

SPRINT128 = [
    ("ThisWeekGrantFinesSpendIncomeSummaryLine", "thisWeekGrantFinesSpendIncome", "this week grant fines spend income", "this_week_grant_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_270;\n            lab.Ledger.ThisWeek.Fines = -940;", "5,270", "940"),
    ("LastWeekGrantFinesSpendIncomeSummaryLine", "lastWeekGrantFinesSpendIncome", "last week grant fines spend income", "last_week_grant_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_200;\n            lab.Ledger.LastWeek.Fines = -950;", "6,200", "950"),
    ("LifetimeGrantFinesSpendIncomeSummaryLine", "lifetimeGrantFinesSpendIncome", "lifetime grant fines spend income", "lifetime_grant_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 187_000;\n            lab.Ledger.Lifetime.Fines = -28_300;", "187,000", "28,300"),
    ("ThisWeekChipDevSpendIncomeSummaryLine", "thisWeekChipDevSpendIncome", "this week chip dev spend income", "this_week_chip_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_210;\n            lab.Ledger.ThisWeek.ChipDevelopment = -4_110;", "8,210", "4,110"),
    ("LastWeekChipDevSpendIncomeSummaryLine", "lastWeekChipDevSpendIncome", "last week chip dev spend income", "last_week_chip_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_900;\n            lab.Ledger.LastWeek.ChipDevelopment = -3_990;", "7,900", "3,990"),
    ("LifetimeChipDevSpendIncomeSummaryLine", "lifetimeChipDevSpendIncome", "lifetime chip dev spend income", "lifetime_chip_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 113_000;\n            lab.Ledger.Lifetime.ChipDevelopment = -92_500;", "113,000", "92,500"),
]

PA125 = '            string lifetimeGreySpendIncomePart = string.IsNullOrEmpty(lifetimeGreySpendIncome) ? "" : " · " + lifetimeGreySpendIncome;'
PA126 = '            string lifetimeGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeGrantSpendIncome) ? "" : " · " + lifetimeGrantSpendIncome;'
PA127 = '            string lifetimeDevSpendIncomePart = string.IsNullOrEmpty(lifetimeDevSpendIncome) ? "" : " · " + lifetimeDevSpendIncome;'
PA128 = '            string lifetimeRevenueSpendIncomePart = string.IsNullOrEmpty(lifetimeRevenueSpendIncome) ? "" : " · " + lifetimeRevenueSpendIncome;'

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



insert_formatters(F125_126)
git_commit("feat(hud): sprint125-126 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT125, PA125, "lifetimeGreySpendIncome")
run_sprint(SPRINT126, PA126, "lifetimeGrantSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F127_128)
git_commit("feat(hud): sprint127-128 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT127, PA127, "lifetimeDevSpendIncome")
run_sprint(SPRINT128, PA128, "lifetimeRevenueSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
