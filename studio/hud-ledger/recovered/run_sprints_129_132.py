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

F129_130 = [
    ("ThisWeekHardwareSiteSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekHardwareSiteSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeHardwareSiteSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
    ]),
    ("ThisWeekLandRoomSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LastWeekLandRoomSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LifetimeLandRoomSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekRentTrainingSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekRentTrainingSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "LastWeekLedgerGpuRentLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeRentTrainingSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("rent", "LastWeekLedgerGpuRentLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekPowerConstructionSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekPowerConstructionSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LifetimePowerConstructionSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
    ]),
]

F131_132 = [
    ("ThisWeekSalariesHiringSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("LastWeekSalariesHiringSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("LifetimeSalariesHiringSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekLaunderCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekLaunderCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeLaunderCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekPatentUpkeepSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LastWeekPatentUpkeepSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LifetimePatentUpkeepSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekNetGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekNetGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeNetGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
]

SPRINT129 = [
    ("ThisWeekHardwareSiteSpendIncomeSummaryLine", "thisWeekHardwareSiteSpendIncome", "this week hardware site spend income", "this_week_hardware_site_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_200;\n            lab.Ledger.ThisWeek.Revenue = 18_840;", "2,200", "18,840"),
    ("LastWeekHardwareSiteSpendIncomeSummaryLine", "lastWeekHardwareSiteSpendIncome", "last week hardware site spend income", "last_week_hardware_site_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_180;\n            lab.Ledger.LastWeek.Net = 3_430;", "2,180", "3,430"),
    ("LifetimeHardwareSiteSpendIncomeSummaryLine", "lifetimeHardwareSiteSpendIncome", "lifetime hardware site spend income", "lifetime_hardware_site_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -85_500;\n            lab.Ledger.Lifetime.Revenue = 450_000;", "85,500", "450,000"),
    ("ThisWeekLandRoomSpendIncomeSummaryLine", "thisWeekLandRoomSpendIncome", "this week land room spend income", "this_week_land_room_spend_income_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -1_100;\n            lab.Ledger.ThisWeek.Grants = 5_280;", "1,100", "5,280"),
    ("LastWeekLandRoomSpendIncomeSummaryLine", "lastWeekLandRoomSpendIncome", "last week land room spend income", "last_week_land_room_spend_income_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -1_080;\n            lab.Ledger.LastWeek.Revenue = 18_390;", "1,080", "18,390"),
    ("LifetimeLandRoomSpendIncomeSummaryLine", "lifetimeLandRoomSpendIncome", "lifetime land room spend income", "lifetime_land_room_spend_income_summary_line",
     "lab.Ledger.Lifetime.RoomUpkeep = -41_000;\n            lab.Ledger.Lifetime.Net = 119_500;", "41,000", "119,500"),
]

SPRINT130 = [
    ("ThisWeekRentTrainingSpendIncomeSummaryLine", "thisWeekRentTrainingSpendIncome", "this week rent training spend income", "this_week_rent_training_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_210;\n            lab.Ledger.ThisWeek.Training = -900;", "1,210", "900"),
    ("LastWeekRentTrainingSpendIncomeSummaryLine", "lastWeekRentTrainingSpendIncome", "last week rent training spend income", "last_week_rent_training_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_190;\n            lab.Ledger.LastWeek.Training = -880;", "1,190", "880"),
    ("LifetimeRentTrainingSpendIncomeSummaryLine", "lifetimeRentTrainingSpendIncome", "lifetime rent training spend income", "lifetime_rent_training_spend_income_summary_line",
     "lab.Ledger.Lifetime.Training = -52_000;\n            lab.Ledger.Lifetime.Power = -169_000;", "52,000", "169,000"),
    ("ThisWeekPowerConstructionSpendIncomeSummaryLine", "thisWeekPowerConstructionSpendIncome", "this week power construction spend income", "this_week_power_construction_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -3_010;\n            lab.Ledger.ThisWeek.Construction = -2_210;", "3,010", "2,210"),
    ("LastWeekPowerConstructionSpendIncomeSummaryLine", "lastWeekPowerConstructionSpendIncome", "last week power construction spend income", "last_week_power_construction_spend_income_summary_line",
     "lab.Ledger.LastWeek.Power = -2_460;\n            lab.Ledger.LastWeek.Construction = -2_190;", "2,460", "2,190"),
    ("LifetimePowerConstructionSpendIncomeSummaryLine", "lifetimePowerConstructionSpendIncome", "lifetime power construction spend income", "lifetime_power_construction_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -170_000;\n            lab.Ledger.Lifetime.Construction = -86_000;", "170,000", "86,000"),
]

SPRINT131 = [
    ("ThisWeekSalariesHiringSpendIncomeSummaryLine", "thisWeekSalariesHiringSpendIncome", "this week salaries hiring spend income", "this_week_salaries_hiring_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -12_400;\n            lab.Ledger.ThisWeek.Hiring = -600;", "12,400", "600"),
    ("LastWeekSalariesHiringSpendIncomeSummaryLine", "lastWeekSalariesHiringSpendIncome", "last week salaries hiring spend income", "last_week_salaries_hiring_spend_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -12_300;\n            lab.Ledger.LastWeek.Hiring = -590;", "12,300", "590"),
    ("LifetimeSalariesHiringSpendIncomeSummaryLine", "lifetimeSalariesHiringSpendIncome", "lifetime salaries hiring spend income", "lifetime_salaries_hiring_spend_income_summary_line",
     "lab.Ledger.Lifetime.Salaries = -890_000;\n            lab.Ledger.Lifetime.Hiring = -18_000;", "890,000", "18,000"),
    ("ThisWeekLaunderCapitalSpendIncomeSummaryLine", "thisWeekLaunderCapitalSpendIncome", "this week launder capital spend income", "this_week_launder_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 3_500;\n            lab.Ledger.ThisWeek.Grants = 5_290;", "3,500", "5,290"),
    ("LastWeekLaunderCapitalSpendIncomeSummaryLine", "lastWeekLaunderCapitalSpendIncome", "last week launder capital spend income", "last_week_launder_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_120;\n            lab.Ledger.LastWeek.Net = 3_440;", "3,120", "3,440"),
    ("LifetimeLaunderCapitalSpendIncomeSummaryLine", "lifetimeLaunderCapitalSpendIncome", "lifetime launder capital spend income", "lifetime_launder_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Laundered = 33_100;\n            lab.Ledger.Lifetime.Grants = 188_000;", "33,100", "188,000"),
]

SPRINT132 = [
    ("ThisWeekPatentUpkeepSpendIncomeSummaryLine", "thisWeekPatentUpkeepSpendIncome", "this week patent upkeep spend income", "this_week_patent_upkeep_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -960;\n            lab.Ledger.ThisWeek.Power = -3_020;", "960", "3,020"),
    ("LastWeekPatentUpkeepSpendIncomeSummaryLine", "lastWeekPatentUpkeepSpendIncome", "last week patent upkeep spend income", "last_week_patent_upkeep_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = 4_200;\n            lab.Ledger.LastWeek.Power = -2_470;", "4,200", "2,470"),
    ("LifetimePatentUpkeepSpendIncomeSummaryLine", "lifetimePatentUpkeepSpendIncome", "lifetime patent upkeep spend income", "lifetime_patent_upkeep_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -28_400;\n            lab.Ledger.Lifetime.Power = -171_000;", "28,400", "171,000"),
    ("ThisWeekNetGreySpendIncomeSummaryLine", "thisWeekNetGreySpendIncome", "this week net grey spend income", "this_week_net_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_450;\n            lab.Ledger.ThisWeek.GreyRevenue = 2_160;", "3,450", "2,160"),
    ("LastWeekNetGreySpendIncomeSummaryLine", "lastWeekNetGreySpendIncome", "last week net grey spend income", "last_week_net_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_460;\n            lab.Ledger.LastWeek.GreyRevenue = 2_680;", "3,460", "2,680"),
    ("LifetimeNetGreySpendIncomeSummaryLine", "lifetimeNetGreySpendIncome", "lifetime net grey spend income", "lifetime_net_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 120_000;\n            lab.Ledger.Lifetime.GreyRevenue = 53_400;", "120,000", "53,400"),
]

PA129 = '            string lifetimeChipDevSpendIncomePart = string.IsNullOrEmpty(lifetimeChipDevSpendIncome) ? "" : " · " + lifetimeChipDevSpendIncome;'
PA130 = '            string lifetimeLandRoomSpendIncomePart = string.IsNullOrEmpty(lifetimeLandRoomSpendIncome) ? "" : " · " + lifetimeLandRoomSpendIncome;'
PA131 = '            string lifetimePowerConstructionSpendIncomePart = string.IsNullOrEmpty(lifetimePowerConstructionSpendIncome) ? "" : " · " + lifetimePowerConstructionSpendIncome;'
PA132 = '            string lifetimeLaunderCapitalSpendIncomePart = string.IsNullOrEmpty(lifetimeLaunderCapitalSpendIncome) ? "" : " · " + lifetimeLaunderCapitalSpendIncome;'

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



insert_formatters(F129_130)
git_commit("feat(hud): sprint129-130 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT129, PA129, "lifetimeChipDevSpendIncome")
run_sprint(SPRINT130, PA130, "lifetimeLandRoomSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F131_132)
git_commit("feat(hud): sprint131-132 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT131, PA131, "lifetimePowerConstructionSpendIncome")
run_sprint(SPRINT132, PA132, "lifetimeLaunderCapitalSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
