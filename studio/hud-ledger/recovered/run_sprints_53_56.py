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

F53_54 = [
    ("LifetimePowerGrantSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekGreyGrantSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekFinesLandSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeHardwareRentSummaryLine", [
        ("hardware", "LifetimeLedgerHardwareLine(lab)"),
        ("gpuRent", "LifetimeLedgerGpuRentLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LastWeekCapitalPowerSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPatentTrainingSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLaunderPowerSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeGreyChipSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekRentConstructionSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeSalariesPowerSummaryLine", [
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekFinesRoomSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekLandChipSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
]

F55_56 = [
    ("ThisWeekHardwareRoomSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimePatentGrantSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekGreyChipSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekCapitalGrantSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekTrainingChipSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekSiteFinesSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGrantPatentSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekLaunderGrantSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekChipPowerSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeConstructionPowerSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringGrantSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekGreyRentSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
]

SPRINT53 = [
    ("LifetimePowerGrantSummaryLine", "lifetimePowerGrant", "lifetime power grant", "lifetime_power_grant_summary_line",
     "lab.Ledger.Lifetime.Power = -135_000;\n            lab.Ledger.Lifetime.Grants = 118_000;", "135,000", "118,000"),
    ("LastWeekGreyGrantSummaryLine", "lastWeekGreyGrant", "last week grey grant", "last_week_grey_grant_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_250;\n            lab.Ledger.LastWeek.Grants = 4_800;", "2,250", "4,800"),
    ("ThisWeekFinesLandSummaryLine", "thisWeekFinesLand", "this week fines land", "this_week_fines_land_summary_line",
     "lab.Ledger.ThisWeek.Fines = -450;\n            lab.Ledger.ThisWeek.Land = -10_300;", "450", "10,300"),
    ("LifetimeHardwareRentSummaryLine", "lifetimeHardwareRent", "lifetime hardware rent", "lifetime_hardware_rent_summary_line",
     "lab.Ledger.Lifetime.Hardware = -150_000;\n            lab.Ledger.Lifetime.GpuRent = -98_000;", "150,000", "98,000"),
    ("LastWeekCapitalPowerSummaryLine", "lastWeekCapitalPower", "last week capital power", "last_week_capital_power_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 41_000;\n            lab.Ledger.LastWeek.Power = -3_500;", "41,000", "3,500"),
    ("ThisWeekPatentTrainingSummaryLine", "thisWeekPatentTraining", "this week patent training", "this_week_patent_training_summary_line",
     "lab.Ledger.ThisWeek.Training = -9_400;\n            lab.Ledger.ThisWeek.Grants = 3_850;", "9,400", "3,850"),
]

SPRINT54 = [
    ("ThisWeekLaunderPowerSummaryLine", "thisWeekLaunderPower", "this week launder power", "this_week_launder_power_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_400;\n            lab.Ledger.ThisWeek.Power = -2_650;", "2,400", "2,650"),
    ("LifetimeGreyChipSummaryLine", "lifetimeGreyChip", "lifetime grey chip", "lifetime_grey_chip_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 47_500;\n            lab.Ledger.Lifetime.ChipSales = 228_000;", "47,500", "228,000"),
    ("LastWeekRentConstructionSummaryLine", "lastWeekRentConstruction", "last week rent construction", "last_week_rent_construction_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_200;\n            lab.Ledger.LastWeek.Construction = -6_000;", "2,200", "6,000"),
    ("LifetimeSalariesPowerSummaryLine", "lifetimeSalariesPower", "lifetime salaries power", "lifetime_salaries_power_summary_line",
     "lab.Ledger.Lifetime.Salaries = -410_000;\n            lab.Ledger.Lifetime.Power = -138_000;", "410,000", "138,000"),
    ("LastWeekFinesRoomSummaryLine", "lastWeekFinesRoom", "last week fines room", "last_week_fines_room_summary_line",
     "lab.Ledger.LastWeek.Fines = -530;\n            lab.Ledger.LastWeek.RoomUpkeep = -2_180;", "530", "2,180"),
    ("LastWeekLandChipSummaryLine", "lastWeekLandChip", "last week land chip", "last_week_land_chip_summary_line",
     "lab.Ledger.LastWeek.Land = -11_800;\n            lab.Ledger.LastWeek.ChipSales = 7_300;", "11,800", "7,300"),
]

SPRINT55 = [
    ("ThisWeekHardwareRoomSummaryLine", "thisWeekHardwareRoom", "this week hardware room", "this_week_hardware_room_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_250;\n            lab.Ledger.ThisWeek.RoomUpkeep = -2_100;", "4,250", "2,100"),
    ("LifetimePatentGrantSummaryLine", "lifetimePatentGrant", "lifetime patent grant", "lifetime_patent_grant_summary_line",
     "lab.Ledger.Lifetime.Grants = 120_000;\n            lab.Ledger.Lifetime.Revenue = 520_000;", "120,000", "520,000"),
    ("LastWeekGreyChipSummaryLine", "lastWeekGreyChip", "last week grey chip", "last_week_grey_chip_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_300;\n            lab.Ledger.LastWeek.ChipSales = 6_950;", "2,300", "6,950"),
    ("ThisWeekCapitalGrantSummaryLine", "thisWeekCapitalGrant", "this week capital grant", "this_week_capital_grant_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 45_500;\n            lab.Ledger.ThisWeek.Grants = 4_050;", "45,500", "4,050"),
    ("LastWeekTrainingChipSummaryLine", "lastWeekTrainingChip", "last week training chip", "last_week_training_chip_summary_line",
     "lab.Ledger.LastWeek.Training = -8_950;\n            lab.Ledger.LastWeek.ChipSales = 7_050;", "8,950", "7,050"),
    ("ThisWeekSiteFinesSummaryLine", "thisWeekSiteFines", "this week site fines", "this_week_site_fines_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -7_100;\n            lab.Ledger.ThisWeek.Fines = -470;", "7,100", "470"),
]

SPRINT56 = [
    ("ThisWeekGrantPatentSummaryLine", "thisWeekGrantPatent", "this week grant patent", "this_week_grant_patent_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_150;\n            lab.Ledger.ThisWeek.ChipDevelopment = -5_300;", "4,150", "5,300"),
    ("LastWeekLaunderGrantSummaryLine", "lastWeekLaunderGrant", "last week launder grant", "last_week_launder_grant_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_100;\n            lab.Ledger.LastWeek.Grants = 4_750;", "3,100", "4,750"),
    ("ThisWeekChipPowerSummaryLine", "thisWeekChipPower", "this week chip power", "this_week_chip_power_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_200;\n            lab.Ledger.ThisWeek.Power = -2_720;", "5,200", "2,720"),
    ("LifetimeConstructionPowerSummaryLine", "lifetimeConstructionPower", "lifetime construction power", "lifetime_construction_power_summary_line",
     "lab.Ledger.Lifetime.Construction = -310_000;\n            lab.Ledger.Lifetime.Power = -140_000;", "310,000", "140,000"),
    ("LastWeekHiringGrantSummaryLine", "lastWeekHiringGrant", "last week hiring grant", "last_week_hiring_grant_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_450;\n            lab.Ledger.LastWeek.Grants = 4_850;", "3,450", "4,850"),
    ("ThisWeekGreyRentSummaryLine", "thisWeekGreyRent", "this week grey rent", "this_week_grey_rent_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_850;\n            lab.Ledger.ThisWeek.GpuRent = -2_000;", "1,850", "2,000"),
]

PA53 = '            string lifetimeNetRoomPart = string.IsNullOrEmpty(lifetimeNetRoom) ? "" : " · " + lifetimeNetRoom;'
PA54 = '            string thisWeekPatentTrainingPart = string.IsNullOrEmpty(thisWeekPatentTraining) ? "" : " · " + thisWeekPatentTraining;'
PA55 = '            string lastWeekLandChipPart = string.IsNullOrEmpty(lastWeekLandChip) ? "" : " · " + lastWeekLandChip;'
PA56 = '            string thisWeekSiteFinesPart = string.IsNullOrEmpty(thisWeekSiteFines) ? "" : " · " + thisWeekSiteFines;'


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


insert_formatters(F53_54)
git_commit("feat(hud): sprint53-54 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT53, PA53, "lifetimeNetRoomPart")
run_sprint(SPRINT54, PA54, "thisWeekPatentTrainingPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F55_56)
git_commit("feat(hud): sprint55-56 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT55, PA55, "lastWeekLandChipPart")
run_sprint(SPRINT56, PA56, "thisWeekSiteFinesPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)