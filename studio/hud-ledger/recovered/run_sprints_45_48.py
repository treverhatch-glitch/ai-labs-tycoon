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

F45_46 = [
    ("ThisWeekCapitalTrainingSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekSiteChipSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LifetimeGreyRoomSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantRevenueSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekLaunderGrantSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekPatentGrantSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekChipRoomSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGreyConstructionSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekNetLandSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekSalariesRentSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekGrantLandSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeCapitalGreySummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
]

F47_48 = [
    ("LastWeekRoomFinesSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekGpuHardwareSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("upkeep", "ThisWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LifetimeFinesRentSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LastWeekCapitalConstructionSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("ThisWeekGreyLandSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekLandNetSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekChipPatentSummaryLine", [
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHiringSalariesSummaryLine", [
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekHardwareChipSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekRentChipSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeTrainingGrantSummaryLine", [
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekNetGrantSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
]

SPRINT45 = [
    ("ThisWeekCapitalTrainingSummaryLine", "thisWeekCapitalTraining", "this week capital training", "this_week_capital_training_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 42_000;\n            lab.Ledger.ThisWeek.Training = -9_300;", "42,000", "9,300"),
    ("LastWeekSiteChipSummaryLine", "lastWeekSiteChip", "last week site chip", "last_week_site_chip_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_500;\n            lab.Ledger.LastWeek.ChipSales = 6_700;", "7,500", "6,700"),
    ("LifetimeGreyRoomSummaryLine", "lifetimeGreyRoom", "lifetime grey room", "lifetime_grey_room_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 44_000;\n            lab.Ledger.Lifetime.RoomUpkeep = -52_000;", "44,000", "52,000"),
    ("LastWeekGrantRevenueSummaryLine", "lastWeekGrantRevenue", "last week grant revenue", "last_week_grant_revenue_summary_line",
     "lab.Ledger.LastWeek.Grants = 4_900;\n            lab.Ledger.LastWeek.Revenue = 17_600;", "4,900", "17,600"),
    ("ThisWeekLaunderGrantSummaryLine", "thisWeekLaunderGrant", "this week launder grant", "this_week_launder_grant_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_250;\n            lab.Ledger.ThisWeek.Grants = 3_700;", "2,250", "3,700"),
    ("LastWeekPatentGrantSummaryLine", "lastWeekPatentGrant", "last week patent grant", "last_week_patent_grant_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_100;\n            lab.Ledger.LastWeek.Fines = -480;", "5,100", "480"),
]

SPRINT46 = [
    ("ThisWeekChipRoomSummaryLine", "thisWeekChipRoom", "this week chip room", "this_week_chip_room_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_200;\n            lab.Ledger.ThisWeek.RoomUpkeep = -2_100;", "5,200", "2,100"),
    ("LastWeekGreyConstructionSummaryLine", "lastWeekGreyConstruction", "last week grey construction", "last_week_grey_construction_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_200;\n            lab.Ledger.LastWeek.Construction = -6_300;", "2,200", "6,300"),
    ("ThisWeekNetLandSummaryLine", "thisWeekNetLand", "this week net land", "this_week_net_land_summary_line",
     "lab.Ledger.ThisWeek.Net = 1_900;\n            lab.Ledger.ThisWeek.Land = -10_500;", "1,900", "10,500"),
    ("LastWeekSalariesRentSummaryLine", "lastWeekSalariesRent", "last week salaries rent", "last_week_salaries_rent_summary_line",
     "lab.Ledger.LastWeek.Salaries = -7_500;\n            lab.Ledger.LastWeek.GpuRent = -2_120;", "7,500", "2,120"),
    ("ThisWeekGrantLandSummaryLine", "thisWeekGrantLand", "this week grant land", "this_week_grant_land_summary_line",
     "lab.Ledger.ThisWeek.Grants = 3_850;\n            lab.Ledger.ThisWeek.Land = -9_600;", "3,850", "9,600"),
    ("LifetimeCapitalGreySummaryLine", "lifetimeCapitalGrey", "lifetime capital grey", "lifetime_capital_grey_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 45_000;\n            lab.Ledger.Lifetime.Laundered = 26_000;", "45,000", "26,000"),
]

SPRINT47 = [
    ("LastWeekRoomFinesSummaryLine", "lastWeekRoomFines", "last week room fines", "last_week_room_fines_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_200;\n            lab.Ledger.LastWeek.Fines = -510;", "2,200", "510"),
    ("ThisWeekGpuHardwareSummaryLine", "thisWeekGpuHardware", "this week gpu hardware", "this_week_gpu_hardware_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_050;\n            lab.Ledger.ThisWeek.Hardware = -4_100;", "2,050", "4,100"),
    ("LifetimeFinesRentSummaryLine", "lifetimeFinesRent", "lifetime fines rent", "lifetime_fines_rent_summary_line",
     "lab.Ledger.Lifetime.Fines = -21_000;\n            lab.Ledger.Lifetime.Power = -128_000;", "21,000", "128,000"),
    ("LastWeekCapitalConstructionSummaryLine", "lastWeekCapitalConstruction", "last week capital construction", "last_week_capital_construction_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 38_000;\n            lab.Ledger.LastWeek.Construction = -5_800;", "38,000", "5,800"),
    ("ThisWeekGreyLandSummaryLine", "thisWeekGreyLand", "this week grey land", "this_week_grey_land_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_780;\n            lab.Ledger.ThisWeek.Land = -10_800;", "1,780", "10,800"),
    ("LastWeekLandNetSummaryLine", "lastWeekLandNet", "last week land net", "last_week_land_net_summary_line",
     "lab.Ledger.LastWeek.Land = -11_200;\n            lab.Ledger.LastWeek.Net = 2_700;", "11,200", "2,700"),
]

SPRINT48 = [
    ("ThisWeekChipPatentSummaryLine", "thisWeekChipPatent", "this week chip patent", "this_week_chip_patent_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -5_100;\n            lab.Ledger.ThisWeek.ChipSales = 4_800;", "5,100", "4,800"),
    ("LifetimeHiringSalariesSummaryLine", "lifetimeHiringSalaries", "lifetime hiring salaries", "lifetime_hiring_salaries_summary_line",
     "lab.Ledger.Lifetime.Hiring = -90_000;\n            lab.Ledger.Lifetime.Salaries = -400_000;", "90,000", "400,000"),
    ("LastWeekHardwareChipSummaryLine", "lastWeekHardwareChip", "last week hardware chip", "last_week_hardware_chip_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_600;\n            lab.Ledger.LastWeek.ChipSales = 7_200;", "4,600", "7,200"),
    ("ThisWeekRentChipSummaryLine", "thisWeekRentChip", "this week rent chip", "this_week_rent_chip_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_950;\n            lab.Ledger.ThisWeek.ChipSales = 5_100;", "1,950", "5,100"),
    ("LifetimeTrainingGrantSummaryLine", "lifetimeTrainingGrant", "lifetime training grant", "lifetime_training_grant_summary_line",
     "lab.Ledger.Lifetime.Training = -210_000;\n            lab.Ledger.Lifetime.Grants = 110_000;", "210,000", "110,000"),
    ("LastWeekNetGrantSummaryLine", "lastWeekNetGrant", "last week net grant", "last_week_net_grant_summary_line",
     "lab.Ledger.LastWeek.Net = 2_500;\n            lab.Ledger.LastWeek.Grants = 4_600;", "2,500", "4,600"),
]

PA45 = '            string lifetimeNetPowerPart = string.IsNullOrEmpty(lifetimeNetPower) ? "" : " · " + lifetimeNetPower;'
PA46 = '            string lastWeekPatentGrantPart = string.IsNullOrEmpty(lastWeekPatentGrant) ? "" : " · " + lastWeekPatentGrant;'
PA47 = '            string lifetimeCapitalGreyPart = string.IsNullOrEmpty(lifetimeCapitalGrey) ? "" : " · " + lifetimeCapitalGrey;'
PA48 = '            string lastWeekLandNetPart = string.IsNullOrEmpty(lastWeekLandNet) ? "" : " · " + lastWeekLandNet;'


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


insert_formatters(F45_46)
git_commit("feat(hud): sprint45-46 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT45, PA45, "lifetimeNetPowerPart")
run_sprint(SPRINT46, PA46, "lastWeekPatentGrantPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F47_48)
git_commit("feat(hud): sprint47-48 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT47, PA47, "lifetimeCapitalGreyPart")
run_sprint(SPRINT48, PA48, "lastWeekLandNetPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)