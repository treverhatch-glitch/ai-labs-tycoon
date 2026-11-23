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

F65_66 = [
    ("ThisWeekSiteTrainingSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LifetimeRentChipSummaryLine", [
        ("gpuRent", "LifetimeLedgerGpuRentLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGreyGrantSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekChipRentSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekGreyPatentSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeFinesGrantSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekLandConstructionSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("ThisWeekRentPowerSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekGreyFinesSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimePowerLandSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekHardwareGrantSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekSiteRentSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
]

F67_68 = [
    ("LifetimeConstructionChipSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekGrantPowerSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekLandGrantSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekSalariesRentSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeLaunderLandSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekSiteFinesSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekCapitalRentSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeGrantRoomSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRentChipSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeLaunderFinesSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("ThisWeekLaunderConstructionSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLaunderFinesSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
]

SPRINT65 = [
    ("ThisWeekSiteTrainingSummaryLine", "thisWeekSiteTraining", "this week site training", "this_week_site_training_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -7_050;\n                        lab.Ledger.ThisWeek.Training = -9_380;", "7,050", "9,380"),
    ("LifetimeRentChipSummaryLine", "lifetimeRentChip", "lifetime rent chip", "lifetime_rent_chip_summary_line",
     "lab.Ledger.Lifetime.GpuRent = -106_000;\n                        lab.Ledger.Lifetime.ChipSales = 240_000;", "106,000", "240,000"),
    ("LifetimeGreyGrantSummaryLine", "lifetimeGreyGrant", "lifetime grey grant", "lifetime_grey_grant_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 49_000;\n                        lab.Ledger.Lifetime.Grants = 128_000;", "49,000", "128,000"),
    ("LastWeekChipRentSummaryLine", "lastWeekChipRent", "last week chip rent", "last_week_chip_rent_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_180;\n                        lab.Ledger.LastWeek.GpuRent = -2_310;", "7,180", "2,310"),
    ("ThisWeekGreyPatentSummaryLine", "thisWeekGreyPatent", "this week grey patent", "this_week_grey_patent_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_900;\n                        lab.Ledger.ThisWeek.Fines = -500;", "1,900", "500"),
    ("LifetimeFinesGrantSummaryLine", "lifetimeFinesGrant", "lifetime fines grant", "lifetime_fines_grant_summary_line",
     "lab.Ledger.Lifetime.Fines = -23_500;\n                        lab.Ledger.Lifetime.Grants = 129_000;", "23,500", "129,000"),
]

SPRINT66 = [
    ("LastWeekLandConstructionSummaryLine", "lastWeekLandConstruction", "last week land construction", "last_week_land_construction_summary_line",
     "lab.Ledger.LastWeek.Land = -11_950;\n                        lab.Ledger.LastWeek.Construction = -6_100;", "11,950", "6,100"),
    ("ThisWeekRentPowerSummaryLine", "thisWeekRentPower", "this week rent power", "this_week_rent_power_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_060;\n                        lab.Ledger.ThisWeek.Power = -2_750;", "2,060", "2,750"),
    ("LastWeekGreyFinesSummaryLine", "lastWeekGreyFines", "last week grey fines", "last_week_grey_fines_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_450;\n                        lab.Ledger.LastWeek.Fines = -590;", "2,450", "590"),
    ("LifetimePowerLandSummaryLine", "lifetimePowerLand", "lifetime power land", "lifetime_power_land_summary_line",
     "lab.Ledger.Lifetime.Power = -145_000;\n                        lab.Ledger.Lifetime.Land = -91_000;", "145,000", "91,000"),
    ("LastWeekHardwareGrantSummaryLine", "lastWeekHardwareGrant", "last week hardware grant", "last_week_hardware_grant_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_550;\n                        lab.Ledger.LastWeek.Grants = 5_350;", "4,550", "5,350"),
    ("LastWeekSiteRentSummaryLine", "lastWeekSiteRent", "last week site rent", "last_week_site_rent_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_350;\n                        lab.Ledger.LastWeek.GpuRent = -2_320;", "7,350", "2,320"),
]

SPRINT67 = [
    ("LifetimeConstructionChipSummaryLine", "lifetimeConstructionChip", "lifetime construction chip", "lifetime_construction_chip_summary_line",
     "lab.Ledger.Lifetime.Construction = -320_000;\n                        lab.Ledger.Lifetime.ChipSales = 242_000;", "320,000", "242,000"),
    ("LastWeekGrantPowerSummaryLine", "lastWeekGrantPower", "last week grant power", "last_week_grant_power_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_400;\n                        lab.Ledger.LastWeek.Power = -3_520;", "5,400", "3,520"),
    ("ThisWeekLandGrantSummaryLine", "thisWeekLandGrant", "this week land grant", "this_week_land_grant_summary_line",
     "lab.Ledger.ThisWeek.Land = -11_200;\n                        lab.Ledger.ThisWeek.Grants = 4_450;", "11,200", "4,450"),
    ("ThisWeekSalariesRentSummaryLine", "thisWeekSalariesRent", "this week salaries rent", "this_week_salaries_rent_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_740;\n                        lab.Ledger.ThisWeek.GpuRent = -2_080;", "7,740", "2,080"),
    ("LifetimeLaunderLandSummaryLine", "lifetimeLaunderLand", "lifetime launder land", "lifetime_launder_land_summary_line",
     "lab.Ledger.Lifetime.Laundered = 29_500;\n                        lab.Ledger.Lifetime.Land = -92_000;", "29,500", "92,000"),
    ("LastWeekSiteFinesSummaryLine", "lastWeekSiteFines", "last week site fines", "last_week_site_fines_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_450;\n                        lab.Ledger.LastWeek.Fines = -600;", "7,450", "600"),
]

SPRINT68 = [
    ("ThisWeekCapitalRentSummaryLine", "thisWeekCapitalRent", "this week capital rent", "this_week_capital_rent_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 48_000;\n                        lab.Ledger.ThisWeek.GpuRent = -2_070;", "48,000", "2,070"),
    ("LifetimeGrantRoomSummaryLine", "lifetimeGrantRoom", "lifetime grant room", "lifetime_grant_room_summary_line",
     "lab.Ledger.Lifetime.Grants = 130_000;\n                        lab.Ledger.Lifetime.RoomUpkeep = -62_000;", "130,000", "62,000"),
    ("LastWeekRentChipSummaryLine", "lastWeekRentChip", "last week rent chip", "last_week_rent_chip_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_340;\n                        lab.Ledger.LastWeek.ChipSales = 7_220;", "2,340", "7,220"),
    ("LifetimeLaunderFinesSummaryLine", "lifetimeLaunderFines", "lifetime launder fines", "lifetime_launder_fines_summary_line",
     "lab.Ledger.Lifetime.Laundered = 30_000;\n                        lab.Ledger.Lifetime.Fines = -24_000;", "30,000", "24,000"),
    ("ThisWeekLaunderConstructionSummaryLine", "thisWeekLaunderConstruction", "this week launder construction", "this_week_launder_construction_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_800;\n                        lab.Ledger.ThisWeek.Construction = -6_000;", "2,800", "6,000"),
    ("LastWeekLaunderFinesSummaryLine", "lastWeekLaunderFines", "last week launder fines", "last_week_launder_fines_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_250;\n                        lab.Ledger.LastWeek.Fines = -610;", "3,250", "610"),
]

PA65 = '            string thisWeekGrantLaunderPart = string.IsNullOrEmpty(thisWeekGrantLaunder) ? "" : " · " + thisWeekGrantLaunder;'
PA66 = '            string lifetimeFinesGrantPart = string.IsNullOrEmpty(lifetimeFinesGrant) ? "" : " · " + lifetimeFinesGrant;'
PA67 = '            string lastWeekSiteRentPart = string.IsNullOrEmpty(lastWeekSiteRent) ? "" : " · " + lastWeekSiteRent;'
PA68 = '            string lastWeekSiteFinesPart = string.IsNullOrEmpty(lastWeekSiteFines) ? "" : " · " + lastWeekSiteFines;'


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


insert_formatters(F65_66)
git_commit("feat(hud): sprint65-66 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT65, PA65, "thisWeekGrantLaunderPart")
run_sprint(SPRINT66, PA66, "lifetimeFinesGrantPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F67_68)
git_commit("feat(hud): sprint67-68 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT67, PA67, "lastWeekSiteRentPart")
run_sprint(SPRINT68, PA68, "lastWeekSiteFinesPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
