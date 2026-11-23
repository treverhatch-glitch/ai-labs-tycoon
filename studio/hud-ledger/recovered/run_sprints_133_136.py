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

F133_134 = [
    ("ThisWeekGrantChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekGrantChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimeGrantChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("ThisWeekFinesDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekFinesDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeFinesDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekHardwareLandSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
    ]),
    ("LastWeekHardwareLandSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("LifetimeHardwareLandSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
    ]),
    ("ThisWeekSiteCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekSiteCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeSiteCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

F135_136 = [
    ("ThisWeekShadyLaunderSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekShadyLaunderSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeShadyLaunderSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekChipRevenueSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekChipRevenueSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeChipRevenueSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPatentGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekPatentGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimePatentGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGreyFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekGreyFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeGreyFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT133 = [
    ("ThisWeekGrantChipSpendIncomeSummaryLine", "thisWeekGrantChipSpendIncome", "this week grant chip spend income", "this_week_grant_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_300;\n            lab.Ledger.ThisWeek.ChipSales = 8_220;", "5,300", "8,220"),
    ("LastWeekGrantChipSpendIncomeSummaryLine", "lastWeekGrantChipSpendIncome", "last week grant chip spend income", "last_week_grant_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_210;\n            lab.Ledger.LastWeek.ChipSales = 7_910;", "6,210", "7,910"),
    ("LifetimeGrantChipSpendIncomeSummaryLine", "lifetimeGrantChipSpendIncome", "lifetime grant chip spend income", "lifetime_grant_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 189_000;\n            lab.Ledger.Lifetime.ChipSales = 114_000;", "189,000", "114,000"),
    ("ThisWeekFinesDevSpendIncomeSummaryLine", "thisWeekFinesDevSpendIncome", "this week fines dev spend income", "this_week_fines_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -970;\n            lab.Ledger.ThisWeek.ChipDevelopment = -4_120;", "970", "4,120"),
    ("LastWeekFinesDevSpendIncomeSummaryLine", "lastWeekFinesDevSpendIncome", "last week fines dev spend income", "last_week_fines_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -980;\n            lab.Ledger.LastWeek.ChipDevelopment = -4_000;", "980", "4,000"),
    ("LifetimeFinesDevSpendIncomeSummaryLine", "lifetimeFinesDevSpendIncome", "lifetime fines dev spend income", "lifetime_fines_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -28_500;\n            lab.Ledger.Lifetime.ChipDevelopment = -93_000;", "28,500", "93,000"),
]

SPRINT134 = [
    ("ThisWeekHardwareLandSpendIncomeSummaryLine", "thisWeekHardwareLandSpendIncome", "this week hardware land spend income", "this_week_hardware_land_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_220;\n            lab.Ledger.ThisWeek.Grants = 5_310;", "2,220", "5,310"),
    ("LastWeekHardwareLandSpendIncomeSummaryLine", "lastWeekHardwareLandSpendIncome", "last week hardware land spend income", "last_week_hardware_land_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_200;\n            lab.Ledger.LastWeek.Revenue = 18_400;", "2,200", "18,400"),
    ("LifetimeHardwareLandSpendIncomeSummaryLine", "lifetimeHardwareLandSpendIncome", "lifetime hardware land spend income", "lifetime_hardware_land_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -86_500;\n            lab.Ledger.Lifetime.Net = 121_000;", "86,500", "121,000"),
    ("ThisWeekSiteCapitalSpendIncomeSummaryLine", "thisWeekSiteCapitalSpendIncome", "this week site capital spend income", "this_week_site_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_470;\n            lab.Ledger.ThisWeek.Grants = 5_320;", "3,470", "5,320"),
    ("LastWeekSiteCapitalSpendIncomeSummaryLine", "lastWeekSiteCapitalSpendIncome", "last week site capital spend income", "last_week_site_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_480;\n            lab.Ledger.LastWeek.Grants = 6_220;", "3,480", "6,220"),
    ("LifetimeSiteCapitalSpendIncomeSummaryLine", "lifetimeSiteCapitalSpendIncome", "lifetime site capital spend income", "lifetime_site_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 190_000;\n            lab.Ledger.Lifetime.Revenue = 451_000;", "190,000", "451,000"),
]

SPRINT135 = [
    ("ThisWeekShadyLaunderSpendIncomeSummaryLine", "thisWeekShadyLaunderSpendIncome", "this week shady launder spend income", "this_week_shady_launder_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_170;\n            lab.Ledger.ThisWeek.Laundered = 3_510;", "2,170", "3,510"),
    ("LastWeekShadyLaunderSpendIncomeSummaryLine", "lastWeekShadyLaunderSpendIncome", "last week shady launder spend income", "last_week_shady_launder_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_690;\n            lab.Ledger.LastWeek.Laundered = 3_130;", "2,690", "3,130"),
    ("LifetimeShadyLaunderSpendIncomeSummaryLine", "lifetimeShadyLaunderSpendIncome", "lifetime shady launder spend income", "lifetime_shady_launder_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_500;\n            lab.Ledger.Lifetime.Laundered = 33_200;", "53,500", "33,200"),
    ("ThisWeekChipRevenueSpendIncomeSummaryLine", "thisWeekChipRevenueSpendIncome", "this week chip revenue spend income", "this_week_chip_revenue_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_230;\n            lab.Ledger.ThisWeek.Revenue = 18_850;", "8,230", "18,850"),
    ("LastWeekChipRevenueSpendIncomeSummaryLine", "lastWeekChipRevenueSpendIncome", "last week chip revenue spend income", "last_week_chip_revenue_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_920;\n            lab.Ledger.LastWeek.Revenue = 18_410;", "7,920", "18,410"),
    ("LifetimeChipRevenueSpendIncomeSummaryLine", "lifetimeChipRevenueSpendIncome", "lifetime chip revenue spend income", "lifetime_chip_revenue_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 115_000;\n            lab.Ledger.Lifetime.Revenue = 452_000;", "115,000", "452,000"),
]

SPRINT136 = [
    ("ThisWeekPatentGrantSpendIncomeSummaryLine", "thisWeekPatentGrantSpendIncome", "this week patent grant spend income", "this_week_patent_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_330;\n            lab.Ledger.ThisWeek.Fines = -990;", "5,330", "990"),
    ("LastWeekPatentGrantSpendIncomeSummaryLine", "lastWeekPatentGrantSpendIncome", "last week patent grant spend income", "last_week_patent_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_230;\n            lab.Ledger.LastWeek.PatentLicense = 4_210;", "6,230", "4,210"),
    ("LifetimePatentGrantSpendIncomeSummaryLine", "lifetimePatentGrantSpendIncome", "lifetime patent grant spend income", "lifetime_patent_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 191_000;\n            lab.Ledger.Lifetime.Fines = -28_600;", "191,000", "28,600"),
    ("ThisWeekGreyFinesSpendIncomeSummaryLine", "thisWeekGreyFinesSpendIncome", "this week grey fines spend income", "this_week_grey_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_180;\n            lab.Ledger.ThisWeek.Fines = -1_000;", "2,180", "1,000"),
    ("LastWeekGreyFinesSpendIncomeSummaryLine", "lastWeekGreyFinesSpendIncome", "last week grey fines spend income", "last_week_grey_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_700;\n            lab.Ledger.LastWeek.Fines = -1_010;", "2,700", "1,010"),
    ("LifetimeGreyFinesSpendIncomeSummaryLine", "lifetimeGreyFinesSpendIncome", "lifetime grey fines spend income", "lifetime_grey_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_600;\n            lab.Ledger.Lifetime.Fines = -28_700;", "53,600", "28,700"),
]

PA133 = '            string lifetimeNetGreySpendIncomePart = string.IsNullOrEmpty(lifetimeNetGreySpendIncome) ? "" : " · " + lifetimeNetGreySpendIncome;'
PA134 = '            string lifetimeFinesDevSpendIncomePart = string.IsNullOrEmpty(lifetimeFinesDevSpendIncome) ? "" : " · " + lifetimeFinesDevSpendIncome;'
PA135 = '            string lifetimeSiteCapitalSpendIncomePart = string.IsNullOrEmpty(lifetimeSiteCapitalSpendIncome) ? "" : " · " + lifetimeSiteCapitalSpendIncome;'
PA136 = '            string lifetimeChipRevenueSpendIncomePart = string.IsNullOrEmpty(lifetimeChipRevenueSpendIncome) ? "" : " · " + lifetimeChipRevenueSpendIncome;'

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



insert_formatters(F133_134)
git_commit("feat(hud): sprint133-134 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT133, PA133, "lifetimeNetGreySpendIncome")
run_sprint(SPRINT134, PA134, "lifetimeFinesDevSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F135_136)
git_commit("feat(hud): sprint135-136 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT135, PA135, "lifetimeSiteCapitalSpendIncome")
run_sprint(SPRINT136, PA136, "lifetimeChipRevenueSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
