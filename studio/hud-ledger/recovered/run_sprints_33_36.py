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

F33_34 = [
    ("LastWeekGreyHardwareSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekNetPowerSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeRevenueNetSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekGrantPatentSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekChipBuildSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekLaunderGreySummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGrantRevenueSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekSiteRoomSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("LifetimeTrainingGrantsSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekCapitalSiteSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRentChipSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekFinesGrantSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
]

F35_36 = [
    ("LastWeekPowerLandSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("construction", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekGrantGreySummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeHardwareChipSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRoomTrainingSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekTrainingNetSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekChipGrantSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("ThisWeekGpuPowerSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimePatentChipSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("LastWeekNetConstructionSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekHiringHardwareSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSalariesChipSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekGreyPatentSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
]

SPRINT33 = [
    ("LastWeekGreyHardwareSummaryLine", "lastWeekGreyHardware", "last week grey hardware", "last_week_grey_hardware_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_300;\n            lab.Ledger.LastWeek.Hardware = -4_600;", "2,300", "4,600"),
    ("ThisWeekNetPowerSummaryLine", "thisWeekNetPower", "this week net power", "this_week_net_power_summary_line",
     "lab.Ledger.ThisWeek.Net = 2_200;\n            lab.Ledger.ThisWeek.Power = -2_900;", "2,200", "2,900"),
    ("LifetimeRevenueNetSummaryLine", "lifetimeRevenueNet", "lifetime revenue net", "lifetime_revenue_net_summary_line",
     "lab.Ledger.Lifetime.Revenue = 650_000;\n            lab.Ledger.Lifetime.Laundered = 23_000;", "650,000", "23,000"),
    ("LastWeekGrantPatentSummaryLine", "lastWeekGrantPatent", "last week grant patent", "last_week_grant_patent_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_400;\n            lab.Ledger.LastWeek.CapitalIn = 36_000;", "5,400", "36,000"),
    ("ThisWeekChipBuildSummaryLine", "thisWeekChipBuild", "this week chip build", "this_week_chip_build_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 4_800;\n            lab.Ledger.ThisWeek.Construction = -3_600;", "4,800", "3,600"),
    ("LastWeekLaunderGreySummaryLine", "lastWeekLaunderGrey", "last week launder grey", "last_week_launder_grey_summary_line",
     "lab.Ledger.LastWeek.Laundered = 2_700;\n            lab.Ledger.LastWeek.GreyRevenue = 1_950;", "2,700", "1,950"),
]

SPRINT34 = [
    ("ThisWeekGrantRevenueSummaryLine", "thisWeekGrantRevenue", "this week grant revenue", "this_week_grant_revenue_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_300;\n            lab.Ledger.ThisWeek.Revenue = 15_600;", "4,300", "15,600"),
    ("LastWeekSiteRoomSummaryLine", "lastWeekSiteRoom", "last week site room", "last_week_site_room_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_400;\n            lab.Ledger.LastWeek.RoomUpkeep = -2_000;", "7,400", "2,000"),
    ("LifetimeTrainingGrantsSummaryLine", "lifetimeTrainingGrants", "lifetime training grants", "lifetime_training_grants_summary_line",
     "lab.Ledger.Lifetime.Grants = 98_000;\n            lab.Ledger.Lifetime.Training = -215_000;", "98,000", "215,000"),
    ("LastWeekCapitalSiteSummaryLine", "lastWeekCapitalSite", "last week capital site", "last_week_capital_site_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 44_000;\n            lab.Ledger.LastWeek.SiteBuild = -8_200;", "44,000", "8,200"),
    ("ThisWeekRentChipSummaryLine", "thisWeekRentChip", "this week rent chip", "this_week_rent_chip_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_920;\n            lab.Ledger.ThisWeek.ChipSales = 5_100;", "1,920", "5,100"),
    ("LastWeekFinesGrantSummaryLine", "lastWeekFinesGrant", "last week fines grant", "last_week_fines_grant_summary_line",
     "lab.Ledger.LastWeek.Fines = -950;\n            lab.Ledger.LastWeek.Grants = 4_800;", "950", "4,800"),
]

SPRINT35 = [
    ("LastWeekPowerLandSummaryLine", "lastWeekPowerLand", "last week power land", "last_week_power_land_summary_line",
     "lab.Ledger.LastWeek.Power = -3_800;\n            lab.Ledger.LastWeek.Land = -14_500;", "3,800", "14,500"),
    ("ThisWeekGrantGreySummaryLine", "thisWeekGrantGrey", "this week grant grey", "this_week_grant_grey_summary_line",
     "lab.Ledger.ThisWeek.Grants = 3_700;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_850;", "3,700", "1,850"),
    ("LifetimeHardwareChipSummaryLine", "lifetimeHardwareChip", "lifetime hardware chip", "lifetime_hardware_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 195_000;\n            lab.Ledger.Lifetime.Power = -122_000;", "195,000", "122,000"),
    ("LastWeekRoomTrainingSummaryLine", "lastWeekRoomTraining", "last week room training", "last_week_room_training_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_300;\n            lab.Ledger.LastWeek.Training = -9_100;", "2,300", "9,100"),
    ("ThisWeekTrainingNetSummaryLine", "thisWeekTrainingNet", "this week training net", "this_week_training_net_summary_line",
     "lab.Ledger.ThisWeek.Training = -5_600;\n            lab.Ledger.ThisWeek.Net = 1_900;", "5,600", "1,900"),
    ("LastWeekChipGrantSummaryLine", "lastWeekChipGrant", "last week chip grant", "last_week_chip_grant_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_500;\n            lab.Ledger.LastWeek.Grants = 5_100;", "7,500", "5,100"),
]

SPRINT36 = [
    ("ThisWeekGpuPowerSummaryLine", "thisWeekGpuPower", "this week gpu power", "this_week_gpu_power_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_050;\n            lab.Ledger.ThisWeek.Power = -2_400;", "2,050", "2,400"),
    ("LifetimePatentChipSummaryLine", "lifetimePatentChip", "lifetime patent chip", "lifetime_patent_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 198_000;\n            lab.Ledger.Lifetime.Revenue = 655_000;", "198,000", "655,000"),
    ("LastWeekNetConstructionSummaryLine", "lastWeekNetConstruction", "last week net construction", "last_week_net_construction_summary_line",
     "lab.Ledger.LastWeek.Net = 2_900;\n            lab.Ledger.LastWeek.Construction = -5_800;", "2,900", "5,800"),
    ("LastWeekHiringHardwareSummaryLine", "lastWeekHiringHardware", "last week hiring hardware", "last_week_hiring_hardware_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_200;\n            lab.Ledger.LastWeek.Hardware = -4_800;", "3,200", "4,800"),
    ("ThisWeekSalariesChipSummaryLine", "thisWeekSalariesChip", "this week salaries chip", "this_week_salaries_chip_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_500;\n            lab.Ledger.ThisWeek.ChipSales = 4_200;", "7,500", "4,200"),
    ("LastWeekGreyPatentSummaryLine", "lastWeekGreyPatent", "last week grey patent", "last_week_grey_patent_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_100;\n            lab.Ledger.LastWeek.Laundered = 2_500;", "2,100", "2,500"),
]

PA33 = '            string lastWeekPatentTrainingPart = string.IsNullOrEmpty(lastWeekPatentTraining) ? "" : " · " + lastWeekPatentTraining;'
PA34 = '            string lastWeekLaunderGreyPart = string.IsNullOrEmpty(lastWeekLaunderGrey) ? "" : " · " + lastWeekLaunderGrey;'
PA35 = '            string lastWeekFinesGrantPart = string.IsNullOrEmpty(lastWeekFinesGrant) ? "" : " · " + lastWeekFinesGrant;'
PA36 = '            string lastWeekChipGrantPart = string.IsNullOrEmpty(lastWeekChipGrant) ? "" : " · " + lastWeekChipGrant;'


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


insert_formatters(F33_34)
git_commit("feat(hud): sprint33-34 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT33, PA33, "lastWeekPatentTrainingPart")
run_sprint(SPRINT34, PA34, "lastWeekLaunderGreyPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F35_36)
git_commit("feat(hud): sprint35-36 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT35, PA35, "lastWeekFinesGrantPart")
run_sprint(SPRINT36, PA36, "lastWeekChipGrantPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)