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

F49_50 = [
    ("ThisWeekSiteGrantSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekLaunderRoomSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekHiringChipSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeLandGrantSummaryLine", [
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPowerSalariesSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekCapitalChipSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeRentGreySummaryLine", [
        ("gpuRent", "LifetimeLedgerGpuRentLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPatentChipSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekGrantRoomSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekLaunderChipSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeSiteTrainingSummaryLine", [
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekGpuGrantSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
]

F51_52 = [
    ("LastWeekConstructionNetSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHardwareGrantSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeFinesLandSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekChipRoomSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekPatentLandSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRentLandSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekRoomTrainingSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeChipGrantSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekPowerGrantSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekRentLandSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekLaunderTrainingSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeNetRoomSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT49 = [
    ("ThisWeekSiteGrantSummaryLine", "thisWeekSiteGrant", "this week site grant", "this_week_site_grant_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -6_900;\n            lab.Ledger.ThisWeek.Grants = 3_950;", "6,900", "3,950"),
    ("LastWeekLaunderRoomSummaryLine", "lastWeekLaunderRoom", "last week launder room", "last_week_launder_room_summary_line",
     "lab.Ledger.LastWeek.Laundered = 2_900;\n            lab.Ledger.LastWeek.RoomUpkeep = -2_150;", "2,900", "2,150"),
    ("LastWeekHiringChipSummaryLine", "lastWeekHiringChip", "last week hiring chip", "last_week_hiring_chip_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_300;\n            lab.Ledger.LastWeek.ChipSales = 6_600;", "3,300", "6,600"),
    ("LifetimeLandGrantSummaryLine", "lifetimeLandGrant", "lifetime land grant", "lifetime_land_grant_summary_line",
     "lab.Ledger.Lifetime.Land = -80_000;\n            lab.Ledger.Lifetime.Grants = 112_000;", "80,000", "112,000"),
    ("ThisWeekPowerSalariesSummaryLine", "thisWeekPowerSalaries", "this week power salaries", "this_week_power_salaries_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_700;\n            lab.Ledger.ThisWeek.Salaries = -7_650;", "2,700", "7,650"),
    ("ThisWeekCapitalChipSummaryLine", "thisWeekCapitalChip", "this week capital chip", "this_week_capital_chip_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 44_000;\n            lab.Ledger.ThisWeek.ChipSales = 4_900;", "44,000", "4,900"),
]

SPRINT50 = [
    ("LifetimeRentGreySummaryLine", "lifetimeRentGrey", "lifetime rent grey", "lifetime_rent_grey_summary_line",
     "lab.Ledger.Lifetime.GpuRent = -95_000;\n            lab.Ledger.Lifetime.GreyRevenue = 46_000;", "95,000", "46,000"),
    ("LastWeekPatentChipSummaryLine", "lastWeekPatentChip", "last week patent chip", "last_week_patent_chip_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_000;\n            lab.Ledger.LastWeek.ChipDevelopment = -5_200;", "7,000", "5,200"),
    ("ThisWeekGrantRoomSummaryLine", "thisWeekGrantRoom", "this week grant room", "this_week_grant_room_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_000;\n            lab.Ledger.ThisWeek.RoomUpkeep = -2_080;", "4,000", "2,080"),
    ("ThisWeekLaunderChipSummaryLine", "thisWeekLaunderChip", "this week launder chip", "this_week_launder_chip_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_300;\n            lab.Ledger.ThisWeek.ChipSales = 5_050;", "2,300", "5,050"),
    ("LifetimeSiteTrainingSummaryLine", "lifetimeSiteTraining", "lifetime site training", "lifetime_site_training_summary_line",
     "lab.Ledger.Lifetime.SiteBuild = -280_000;\n            lab.Ledger.Lifetime.Training = -215_000;", "280,000", "215,000"),
    ("LastWeekGpuGrantSummaryLine", "lastWeekGpuGrant", "last week gpu grant", "last_week_gpu_grant_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_240;\n            lab.Ledger.LastWeek.Grants = 4_400;", "2,240", "4,400"),
]

SPRINT51 = [
    ("LastWeekConstructionNetSummaryLine", "lastWeekConstructionNet", "last week construction net", "last_week_construction_net_summary_line",
     "lab.Ledger.LastWeek.Construction = -6_100;\n            lab.Ledger.LastWeek.Net = 2_550;", "6,100", "2,550"),
    ("ThisWeekHardwareGrantSummaryLine", "thisWeekHardwareGrant", "this week hardware grant", "this_week_hardware_grant_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_300;\n            lab.Ledger.ThisWeek.Grants = 3_750;", "4,300", "3,750"),
    ("LifetimeFinesLandSummaryLine", "lifetimeFinesLand", "lifetime fines land", "lifetime_fines_land_summary_line",
     "lab.Ledger.Lifetime.Fines = -22_000;\n            lab.Ledger.Lifetime.Land = -82_000;", "22,000", "82,000"),
    ("LastWeekChipRoomSummaryLine", "lastWeekChipRoom", "last week chip room", "last_week_chip_room_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 6_900;\n            lab.Ledger.LastWeek.RoomUpkeep = -2_120;", "6,900", "2,120"),
    ("ThisWeekPatentLandSummaryLine", "thisWeekPatentLand", "this week patent land", "this_week_patent_land_summary_line",
     "lab.Ledger.ThisWeek.Land = -10_400;\n            lab.Ledger.ThisWeek.Grants = 3_900;", "10,400", "3,900"),
    ("LastWeekRentLandSummaryLine", "lastWeekRentLand", "last week rent land", "last_week_rent_land_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_160;\n            lab.Ledger.LastWeek.Land = -11_500;", "2,160", "11,500"),
]

SPRINT52 = [
    ("ThisWeekRoomTrainingSummaryLine", "thisWeekRoomTraining", "this week room training", "this_week_room_training_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -2_180;\n            lab.Ledger.ThisWeek.Training = -9_200;", "2,180", "9,200"),
    ("LifetimeChipGrantSummaryLine", "lifetimeChipGrant", "lifetime chip grant", "lifetime_chip_grant_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 225_000;\n            lab.Ledger.Lifetime.Grants = 115_000;", "225,000", "115,000"),
    ("LastWeekPowerGrantSummaryLine", "lastWeekPowerGrant", "last week power grant", "last_week_power_grant_summary_line",
     "lab.Ledger.LastWeek.Power = -3_450;\n            lab.Ledger.LastWeek.Grants = 4_700;", "3,450", "4,700"),
    ("ThisWeekRentLandSummaryLine", "thisWeekRentLand", "this week rent land", "this_week_rent_land_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_020;\n            lab.Ledger.ThisWeek.Land = -10_600;", "2,020", "10,600"),
    ("LastWeekLaunderTrainingSummaryLine", "lastWeekLaunderTraining", "last week launder training", "last_week_launder_training_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_000;\n            lab.Ledger.LastWeek.Training = -8_700;", "3,000", "8,700"),
    ("LifetimeNetRoomSummaryLine", "lifetimeNetRoom", "lifetime net room", "lifetime_net_room_summary_line",
     "lab.Ledger.Lifetime.Net = 55_000;\n            lab.Ledger.Lifetime.RoomUpkeep = -55_000;", "55,000"),
]

PA49 = '            string lastWeekNetGrantPart = string.IsNullOrEmpty(lastWeekNetGrant) ? "" : " · " + lastWeekNetGrant;'
PA50 = '            string thisWeekCapitalChipPart = string.IsNullOrEmpty(thisWeekCapitalChip) ? "" : " · " + thisWeekCapitalChip;'
PA51 = '            string lastWeekGpuGrantPart = string.IsNullOrEmpty(lastWeekGpuGrant) ? "" : " · " + lastWeekGpuGrant;'
PA52 = '            string lastWeekRentLandPart = string.IsNullOrEmpty(lastWeekRentLand) ? "" : " · " + lastWeekRentLand;'


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
        if len(raw) == 6:  # sprint52 lifetimeNetRoom only one assert value
            pass
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


insert_formatters(F49_50)
git_commit("feat(hud): sprint49-50 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT49, PA49, "lastWeekNetGrantPart")
run_sprint(SPRINT50, PA50, "thisWeekCapitalChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F51_52)
git_commit("feat(hud): sprint51-52 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT51, PA51, "lastWeekGpuGrantPart")
run_sprint(SPRINT52, PA52, "lastWeekRentLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)