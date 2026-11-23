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

F29_30 = [
    ("LastWeekPatentPowerSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekChipGrantSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeNetChipSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSiteGreySummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRevenueFinesSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantHardwareSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekRentGrantSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekFinesSiteSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("construction", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LifetimeGreyPowerSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekSalariesGrantSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekChipLandSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekUpkeepGpuSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
]

F31_32 = [
    ("LastWeekTrainingSiteSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekNetChipGrantSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeLaunderChipSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekRoomPatentSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekPowerGreySummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("construction", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekRevenueHardwareSummaryLine", [
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("ThisWeekGrantConstructionSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeFinesChipSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("LastWeekLaunderChipSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekCapitalLaunderSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGrantChipNetSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekPatentTrainingSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
]

SPRINT29 = [
    ("LastWeekPatentPowerSummaryLine", "lastWeekPatentPower", "last week patent power", "last_week_patent_power_summary_line",
     "lab.Ledger.LastWeek.Power = -3_500;\n            lab.Ledger.LastWeek.Fines = -720;", "3,500", "720"),
    ("ThisWeekChipGrantSummaryLine", "thisWeekChipGrant", "this week chip grant", "this_week_chip_grant_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 4_400;\n            lab.Ledger.ThisWeek.Grants = 3_800;", "4,400", "3,800"),
    ("LifetimeNetChipSummaryLine", "lifetimeNetChip", "lifetime net chip", "lifetime_net_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 185_000;\n            lab.Ledger.Lifetime.Grants = 94_000;", "185,000", "94,000"),
    ("LastWeekSiteGreySummaryLine", "lastWeekSiteGrey", "last week site grey", "last_week_site_grey_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -8_900;\n            lab.Ledger.LastWeek.GreyRevenue = 2_100;", "8,900", "2,100"),
    ("ThisWeekRevenueFinesSummaryLine", "thisWeekRevenueFines", "this week revenue fines", "this_week_revenue_fines_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 13_800;\n            lab.Ledger.ThisWeek.Fines = -460;", "13,800", "460"),
    ("LastWeekGrantHardwareSummaryLine", "lastWeekGrantHardware", "last week grant hardware", "last_week_grant_hardware_summary_line",
     "lab.Ledger.LastWeek.Grants = 4_900;\n            lab.Ledger.LastWeek.Hardware = -4_700;", "4,900", "4,700"),
]

SPRINT30 = [
    ("ThisWeekRentGrantSummaryLine", "thisWeekRentGrant", "this week rent grant", "this_week_rent_grant_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_100;\n            lab.Ledger.ThisWeek.Grants = 4_200;", "2,100", "4,200"),
    ("LastWeekFinesSiteSummaryLine", "lastWeekFinesSite", "last week fines site", "last_week_fines_site_summary_line",
     "lab.Ledger.LastWeek.Fines = -1_100;\n            lab.Ledger.LastWeek.SiteBuild = -7_800;", "1,100", "7,800"),
    ("LifetimeGreyPowerSummaryLine", "lifetimeGreyPower", "lifetime grey power", "lifetime_grey_power_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 41_000;\n            lab.Ledger.Lifetime.Power = -118_000;", "41,000", "118,000"),
    ("ThisWeekSalariesGrantSummaryLine", "thisWeekSalariesGrant", "this week salaries grant", "this_week_salaries_grant_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_900;\n            lab.Ledger.ThisWeek.Grants = 3_500;", "7,900", "3,500"),
    ("LastWeekChipLandSummaryLine", "lastWeekChipLand", "last week chip land", "last_week_chip_land_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_200;\n            lab.Ledger.LastWeek.Land = -15_000;", "7,200", "15,000"),
    ("LastWeekUpkeepGpuSummaryLine", "lastWeekUpkeepGpu", "last week upkeep gpu", "last_week_upkeep_gpu_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -1_400;\n            lab.Ledger.LastWeek.GpuRent = -2_300;", "1,400", "2,300"),
]

SPRINT31 = [
    ("LastWeekTrainingSiteSummaryLine", "lastWeekTrainingSite", "last week training site", "last_week_training_site_summary_line",
     "lab.Ledger.LastWeek.Training = -9_800;\n            lab.Ledger.LastWeek.SiteBuild = -6_400;", "9,800", "6,400"),
    ("ThisWeekNetChipGrantSummaryLine", "thisWeekNetChipGrant", "this week net chip grant", "this_week_net_chip_grant_summary_line",
     "lab.Ledger.ThisWeek.Net = 2_050;\n            lab.Ledger.ThisWeek.ChipSales = 4_700;", "2,050", "4,700"),
    ("LifetimeLaunderChipSummaryLine", "lifetimeLaunderChip", "lifetime launder chip", "lifetime_launder_chip_summary_line",
     "lab.Ledger.Lifetime.Laundered = 22_000;\n            lab.Ledger.Lifetime.ChipSales = 188_000;", "22,000", "188,000"),
    ("LastWeekRoomPatentSummaryLine", "lastWeekRoomPatent", "last week room patent", "last_week_room_patent_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_100;\n            lab.Ledger.LastWeek.Revenue = 19_500;", "2,100", "19,500"),
    ("ThisWeekPowerGreySummaryLine", "thisWeekPowerGrey", "this week power grey", "this_week_power_grey_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_800;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_650;", "2,800", "1,650"),
    ("LastWeekRevenueHardwareSummaryLine", "lastWeekRevenueHardware", "last week revenue hardware", "last_week_revenue_hardware_summary_line",
     "lab.Ledger.LastWeek.Revenue = 20_500;\n            lab.Ledger.LastWeek.Hardware = -4_900;", "20,500", "4,900"),
]

SPRINT32 = [
    ("ThisWeekGrantConstructionSummaryLine", "thisWeekGrantConstruction", "this week grant construction", "this_week_grant_construction_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_600;\n            lab.Ledger.ThisWeek.Construction = -3_700;", "4,600", "3,700"),
    ("LifetimeFinesChipSummaryLine", "lifetimeFinesChip", "lifetime fines chip", "lifetime_fines_chip_summary_line",
     "lab.Ledger.Lifetime.Fines = -19_500;\n            lab.Ledger.Lifetime.ChipSales = 192_000;", "19,500", "192,000"),
    ("LastWeekLaunderChipSummaryLine", "lastWeekLaunderChip", "last week launder chip", "last_week_launder_chip_summary_line",
     "lab.Ledger.LastWeek.Laundered = 2_900;\n            lab.Ledger.LastWeek.ChipSales = 6_800;", "2,900", "6,800"),
    ("LastWeekCapitalLaunderSummaryLine", "lastWeekCapitalLaunder", "last week capital launder", "last_week_capital_launder_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 42_000;\n            lab.Ledger.LastWeek.Laundered = 2_400;", "42,000", "2,400"),
    ("ThisWeekGrantChipNetSummaryLine", "thisWeekGrantChipNet", "this week grant chip net", "this_week_grant_chip_net_summary_line",
     "lab.Ledger.ThisWeek.Grants = 3_300;\n            lab.Ledger.ThisWeek.Net = 1_850;", "3,300", "1,850"),
    ("LastWeekPatentTrainingSummaryLine", "lastWeekPatentTraining", "last week patent training", "last_week_patent_training_summary_line",
     "lab.Ledger.LastWeek.Training = -8_700;\n            lab.Ledger.LastWeek.GpuRent = -2_150;", "8,700", "2,150"),
]

PA29 = '            string thisWeekGrantTraining2Part = string.IsNullOrEmpty(thisWeekGrantTraining2) ? "" : " · " + thisWeekGrantTraining2;'
PA30 = '            string lastWeekGrantHardwarePart = string.IsNullOrEmpty(lastWeekGrantHardware) ? "" : " · " + lastWeekGrantHardware;'
PA31 = '            string lastWeekUpkeepGpuPart = string.IsNullOrEmpty(lastWeekUpkeepGpu) ? "" : " · " + lastWeekUpkeepGpu;'
PA32 = '            string lastWeekRevenueHardwarePart = string.IsNullOrEmpty(lastWeekRevenueHardware) ? "" : " · " + lastWeekRevenueHardware;'


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


insert_formatters(F29_30)
git_commit("feat(hud): sprint29-30 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT29, PA29, "thisWeekGrantTraining2Part")
run_sprint(SPRINT30, PA30, "lastWeekGrantHardwarePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F31_32)
git_commit("feat(hud): sprint31-32 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT31, PA31, "lastWeekUpkeepGpuPart")
run_sprint(SPRINT32, PA32, "lastWeekRevenueHardwarePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)