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

F57_58 = [
    ("LastWeekSiteGrantSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekLaunderLandSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeRentGrantSummaryLine", [
        ("gpuRent", "LifetimeLedgerGpuRentLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekCapitalChipSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeGreyLandSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekConstructionLandSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekPatentRoomSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGrantHardwareSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekGreyRentSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekChipLandSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekSiteNetSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGrantChipSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
]

F59_60 = [
    ("LifetimeLaunderGrantSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekRentTrainingSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeRoomChipSummaryLine", [
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekFinesChipSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekCapitalPowerSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekSiteLandSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekLaunderLandSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekGrantSiteSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeChipLandSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRentTrainingSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("LifetimeHiringPowerSummaryLine", [
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekNetRoomSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
]

SPRINT57 = [
    ("LastWeekSiteGrantSummaryLine", "lastWeekSiteGrant", "last week site grant", "last_week_site_grant_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_200;\n            lab.Ledger.LastWeek.Grants = 4_950;", "7,200", "4,950"),
    ("ThisWeekLaunderLandSummaryLine", "thisWeekLaunderLand", "this week launder land", "this_week_launder_land_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_500;\n            lab.Ledger.ThisWeek.Land = -10_700;", "2,500", "10,700"),
    ("LifetimeRentGrantSummaryLine", "lifetimeRentGrant", "lifetime rent grant", "lifetime_rent_grant_summary_line",
     "lab.Ledger.Lifetime.GpuRent = -102_000;\n            lab.Ledger.Lifetime.Grants = 122_000;", "102,000", "122,000"),
    ("LastWeekCapitalChipSummaryLine", "lastWeekCapitalChip", "last week capital chip", "last_week_capital_chip_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 39_500;\n            lab.Ledger.LastWeek.ChipSales = 7_100;", "39,500", "7,100"),
    ("LifetimeGreyLandSummaryLine", "lifetimeGreyLand", "lifetime grey land", "lifetime_grey_land_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 48_000;\n            lab.Ledger.Lifetime.Land = -85_000;", "48,000", "85,000"),
    ("ThisWeekConstructionLandSummaryLine", "thisWeekConstructionLand", "this week construction land", "this_week_construction_land_summary_line",
     "lab.Ledger.ThisWeek.Construction = -5_900;\n            lab.Ledger.ThisWeek.Land = -10_900;", "5,900", "10,900"),
]

SPRINT58 = [
    ("LastWeekPatentRoomSummaryLine", "lastWeekPatentRoom", "last week patent room", "last_week_patent_room_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_190;\n            lab.Ledger.LastWeek.Fines = -540;", "2,190", "540"),
    ("ThisWeekGrantHardwareSummaryLine", "thisWeekGrantHardware", "this week grant hardware", "this_week_grant_hardware_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_200;\n            lab.Ledger.ThisWeek.Hardware = -4_350;", "4,200", "4,350"),
    ("LastWeekGreyRentSummaryLine", "lastWeekGreyRent", "last week grey rent", "last_week_grey_rent_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_350;\n            lab.Ledger.LastWeek.GpuRent = -2_280;", "2,350", "2,280"),
    ("ThisWeekChipLandSummaryLine", "thisWeekChipLand", "this week chip land", "this_week_chip_land_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_150;\n            lab.Ledger.ThisWeek.Land = -10_800;", "5,150", "10,800"),
    ("ThisWeekSiteNetSummaryLine", "thisWeekSiteNet", "this week site net", "this_week_site_net_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -7_000;\n            lab.Ledger.ThisWeek.Net = 2_050;", "7,000", "2,050"),
    ("LastWeekGrantChipSummaryLine", "lastWeekGrantChip", "last week grant chip", "last_week_grant_chip_summary_line",
     "lab.Ledger.LastWeek.Grants = 4_900;\n            lab.Ledger.LastWeek.ChipSales = 7_150;", "4,900", "7,150"),
]

SPRINT59 = [
    ("LifetimeLaunderGrantSummaryLine", "lifetimeLaunderGrant", "lifetime launder grant", "lifetime_launder_grant_summary_line",
     "lab.Ledger.Lifetime.Laundered = 28_000;\n            lab.Ledger.Lifetime.Grants = 123_000;", "28,000", "123,000"),
    ("ThisWeekRentTrainingSummaryLine", "thisWeekRentTraining", "this week rent training", "this_week_rent_training_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_010;\n            lab.Ledger.ThisWeek.Training = -9_350;", "2,010", "9,350"),
    ("LifetimeRoomChipSummaryLine", "lifetimeRoomChip", "lifetime room chip", "lifetime_room_chip_summary_line",
     "lab.Ledger.Lifetime.RoomUpkeep = -58_000;\n            lab.Ledger.Lifetime.ChipSales = 230_000;", "58,000", "230,000"),
    ("LastWeekFinesChipSummaryLine", "lastWeekFinesChip", "last week fines chip", "last_week_fines_chip_summary_line",
     "lab.Ledger.LastWeek.Fines = -560;\n            lab.Ledger.LastWeek.ChipSales = 7_050;", "560", "7,050"),
    ("ThisWeekCapitalPowerSummaryLine", "thisWeekCapitalPower", "this week capital power", "this_week_capital_power_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 46_000;\n            lab.Ledger.ThisWeek.Power = -2_780;", "46,000", "2,780"),
    ("LastWeekSiteLandSummaryLine", "lastWeekSiteLand", "last week site land", "last_week_site_land_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_400;\n            lab.Ledger.LastWeek.Land = -11_600;", "7,400", "11,600"),
]

SPRINT60 = [
    ("LastWeekLaunderLandSummaryLine", "lastWeekLaunderLand", "last week launder land", "last_week_launder_land_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_150;\n            lab.Ledger.LastWeek.Land = -11_900;", "3,150", "11,900"),
    ("ThisWeekGrantSiteSummaryLine", "thisWeekGrantSite", "this week grant site", "this_week_grant_site_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_300;\n            lab.Ledger.ThisWeek.SiteBuild = -6_950;", "4,300", "6,950"),
    ("LifetimeChipLandSummaryLine", "lifetimeChipLand", "lifetime chip land", "lifetime_chip_land_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 232_000;\n            lab.Ledger.Lifetime.Land = -86_000;", "232,000", "86,000"),
    ("LastWeekRentTrainingSummaryLine", "lastWeekRentTraining", "last week rent training", "last_week_rent_training_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_260;\n            lab.Ledger.LastWeek.Training = -8_850;", "2,260", "8,850"),
    ("LifetimeHiringPowerSummaryLine", "lifetimeHiringPower", "lifetime hiring power", "lifetime_hiring_power_summary_line",
     "lab.Ledger.Lifetime.Hiring = -95_000;\n            lab.Ledger.Lifetime.Power = -142_000;", "95,000", "142,000"),
    ("ThisWeekNetRoomSummaryLine", "thisWeekNetRoom", "this week net room", "this_week_net_room_summary_line",
     "lab.Ledger.ThisWeek.Net = 2_150;\n            lab.Ledger.ThisWeek.RoomUpkeep = -2_200;", "2,150", "2,200"),
]

PA57 = '            string thisWeekGreyRentPart = string.IsNullOrEmpty(thisWeekGreyRent) ? "" : " · " + thisWeekGreyRent;'
PA58 = '            string thisWeekConstructionLandPart = string.IsNullOrEmpty(thisWeekConstructionLand) ? "" : " · " + thisWeekConstructionLand;'
PA59 = '            string lastWeekGrantChipPart = string.IsNullOrEmpty(lastWeekGrantChip) ? "" : " · " + lastWeekGrantChip;'
PA60 = '            string lastWeekSiteLandPart = string.IsNullOrEmpty(lastWeekSiteLand) ? "" : " · " + lastWeekSiteLand;'


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


insert_formatters(F57_58)
git_commit("feat(hud): sprint57-58 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT57, PA57, "thisWeekGreyRentPart")
run_sprint(SPRINT58, PA58, "thisWeekConstructionLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F59_60)
git_commit("feat(hud): sprint59-60 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT59, PA59, "lastWeekGrantChipPart")
run_sprint(SPRINT60, PA60, "lastWeekSiteLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)