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

F41_42 = [
    ("ThisWeekChipDevSummaryLine", [
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekFinesGpuSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LifetimeRevenueTrainingSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHiringSalariesSummaryLine", [
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGrantLandSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekPatentGreySummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPowerRentSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekCapitalLandSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
    ]),
    ("LastWeekGreyPowerSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekSiteRevenueSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekNetChipSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeConstructionGrantSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
]

F43_44 = [
    ("ThisWeekLaunderFinesSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekTrainingPowerSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
    ]),
    ("LifetimePatentRevenueSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSalariesConstructionSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekChipDevGrantSummaryLine", [
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekRentRevenueSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHardwareFinesSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeRoomGrantSummaryLine", [
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("LastWeekChipFinesSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekLandTrainingSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekGrantGpuSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("LifetimeNetPowerSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
]

SPRINT41 = [
    ("ThisWeekChipDevSummaryLine", "thisWeekChipDev", "this week chip dev", "this_week_chip_dev_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -5_400;\n            lab.Ledger.ThisWeek.ChipSales = 4_200;", "5,400", "4,200"),
    ("LastWeekFinesGpuSummaryLine", "lastWeekFinesGpu", "last week fines gpu", "last_week_fines_gpu_summary_line",
     "lab.Ledger.LastWeek.Fines = -520;\n            lab.Ledger.LastWeek.GpuRent = -2_100;", "520", "2,100"),
    ("LifetimeRevenueTrainingSummaryLine", "lifetimeRevenueTraining", "lifetime revenue training", "lifetime_revenue_training_summary_line",
     "lab.Ledger.Lifetime.Revenue = 500_000;\n            lab.Ledger.Lifetime.Training = -200_000;", "500,000", "200,000"),
    ("ThisWeekHiringSalariesSummaryLine", "thisWeekHiringSalaries", "this week hiring salaries", "this_week_hiring_salaries_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -3_200;\n            lab.Ledger.ThisWeek.Salaries = -7_600;", "3,200", "7,600"),
    ("LastWeekGrantLandSummaryLine", "lastWeekGrantLand", "last week grant land", "last_week_grant_land_summary_line",
     "lab.Ledger.LastWeek.Grants = 4_800;\n            lab.Ledger.LastWeek.Land = -12_500;", "4,800", "12,500"),
    ("ThisWeekPatentGreySummaryLine", "thisWeekPatentGrey", "this week patent grey", "this_week_patent_grey_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_650;\n            lab.Ledger.ThisWeek.ChipSales = 4_900;", "1,650", "4,900"),
]

SPRINT42 = [
    ("LastWeekPowerRentSummaryLine", "lastWeekPowerRent", "last week power rent", "last_week_power_rent_summary_line",
     "lab.Ledger.LastWeek.Power = -3_400;\n            lab.Ledger.LastWeek.GpuRent = -2_050;", "3,400", "2,050"),
    ("ThisWeekCapitalLandSummaryLine", "thisWeekCapitalLand", "this week capital land", "this_week_capital_land_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 35_000;\n            lab.Ledger.ThisWeek.Land = -9_800;", "35,000", "9,800"),
    ("LastWeekGreyPowerSummaryLine", "lastWeekGreyPower", "last week grey power", "last_week_grey_power_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_100;\n            lab.Ledger.LastWeek.Power = -2_900;", "2,100", "2,900"),
    ("ThisWeekSiteRevenueSummaryLine", "thisWeekSiteRevenue", "this week site revenue", "this_week_site_revenue_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -6_700;\n            lab.Ledger.ThisWeek.Revenue = 15_200;", "6,700", "15,200"),
    ("LastWeekNetChipSummaryLine", "lastWeekNetChip", "last week net chip", "last_week_net_chip_summary_line",
     "lab.Ledger.LastWeek.Net = 2_400;\n            lab.Ledger.LastWeek.ChipSales = 7_100;", "2,400", "7,100"),
    ("LifetimeConstructionGrantSummaryLine", "lifetimeConstructionGrant", "lifetime construction grant", "lifetime_construction_grant_summary_line",
     "lab.Ledger.Lifetime.Construction = -300_000;\n            lab.Ledger.Lifetime.Grants = 105_000;", "300,000", "105,000"),
]

SPRINT43 = [
    ("ThisWeekLaunderFinesSummaryLine", "thisWeekLaunderFines", "this week launder fines", "this_week_launder_fines_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_100;\n            lab.Ledger.ThisWeek.Fines = -380;", "2,100", "380"),
    ("LastWeekTrainingPowerSummaryLine", "lastWeekTrainingPower", "last week training power", "last_week_training_power_summary_line",
     "lab.Ledger.LastWeek.Training = -8_800;\n            lab.Ledger.LastWeek.Power = -3_100;", "8,800", "3,100"),
    ("LifetimePatentRevenueSummaryLine", "lifetimePatentRevenue", "lifetime patent revenue", "lifetime_patent_revenue_summary_line",
     "lab.Ledger.Lifetime.Revenue = 510_000;\n            lab.Ledger.Lifetime.ChipSales = 220_000;", "510,000", "220,000"),
    ("LastWeekSalariesConstructionSummaryLine", "lastWeekSalariesConstruction", "last week salaries construction", "last_week_salaries_construction_summary_line",
     "lab.Ledger.LastWeek.Salaries = -7_300;\n            lab.Ledger.LastWeek.Construction = -5_900;", "7,300", "5,900"),
    ("ThisWeekChipDevGrantSummaryLine", "thisWeekChipDevGrant", "this week chip dev grant", "this_week_chip_dev_grant_summary_line",
     "lab.Ledger.ThisWeek.ChipDevelopment = -4_900;\n            lab.Ledger.ThisWeek.Grants = 3_600;", "4,900", "3,600"),
    ("LastWeekRentRevenueSummaryLine", "lastWeekRentRevenue", "last week rent revenue", "last_week_rent_revenue_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_180;\n            lab.Ledger.LastWeek.Revenue = 18_400;", "2,180", "18,400"),
]

SPRINT44 = [
    ("ThisWeekHardwareFinesSummaryLine", "thisWeekHardwareFines", "this week hardware fines", "this_week_hardware_fines_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_200;\n            lab.Ledger.ThisWeek.Fines = -420;", "4,200", "420"),
    ("LifetimeRoomGrantSummaryLine", "lifetimeRoomGrant", "lifetime room grant", "lifetime_room_grant_summary_line",
     "lab.Ledger.Lifetime.RoomUpkeep = -50_000;\n            lab.Ledger.Lifetime.Grants = 108_000;", "50,000", "108,000"),
    ("LastWeekChipFinesSummaryLine", "lastWeekChipFines", "last week chip fines", "last_week_chip_fines_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 6_800;\n            lab.Ledger.LastWeek.Fines = -490;", "6,800", "490"),
    ("ThisWeekLandTrainingSummaryLine", "thisWeekLandTraining", "this week land training", "this_week_land_training_summary_line",
     "lab.Ledger.ThisWeek.Land = -10_200;\n            lab.Ledger.ThisWeek.Training = -9_100;", "10,200", "9,100"),
    ("LastWeekGrantGpuSummaryLine", "lastWeekGrantGpu", "last week grant gpu", "last_week_grant_gpu_summary_line",
     "lab.Ledger.LastWeek.Grants = 4_500;\n            lab.Ledger.LastWeek.GpuRent = -2_300;", "4,500", "2,300"),
    ("LifetimeNetPowerSummaryLine", "lifetimeNetPower", "lifetime net power", "lifetime_net_power_summary_line",
     "lab.Ledger.Lifetime.Net = 50_000;\n            lab.Ledger.Lifetime.Power = -130_000;", "50,000", "130,000"),
]

PA41 = '            string lastWeekRoomNetPart = string.IsNullOrEmpty(lastWeekRoomNet) ? "" : " · " + lastWeekRoomNet;'
PA42 = '            string thisWeekPatentGreyPart = string.IsNullOrEmpty(thisWeekPatentGrey) ? "" : " · " + thisWeekPatentGrey;'
PA43 = '            string lifetimeConstructionGrantPart = string.IsNullOrEmpty(lifetimeConstructionGrant) ? "" : " · " + lifetimeConstructionGrant;'
PA44 = '            string lastWeekRentRevenuePart = string.IsNullOrEmpty(lastWeekRentRevenue) ? "" : " · " + lastWeekRentRevenue;'


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


insert_formatters(F41_42)
git_commit("feat(hud): sprint41-42 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT41, PA41, "lastWeekRoomNetPart")
run_sprint(SPRINT42, PA42, "thisWeekPatentGreyPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F43_44)
git_commit("feat(hud): sprint43-44 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT43, PA43, "lifetimeConstructionGrantPart")
run_sprint(SPRINT44, PA44, "lastWeekRentRevenuePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)