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

F21_22 = [
    ("LastWeekCapitalHardwareSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekFinesNetSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGrantsChipSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekLaunderPowerSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("ThisWeekSalariesConstructionSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekChipTrainingSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekGrantsPowerSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekRoomLandSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeFinesTrainingSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekNetRevenueSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekHiringSiteSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("ThisWeekChipRentSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
]

F23_24 = [
    ("LastWeekPatentGreySummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekRentNetSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeRevenueLaunderSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekConstructionPowerSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekSalariesGreySummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekGrantChipSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekHardwareSiteSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("ThisWeekFinesConstructionSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeGreyTrainingSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekGrantLaunderSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekGrantPowerSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekTrainingGreySummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
]

SPRINT21 = [
    ("LastWeekCapitalHardwareSummaryLine", "lastWeekCapitalHardware", "last week capital hardware", "last_week_capital_hardware_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 50_000;\n            lab.Ledger.LastWeek.Hardware = -5_500;", "50,000", "5,500"),
    ("ThisWeekFinesNetSummaryLine", "thisWeekFinesNet", "this week fines net", "this_week_fines_net_summary_line",
     "lab.Ledger.ThisWeek.Fines = -650;\n            lab.Ledger.ThisWeek.Net = 2_100;", "650", "2,100"),
    ("LifetimeGrantsChipSummaryLine", "lifetimeGrantsChip", "lifetime grants chip", "lifetime_grants_chip_summary_line",
     "lab.Ledger.Lifetime.Grants = 88_000;\n            lab.Ledger.Lifetime.ChipSales = 175_000;", "88,000", "175,000"),
    ("LastWeekLaunderPowerSummaryLine", "lastWeekLaunderPower", "last week launder power", "last_week_launder_power_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_200;\n            lab.Ledger.LastWeek.Power = -4_100;", "3,200", "4,100"),
    ("ThisWeekSalariesConstructionSummaryLine", "thisWeekSalariesConstruction", "this week salaries construction", "this_week_salaries_construction_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -8_200;\n            lab.Ledger.ThisWeek.Construction = -3_900;", "8,200", "3,900"),
    ("LastWeekChipTrainingSummaryLine", "lastWeekChipTraining", "last week chip training", "last_week_chip_training_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_800;\n            lab.Ledger.LastWeek.Training = -11_000;", "7,800", "11,000"),
]

SPRINT22 = [
    ("ThisWeekGrantsPowerSummaryLine", "thisWeekGrantsPower", "this week grants power", "this_week_grants_power_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_500;\n            lab.Ledger.ThisWeek.Power = -2_900;", "5,500", "2,900"),
    ("LastWeekRoomLandSummaryLine", "lastWeekRoomLand", "last week room land", "last_week_room_land_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -1_900;\n            lab.Ledger.LastWeek.Land = -14_000;", "1,900", "14,000"),
    ("LifetimeFinesTrainingSummaryLine", "lifetimeFinesTraining", "lifetime fines training", "lifetime_fines_training_summary_line",
     "lab.Ledger.Lifetime.Fines = -18_000;\n            lab.Ledger.Lifetime.Training = -220_000;", "18,000", "220,000"),
    ("ThisWeekNetRevenueSummaryLine", "thisWeekNetRevenue", "this week net revenue", "this_week_net_revenue_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 15_500;\n            lab.Ledger.ThisWeek.Net = 1_800;", "15,500", "1,800"),
    ("LastWeekHiringSiteSummaryLine", "lastWeekHiringSite", "last week hiring site", "last_week_hiring_site_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_400;\n            lab.Ledger.LastWeek.SiteBuild = -9_800;", "3,400", "9,800"),
    ("ThisWeekChipRentSummaryLine", "thisWeekChipRent", "this week chip rent", "this_week_chip_rent_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 4_600;\n            lab.Ledger.ThisWeek.GpuRent = -1_750;", "4,600", "1,750"),
]

SPRINT23 = [
    ("LastWeekPatentGreySummaryLine", "lastWeekPatentGrey", "last week patent grey", "last_week_patent_grey_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_400;\n            lab.Ledger.LastWeek.CapitalIn = 32_000;", "2,400", "32,000"),
    ("ThisWeekRentNetSummaryLine", "thisWeekRentNet", "this week rent net", "this_week_rent_net_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_050;\n            lab.Ledger.ThisWeek.Net = 2_400;", "2,050", "2,400"),
    ("LifetimeRevenueLaunderSummaryLine", "lifetimeRevenueLaunder", "lifetime revenue launder", "lifetime_revenue_launder_summary_line",
     "lab.Ledger.Lifetime.Revenue = 610_000;\n            lab.Ledger.Lifetime.Laundered = 19_000;", "610,000", "19,000"),
    ("LastWeekConstructionPowerSummaryLine", "lastWeekConstructionPower", "last week construction power", "last_week_construction_power_summary_line",
     "lab.Ledger.LastWeek.Construction = -6_700;\n            lab.Ledger.LastWeek.Power = -3_200;", "6,700", "3,200"),
    ("LastWeekSalariesGreySummaryLine", "lastWeekSalariesGrey", "last week salaries grey", "last_week_salaries_grey_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_900;\n            lab.Ledger.LastWeek.GreyRevenue = 1_600;", "8,900", "1,600"),
    ("ThisWeekGrantChipSummaryLine", "thisWeekGrantChip", "this week grant chip", "this_week_grant_chip_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_800;\n            lab.Ledger.ThisWeek.ChipSales = 5_100;", "4,800", "5,100"),
]

SPRINT24 = [
    ("LastWeekHardwareSiteSummaryLine", "lastWeekHardwareSite", "last week hardware site", "last_week_hardware_site_summary_line",
     "lab.Ledger.LastWeek.Hardware = -5_100;\n            lab.Ledger.LastWeek.SiteBuild = -10_500;", "5,100", "10,500"),
    ("ThisWeekFinesConstructionSummaryLine", "thisWeekFinesConstruction", "this week fines construction", "this_week_fines_construction_summary_line",
     "lab.Ledger.ThisWeek.Fines = -420;\n            lab.Ledger.ThisWeek.Construction = -4_100;", "420", "4,100"),
    ("LifetimeGreyTrainingSummaryLine", "lifetimeGreyTraining", "lifetime grey training", "lifetime_grey_training_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 42_000;\n            lab.Ledger.Lifetime.Training = -200_000;", "42,000", "200,000"),
    ("LastWeekGrantLaunderSummaryLine", "lastWeekGrantLaunder", "last week grant launder", "last_week_grant_launder_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_200;\n            lab.Ledger.LastWeek.Laundered = 2_800;", "6,200", "2,800"),
    ("ThisWeekGrantPowerSummaryLine", "thisWeekGrantPower", "this week grant power", "this_week_grant_power_summary_line",
     "lab.Ledger.ThisWeek.Grants = 3_900;\n            lab.Ledger.ThisWeek.Power = -2_600;", "3,900", "2,600"),
    ("LastWeekTrainingGreySummaryLine", "lastWeekTrainingGrey", "last week training grey", "last_week_training_grey_summary_line",
     "lab.Ledger.LastWeek.Training = -9_400;\n            lab.Ledger.LastWeek.GreyRevenue = 1_900;", "9,400", "1,900"),
]

PA21 = '            string thisWeekTrainingGrantPart = string.IsNullOrEmpty(thisWeekTrainingGrant) ? "" : " · " + thisWeekTrainingGrant;'
PA22 = '            string lastWeekChipTrainingPart = string.IsNullOrEmpty(lastWeekChipTraining) ? "" : " · " + lastWeekChipTraining;'
PA23 = '            string thisWeekChipRentPart = string.IsNullOrEmpty(thisWeekChipRent) ? "" : " · " + thisWeekChipRent;'
PA24 = '            string thisWeekGrantChipPart = string.IsNullOrEmpty(thisWeekGrantChip) ? "" : " · " + thisWeekGrantChip;'


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


insert_formatters(F21_22)
git_commit("feat(hud): sprint21-22 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT21, PA21, "thisWeekTrainingGrantPart")
run_sprint(SPRINT22, PA22, "lastWeekChipTrainingPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F23_24)
git_commit("feat(hud): sprint23-24 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT23, PA23, "thisWeekChipRentPart")
run_sprint(SPRINT24, PA24, "thisWeekGrantChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)