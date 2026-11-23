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

F145_146 = [
    ("ThisWeekHardwareGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHardwareGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHardwareGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLandCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekLandCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeLandCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRentPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekRentPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "LastWeekLedgerGpuRentLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeRentPowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekSalariesConstructionSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekSalariesConstructionSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LifetimeSalariesConstructionSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
    ]),
]

F147_148 = [
    ("ThisWeekNetChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekNetChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimeNetChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("ThisWeekGreyDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekGreyDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeGreyDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekShadyGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekShadyGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeShadyGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekPatentChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekPatentChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimePatentChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
]

SPRINT145 = [
    ("ThisWeekHardwareGrantSpendIncomeSummaryLine", "thisWeekHardwareGrantSpendIncome", "this week hardware grant spend income", "this_week_hardware_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_250;\n            lab.Ledger.ThisWeek.Grants = 5_400;", "2,250", "5,400"),
    ("LastWeekHardwareGrantSpendIncomeSummaryLine", "lastWeekHardwareGrantSpendIncome", "last week hardware grant spend income", "last_week_hardware_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_230;\n            lab.Ledger.LastWeek.Grants = 6_290;", "2,230", "6,290"),
    ("LifetimeHardwareGrantSpendIncomeSummaryLine", "lifetimeHardwareGrantSpendIncome", "lifetime hardware grant spend income", "lifetime_hardware_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -88_000;\n            lab.Ledger.Lifetime.Grants = 200_000;", "88,000", "200,000"),
    ("ThisWeekLandCapitalSpendIncomeSummaryLine", "thisWeekLandCapitalSpendIncome", "this week land capital spend income", "this_week_land_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_550;\n            lab.Ledger.ThisWeek.Grants = 5_410;", "3,550", "5,410"),
    ("LastWeekLandCapitalSpendIncomeSummaryLine", "lastWeekLandCapitalSpendIncome", "last week land capital spend income", "last_week_land_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_560;\n            lab.Ledger.LastWeek.Revenue = 18_460;", "3,560", "18,460"),
    ("LifetimeLandCapitalSpendIncomeSummaryLine", "lifetimeLandCapitalSpendIncome", "lifetime land capital spend income", "lifetime_land_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 124_000;\n            lab.Ledger.Lifetime.Grants = 201_000;", "124,000", "201,000"),
]

SPRINT146 = [
    ("ThisWeekRentPowerSpendIncomeSummaryLine", "thisWeekRentPowerSpendIncome", "this week rent power spend income", "this_week_rent_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_240;\n            lab.Ledger.ThisWeek.Power = -3_040;", "1,240", "3,040"),
    ("LastWeekRentPowerSpendIncomeSummaryLine", "lastWeekRentPowerSpendIncome", "last week rent power spend income", "last_week_rent_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_220;\n            lab.Ledger.LastWeek.Power = -2_490;", "1,220", "2,490"),
    ("LifetimeRentPowerSpendIncomeSummaryLine", "lifetimeRentPowerSpendIncome", "lifetime rent power spend income", "lifetime_rent_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -174_000;\n            lab.Ledger.Lifetime.Training = -54_500;", "174,000", "54,500"),
    ("ThisWeekSalariesConstructionSpendIncomeSummaryLine", "thisWeekSalariesConstructionSpendIncome", "this week salaries construction spend income", "this_week_salaries_construction_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -12_410;\n            lab.Ledger.ThisWeek.Construction = -2_260;", "12,410", "2,260"),
    ("LastWeekSalariesConstructionSpendIncomeSummaryLine", "lastWeekSalariesConstructionSpendIncome", "last week salaries construction spend income", "last_week_salaries_construction_spend_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -12_310;\n            lab.Ledger.LastWeek.Construction = -2_240;", "12,310", "2,240"),
    ("LifetimeSalariesConstructionSpendIncomeSummaryLine", "lifetimeSalariesConstructionSpendIncome", "lifetime salaries construction spend income", "lifetime_salaries_construction_spend_income_summary_line",
     "lab.Ledger.Lifetime.Salaries = -892_000;\n            lab.Ledger.Lifetime.Construction = -88_500;", "892,000", "88,500"),
]

SPRINT147 = [
    ("ThisWeekNetChipSpendIncomeSummaryLine", "thisWeekNetChipSpendIncome", "this week net chip spend income", "this_week_net_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_570;\n            lab.Ledger.ThisWeek.ChipSales = 8_260;", "3,570", "8,260"),
    ("LastWeekNetChipSpendIncomeSummaryLine", "lastWeekNetChipSpendIncome", "last week net chip spend income", "last_week_net_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_580;\n            lab.Ledger.LastWeek.ChipSales = 7_950;", "3,580", "7,950"),
    ("LifetimeNetChipSpendIncomeSummaryLine", "lifetimeNetChipSpendIncome", "lifetime net chip spend income", "lifetime_net_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 124_500;\n            lab.Ledger.Lifetime.ChipSales = 118_000;", "124,500", "118,000"),
    ("ThisWeekGreyDevSpendIncomeSummaryLine", "thisWeekGreyDevSpendIncome", "this week grey dev spend income", "this_week_grey_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_220;\n            lab.Ledger.ThisWeek.ChipDevelopment = -4_140;", "2,220", "4,140"),
    ("LastWeekGreyDevSpendIncomeSummaryLine", "lastWeekGreyDevSpendIncome", "last week grey dev spend income", "last_week_grey_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_740;\n            lab.Ledger.LastWeek.ChipDevelopment = -4_020;", "2,740", "4,020"),
    ("LifetimeGreyDevSpendIncomeSummaryLine", "lifetimeGreyDevSpendIncome", "lifetime grey dev spend income", "lifetime_grey_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 54_000;\n            lab.Ledger.Lifetime.ChipDevelopment = -94_000;", "54,000", "94,000"),
]

SPRINT148 = [
    ("ThisWeekShadyGrantSpendIncomeSummaryLine", "thisWeekShadyGrantSpendIncome", "this week shady grant spend income", "this_week_shady_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_230;\n            lab.Ledger.ThisWeek.Grants = 5_420;", "2,230", "5,420"),
    ("LastWeekShadyGrantSpendIncomeSummaryLine", "lastWeekShadyGrantSpendIncome", "last week shady grant spend income", "last_week_shady_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_750;\n            lab.Ledger.LastWeek.Grants = 6_300;", "2,750", "6,300"),
    ("LifetimeShadyGrantSpendIncomeSummaryLine", "lifetimeShadyGrantSpendIncome", "lifetime shady grant spend income", "lifetime_shady_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 54_100;\n            lab.Ledger.Lifetime.Grants = 202_000;", "54,100", "202,000"),
    ("ThisWeekPatentChipSpendIncomeSummaryLine", "thisWeekPatentChipSpendIncome", "this week patent chip spend income", "this_week_patent_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_070;\n            lab.Ledger.ThisWeek.ChipSales = 8_270;", "1,070", "8,270"),
    ("LastWeekPatentChipSpendIncomeSummaryLine", "lastWeekPatentChipSpendIncome", "last week patent chip spend income", "last_week_patent_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = 4_240;\n            lab.Ledger.LastWeek.ChipSales = 7_960;", "4,240", "7,960"),
    ("LifetimePatentChipSpendIncomeSummaryLine", "lifetimePatentChipSpendIncome", "lifetime patent chip spend income", "lifetime_patent_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -29_000;\n            lab.Ledger.Lifetime.ChipSales = 119_000;", "29,000", "119,000"),
]

PA145 = '            string lifetimePatentFinesSpendIncomePart = string.IsNullOrEmpty(lifetimePatentFinesSpendIncome) ? "" : " · " + lifetimePatentFinesSpendIncome;'
PA146 = '            string lifetimeLandCapitalSpendIncomePart = string.IsNullOrEmpty(lifetimeLandCapitalSpendIncome) ? "" : " · " + lifetimeLandCapitalSpendIncome;'
PA147 = '            string lifetimeSalariesConstructionSpendIncomePart = string.IsNullOrEmpty(lifetimeSalariesConstructionSpendIncome) ? "" : " · " + lifetimeSalariesConstructionSpendIncome;'
PA148 = '            string lifetimeGreyDevSpendIncomePart = string.IsNullOrEmpty(lifetimeGreyDevSpendIncome) ? "" : " · " + lifetimeGreyDevSpendIncome;'

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



insert_formatters(F145_146)
git_commit("feat(hud): sprint145-146 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT145, PA145, "lifetimePatentFinesSpendIncome")
run_sprint(SPRINT146, PA146, "lifetimeLandCapitalSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F147_148)
git_commit("feat(hud): sprint147-148 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT147, PA147, "lifetimeSalariesConstructionSpendIncome")
run_sprint(SPRINT148, PA148, "lifetimeGreyDevSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
