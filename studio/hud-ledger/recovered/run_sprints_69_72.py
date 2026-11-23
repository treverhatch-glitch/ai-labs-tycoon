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


F69_70 = [
    ("ThisWeekTrainingPatentSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringRentSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeRevenueGrantSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekIncomeChipSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekUpkeepLandSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPatentChipSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekTrainingPatentSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeTrainingPatentSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendPowerSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekDevRentSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeSpendGrantSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekIncomeRentSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
]

F71_72 = [
    ("LastWeekDevPowerSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeIncomeChipSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHiringPowerSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekSpendChipSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeDevGrantSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekUpkeepChipSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekIncomePowerSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeHiringGrantSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekDevLandSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekUpkeepGrantSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekLandPatentSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHiringChipSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
]

SPRINT69 = [
    ("ThisWeekTrainingPatentSummaryLine", "thisWeekTrainingPatent", "this week training patent", "this_week_training_patent_summary_line",
     "lab.Ledger.ThisWeek.Training = -9_420;\n            lab.Ledger.ThisWeek.Fines = -520;", "9,420", "520"),
    ("LastWeekHiringRentSummaryLine", "lastWeekHiringRent", "last week hiring rent", "last_week_hiring_rent_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_100;\n            lab.Ledger.LastWeek.GpuRent = -2_360;", "3,100", "2,360"),
    ("LifetimeRevenueGrantSummaryLine", "lifetimeRevenueGrant", "lifetime revenue grant", "lifetime_revenue_grant_summary_line",
     "lab.Ledger.Lifetime.Revenue = 410_000;\n            lab.Ledger.Lifetime.Grants = 131_000;", "410,000", "131,000"),
    ("ThisWeekIncomeChipSummaryLine", "thisWeekIncomeChip", "this week income chip", "this_week_income_chip_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_320;\n            lab.Ledger.ThisWeek.Revenue = 18_200;", "5,320", "18,200"),
    ("LastWeekUpkeepLandSummaryLine", "lastWeekUpkeepLand", "last week upkeep land", "last_week_upkeep_land_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -1_980;\n            lab.Ledger.LastWeek.Land = -12_000;", "1,980", "12,000"),
    ("ThisWeekPatentChipSummaryLine", "thisWeekPatentChip", "this week patent chip", "this_week_patent_chip_summary_line",
     "lab.Ledger.ThisWeek.Fines = -530;\n            lab.Ledger.ThisWeek.ChipSales = 5_340;", "530", "5,340"),
]

SPRINT70 = [
    ("LastWeekTrainingPatentSummaryLine", "lastWeekTrainingPatent", "last week training patent", "last_week_training_patent_summary_line",
     "lab.Ledger.LastWeek.Training = -9_050;\n            lab.Ledger.LastWeek.Grants = 5_420;", "9,050", "5,420"),
    ("LifetimeTrainingPatentSummaryLine", "lifetimeTrainingPatent", "lifetime training patent", "lifetime_training_patent_summary_line",
     "lab.Ledger.Lifetime.Training = -150_000;\n            lab.Ledger.Lifetime.Grants = 132_000;", "150,000", "132,000"),
    ("ThisWeekSpendPowerSummaryLine", "thisWeekSpendPower", "this week spend power", "this_week_spend_power_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_780;\n            lab.Ledger.ThisWeek.Revenue = 18_250;", "2,780", "18,250"),
    ("ThisWeekDevRentSummaryLine", "thisWeekDevRent", "this week dev rent", "this_week_dev_rent_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_360;\n            lab.Ledger.ThisWeek.GpuRent = -2_090;", "5,360", "2,090"),
    ("LifetimeSpendGrantSummaryLine", "lifetimeSpendGrant", "lifetime spend grant", "lifetime_spend_grant_summary_line",
     "lab.Ledger.Lifetime.Grants = 133_000;\n            lab.Ledger.Lifetime.Fines = -24_500;", "133,000", "24,500"),
    ("ThisWeekIncomeRentSummaryLine", "thisWeekIncomeRent", "this week income rent", "this_week_income_rent_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_100;\n            lab.Ledger.ThisWeek.Net = 3_050;", "2,100", "3,050"),
]

SPRINT71 = [
    ("LastWeekDevPowerSummaryLine", "lastWeekDevPower", "last week dev power", "last_week_dev_power_summary_line",
     "lab.Ledger.LastWeek.Power = -3_540;\n            lab.Ledger.LastWeek.Revenue = 17_800;", "3,540", "17,800"),
    ("LifetimeIncomeChipSummaryLine", "lifetimeIncomeChip", "lifetime income chip", "lifetime_income_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 244_000;\n            lab.Ledger.Lifetime.Revenue = 412_000;", "244,000", "412,000"),
    ("ThisWeekHiringPowerSummaryLine", "thisWeekHiringPower", "this week hiring power", "this_week_hiring_power_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_760;\n            lab.Ledger.ThisWeek.Power = -2_790;", "7,760", "2,790"),
    ("LastWeekSpendChipSummaryLine", "lastWeekSpendChip", "last week spend chip", "last_week_spend_chip_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_240;\n            lab.Ledger.LastWeek.Grants = 5_450;", "7,240", "5,450"),
    ("LifetimeDevGrantSummaryLine", "lifetimeDevGrant", "lifetime dev grant", "lifetime_dev_grant_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 245_000;\n            lab.Ledger.Lifetime.Grants = 134_000;", "245,000", "134,000"),
    ("ThisWeekUpkeepChipSummaryLine", "thisWeekUpkeepChip", "this week upkeep chip", "this_week_upkeep_chip_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_110;\n            lab.Ledger.ThisWeek.ChipSales = 5_380;", "2,110", "5,380"),
]

SPRINT72 = [
    ("LastWeekIncomePowerSummaryLine", "lastWeekIncomePower", "last week income power", "last_week_income_power_summary_line",
     "lab.Ledger.LastWeek.Power = -3_560;\n            lab.Ledger.LastWeek.Net = 2_700;", "3,560", "2,700"),
    ("LifetimeHiringGrantSummaryLine", "lifetimeHiringGrant", "lifetime hiring grant", "lifetime_hiring_grant_summary_line",
     "lab.Ledger.Lifetime.Training = -151_000;\n            lab.Ledger.Lifetime.Grants = 135_000;", "151,000", "135,000"),
    ("ThisWeekDevLandSummaryLine", "thisWeekDevLand", "this week dev land", "this_week_dev_land_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_400;\n            lab.Ledger.ThisWeek.Land = -11_250;", "5,400", "11,250"),
    ("LastWeekUpkeepGrantSummaryLine", "lastWeekUpkeepGrant", "last week upkeep grant", "last_week_upkeep_grant_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -2_000;\n            lab.Ledger.LastWeek.Grants = 5_480;", "2,000", "5,480"),
    ("ThisWeekLandPatentSummaryLine", "thisWeekLandPatent", "this week land patent", "this_week_land_patent_summary_line",
     "lab.Ledger.ThisWeek.Land = -11_300;\n            lab.Ledger.ThisWeek.Fines = -540;", "11,300", "540"),
    ("ThisWeekHiringChipSummaryLine", "thisWeekHiringChip", "this week hiring chip", "this_week_hiring_chip_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_780;\n            lab.Ledger.ThisWeek.ChipSales = 5_410;", "7,780", "5,410"),
]

PA69 = '            string thisWeekGreyRoomPart = string.IsNullOrEmpty(thisWeekGreyRoom) ? "" : " · " + thisWeekGreyRoom;'
PA70 = '            string thisWeekPatentChipPart = string.IsNullOrEmpty(thisWeekPatentChip) ? "" : " · " + thisWeekPatentChip;'
PA71 = '            string thisWeekIncomeRentPart = string.IsNullOrEmpty(thisWeekIncomeRent) ? "" : " · " + thisWeekIncomeRent;'
PA72 = '            string thisWeekUpkeepChipPart = string.IsNullOrEmpty(thisWeekUpkeepChip) ? "" : " · " + thisWeekUpkeepChip;'


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


insert_formatters(F69_70)
git_commit("feat(hud): sprint69-70 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT69, PA69, "thisWeekGreyRoomPart")
run_sprint(SPRINT70, PA70, "thisWeekPatentChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F71_72)
git_commit("feat(hud): sprint71-72 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT71, PA71, "thisWeekIncomeRentPart")
run_sprint(SPRINT72, PA72, "thisWeekUpkeepChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
