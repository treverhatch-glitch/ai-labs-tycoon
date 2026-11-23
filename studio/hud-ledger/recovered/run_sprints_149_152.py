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

F149_150 = [
    ("ThisWeekFinesGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekFinesGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeFinesGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGrantRevenueSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekGrantRevenueSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGrantRevenueSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekNetFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekNetFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeNetFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGreyChipSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekGreyChipSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimeGreyChipSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
]

F151_152 = [
    ("ThisWeekGrantPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekGrantPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeGrantPowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekDevFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekDevFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeDevFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekHardwareCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekHardwareCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeHardwareCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekSiteGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSiteGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSiteGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT149 = [
    ("ThisWeekFinesGrantSpendIncomeSummaryLine", "thisWeekFinesGrantSpendIncome", "this week fines grant spend income", "this_week_fines_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_080;\n            lab.Ledger.ThisWeek.Grants = 5_430;", "1,080", "5,430"),
    ("LastWeekFinesGrantSpendIncomeSummaryLine", "lastWeekFinesGrantSpendIncome", "last week fines grant spend income", "last_week_fines_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -1_090;\n            lab.Ledger.LastWeek.Grants = 6_310;", "1,090", "6,310"),
    ("LifetimeFinesGrantSpendIncomeSummaryLine", "lifetimeFinesGrantSpendIncome", "lifetime fines grant spend income", "lifetime_fines_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -29_100;\n            lab.Ledger.Lifetime.Grants = 203_000;", "29,100", "203,000"),
    ("ThisWeekGrantRevenueSpendIncomeSummaryLine", "thisWeekGrantRevenueSpendIncome", "this week grant revenue spend income", "this_week_grant_revenue_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_440;\n            lab.Ledger.ThisWeek.Revenue = 18_880;", "5,440", "18,880"),
    ("LastWeekGrantRevenueSpendIncomeSummaryLine", "lastWeekGrantRevenueSpendIncome", "last week grant revenue spend income", "last_week_grant_revenue_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_320;\n            lab.Ledger.LastWeek.Revenue = 18_470;", "6,320", "18,470"),
    ("LifetimeGrantRevenueSpendIncomeSummaryLine", "lifetimeGrantRevenueSpendIncome", "lifetime grant revenue spend income", "lifetime_grant_revenue_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 204_000;\n            lab.Ledger.Lifetime.Revenue = 455_000;", "204,000", "455,000"),
]

SPRINT150 = [
    ("ThisWeekNetFinesSpendIncomeSummaryLine", "thisWeekNetFinesSpendIncome", "this week net fines spend income", "this_week_net_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_590;\n            lab.Ledger.ThisWeek.Fines = -1_100;", "3,590", "1,100"),
    ("LastWeekNetFinesSpendIncomeSummaryLine", "lastWeekNetFinesSpendIncome", "last week net fines spend income", "last_week_net_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_600;\n            lab.Ledger.LastWeek.Fines = -1_110;", "3,600", "1,110"),
    ("LifetimeNetFinesSpendIncomeSummaryLine", "lifetimeNetFinesSpendIncome", "lifetime net fines spend income", "lifetime_net_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 125_000;\n            lab.Ledger.Lifetime.Fines = -29_200;", "125,000", "29,200"),
    ("ThisWeekGreyChipSpendIncomeSummaryLine", "thisWeekGreyChipSpendIncome", "this week grey chip spend income", "this_week_grey_chip_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_240;\n            lab.Ledger.ThisWeek.ChipSales = 8_280;", "2,240", "8,280"),
    ("LastWeekGreyChipSpendIncomeSummaryLine", "lastWeekGreyChipSpendIncome", "last week grey chip spend income", "last_week_grey_chip_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_760;\n            lab.Ledger.LastWeek.ChipSales = 7_970;", "2,760", "7,970"),
    ("LifetimeGreyChipSpendIncomeSummaryLine", "lifetimeGreyChipSpendIncome", "lifetime grey chip spend income", "lifetime_grey_chip_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 54_200;\n            lab.Ledger.Lifetime.ChipSales = 120_000;", "54,200", "120,000"),
]

SPRINT151 = [
    ("ThisWeekGrantPowerSpendIncomeSummaryLine", "thisWeekGrantPowerSpendIncome", "this week grant power spend income", "this_week_grant_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_450;\n            lab.Ledger.ThisWeek.Power = -3_050;", "5,450", "3,050"),
    ("LastWeekGrantPowerSpendIncomeSummaryLine", "lastWeekGrantPowerSpendIncome", "last week grant power spend income", "last_week_grant_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_330;\n            lab.Ledger.LastWeek.Power = -2_500;", "6,330", "2,500"),
    ("LifetimeGrantPowerSpendIncomeSummaryLine", "lifetimeGrantPowerSpendIncome", "lifetime grant power spend income", "lifetime_grant_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 205_000;\n            lab.Ledger.Lifetime.Power = -175_000;", "205,000", "175,000"),
    ("ThisWeekDevFinesSpendIncomeSummaryLine", "thisWeekDevFinesSpendIncome", "this week dev fines spend income", "this_week_dev_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -4_150;\n            lab.Ledger.ThisWeek.Fines = -1_120;", "4,150", "1,120"),
    ("LastWeekDevFinesSpendIncomeSummaryLine", "lastWeekDevFinesSpendIncome", "last week dev fines spend income", "last_week_dev_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipDevelopment = -4_030;\n            lab.Ledger.LastWeek.Fines = -1_130;", "4,030", "1,130"),
    ("LifetimeDevFinesSpendIncomeSummaryLine", "lifetimeDevFinesSpendIncome", "lifetime dev fines spend income", "lifetime_dev_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -94_500;\n            lab.Ledger.Lifetime.Fines = -29_300;", "94,500", "29,300"),
]

SPRINT152 = [
    ("ThisWeekHardwareCapitalSpendIncomeSummaryLine", "thisWeekHardwareCapitalSpendIncome", "this week hardware capital spend income", "this_week_hardware_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_270;\n            lab.Ledger.ThisWeek.Grants = 5_460;", "2,270", "5,460"),
    ("LastWeekHardwareCapitalSpendIncomeSummaryLine", "lastWeekHardwareCapitalSpendIncome", "last week hardware capital spend income", "last_week_hardware_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_250;\n            lab.Ledger.LastWeek.Net = 3_610;", "2,250", "3,610"),
    ("LifetimeHardwareCapitalSpendIncomeSummaryLine", "lifetimeHardwareCapitalSpendIncome", "lifetime hardware capital spend income", "lifetime_hardware_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -89_000;\n            lab.Ledger.Lifetime.Grants = 206_000;", "89,000", "206,000"),
    ("ThisWeekSiteGrantSpendIncomeSummaryLine", "thisWeekSiteGrantSpendIncome", "this week site grant spend income", "this_week_site_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_620;\n            lab.Ledger.ThisWeek.Grants = 5_470;", "3,620", "5,470"),
    ("LastWeekSiteGrantSpendIncomeSummaryLine", "lastWeekSiteGrantSpendIncome", "last week site grant spend income", "last_week_site_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_630;\n            lab.Ledger.LastWeek.Grants = 6_340;", "3,630", "6,340"),
    ("LifetimeSiteGrantSpendIncomeSummaryLine", "lifetimeSiteGrantSpendIncome", "lifetime site grant spend income", "lifetime_site_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -89_500;\n            lab.Ledger.Lifetime.Grants = 207_000;", "89,500", "207,000"),
]

PA149 = '            string lifetimePatentChipSpendIncomePart = string.IsNullOrEmpty(lifetimePatentChipSpendIncome) ? "" : " · " + lifetimePatentChipSpendIncome;'
PA150 = '            string lifetimeGrantRevenueSpendIncomePart = string.IsNullOrEmpty(lifetimeGrantRevenueSpendIncome) ? "" : " · " + lifetimeGrantRevenueSpendIncome;'
PA151 = '            string lifetimeGreyChipSpendIncomePart = string.IsNullOrEmpty(lifetimeGreyChipSpendIncome) ? "" : " · " + lifetimeGreyChipSpendIncome;'
PA152 = '            string lifetimeDevFinesSpendIncomePart = string.IsNullOrEmpty(lifetimeDevFinesSpendIncome) ? "" : " · " + lifetimeDevFinesSpendIncome;'

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



insert_formatters(F149_150)
git_commit("feat(hud): sprint149-150 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT149, PA149, "lifetimePatentChipSpendIncome")
run_sprint(SPRINT150, PA150, "lifetimeGrantRevenueSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F151_152)
git_commit("feat(hud): sprint151-152 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT151, PA151, "lifetimeGreyChipSpendIncome")
run_sprint(SPRINT152, PA152, "lifetimeDevFinesSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
