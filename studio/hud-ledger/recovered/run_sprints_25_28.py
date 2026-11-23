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

F25_26 = [
    ("LastWeekRevenueCapitalSummaryLine", [
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekNetGrantSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeLaunderNetSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekFinesHardwareSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekSalariesPowerSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekGrantSiteSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("ThisWeekConstructionChipSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekNetLandSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LifetimeHardwareTrainingSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekGreyGrantSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekChipPowerSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
    ]),
    ("ThisWeekTrainingFinesSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
]

F27_28 = [
    ("LastWeekSalariesSiteSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
    ]),
    ("ThisWeekRevenueGreySummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimePatentGrantsSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("LastWeekTrainingCapitalSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekGpuGrantSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGreyLandSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekNetConstructionSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
    ]),
    ("LastWeekRevenueLaunderSummaryLine", [
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeSiteChipSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekFinesGreySummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
    ]),
    ("LastWeekHardwareNetSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("construction", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekGrantTrainingSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("chipSales", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
]

SPRINT25 = [
    ("LastWeekRevenueCapitalSummaryLine", "lastWeekRevenueCapital", "last week revenue capital", "last_week_revenue_capital_summary_line",
     "lab.Ledger.LastWeek.Revenue = 24_000;\n            lab.Ledger.LastWeek.CapitalIn = 45_000;", "24,000", "45,000"),
    ("ThisWeekNetGrantSummaryLine", "thisWeekNetGrant", "this week net grant", "this_week_net_grant_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_100;\n            lab.Ledger.ThisWeek.Net = 2_300;", "4,100", "2,300"),
    ("LifetimeLaunderNetSummaryLine", "lifetimeLaunderNet", "lifetime launder net", "lifetime_launder_net_summary_line",
     "lab.Ledger.Lifetime.Laundered = 20_500;\n            lab.Ledger.Lifetime.GreyRevenue = 38_000;", "20,500", "38,000"),
    ("LastWeekFinesHardwareSummaryLine", "lastWeekFinesHardware", "last week fines hardware", "last_week_fines_hardware_summary_line",
     "lab.Ledger.LastWeek.Fines = -880;\n            lab.Ledger.LastWeek.Hardware = -4_200;", "880", "4,200"),
    ("ThisWeekSalariesPowerSummaryLine", "thisWeekSalariesPower", "this week salaries power", "this_week_salaries_power_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_600;\n            lab.Ledger.ThisWeek.Power = -2_700;", "7,600", "2,700"),
    ("LastWeekGrantSiteSummaryLine", "lastWeekGrantSite", "last week grant site", "last_week_grant_site_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_800;\n            lab.Ledger.LastWeek.SiteBuild = -9_200;", "5,800", "9,200"),
]

SPRINT26 = [
    ("ThisWeekConstructionChipSummaryLine", "thisWeekConstructionChip", "this week construction chip", "this_week_construction_chip_summary_line",
     "lab.Ledger.ThisWeek.Construction = -3_800;\n            lab.Ledger.ThisWeek.ChipSales = 4_900;", "3,800", "4,900"),
    ("LastWeekNetLandSummaryLine", "lastWeekNetLand", "last week net land", "last_week_net_land_summary_line",
     "lab.Ledger.LastWeek.Land = -16_000;\n            lab.Ledger.LastWeek.Net = 3_100;", "16,000", "3,100"),
    ("LifetimeHardwareTrainingSummaryLine", "lifetimeHardwareTraining", "lifetime hardware training", "lifetime_hardware_training_summary_line",
     "lab.Ledger.Lifetime.Training = -210_000;\n            lab.Ledger.Lifetime.Power = -105_000;", "210,000", "105,000"),
    ("ThisWeekGreyGrantSummaryLine", "thisWeekGreyGrant", "this week grey grant", "this_week_grey_grant_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_900;\n            lab.Ledger.ThisWeek.Grants = 3_600;", "1,900", "3,600"),
    ("LastWeekChipPowerSummaryLine", "lastWeekChipPower", "last week chip power", "last_week_chip_power_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 8_100;\n            lab.Ledger.LastWeek.Power = -3_400;", "8,100", "3,400"),
    ("ThisWeekTrainingFinesSummaryLine", "thisWeekTrainingFines", "this week training fines", "this_week_training_fines_summary_line",
     "lab.Ledger.ThisWeek.Training = -5_500;\n            lab.Ledger.ThisWeek.Fines = -390;", "5,500", "390"),
]

SPRINT27 = [
    ("LastWeekSalariesSiteSummaryLine", "lastWeekSalariesSite", "last week salaries site", "last_week_salaries_site_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_400;\n            lab.Ledger.LastWeek.SiteBuild = -7_600;", "8,400", "7,600"),
    ("ThisWeekRevenueGreySummaryLine", "thisWeekRevenueGrey", "this week revenue grey", "this_week_revenue_grey_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 14_200;\n            lab.Ledger.ThisWeek.GreyRevenue = 2_050;", "14,200", "2,050"),
    ("LifetimePatentGrantsSummaryLine", "lifetimePatentGrants", "lifetime patent grants", "lifetime_patent_grants_summary_line",
     "lab.Ledger.Lifetime.Grants = 92_000;\n            lab.Ledger.Lifetime.Revenue = 640_000;", "92,000", "640,000"),
    ("LastWeekTrainingCapitalSummaryLine", "lastWeekTrainingCapital", "last week training capital", "last_week_training_capital_summary_line",
     "lab.Ledger.LastWeek.Training = -10_200;\n            lab.Ledger.LastWeek.CapitalIn = 38_000;", "10,200", "38,000"),
    ("ThisWeekGpuGrantSummaryLine", "thisWeekGpuGrant", "this week gpu grant", "this_week_gpu_grant_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_880;\n            lab.Ledger.ThisWeek.Grants = 4_400;", "1,880", "4,400"),
    ("LastWeekGreyLandSummaryLine", "lastWeekGreyLand", "last week grey land", "last_week_grey_land_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_200;\n            lab.Ledger.LastWeek.Land = -12_500;", "2,200", "12,500"),
]

SPRINT28 = [
    ("ThisWeekNetConstructionSummaryLine", "thisWeekNetConstruction", "this week net construction", "this_week_net_construction_summary_line",
     "lab.Ledger.ThisWeek.Net = 1_950;\n            lab.Ledger.ThisWeek.Construction = -4_300;", "1,950", "4,300"),
    ("LastWeekRevenueLaunderSummaryLine", "lastWeekRevenueLaunder", "last week revenue launder", "last_week_revenue_launder_summary_line",
     "lab.Ledger.LastWeek.Revenue = 21_000;\n            lab.Ledger.LastWeek.Laundered = 2_600;", "21,000", "2,600"),
    ("LifetimeSiteChipSummaryLine", "lifetimeSiteChip", "lifetime site chip", "lifetime_site_chip_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 190_000;\n            lab.Ledger.Lifetime.Grants = 96_000;", "190,000", "96,000"),
    ("ThisWeekFinesGreySummaryLine", "thisWeekFinesGrey", "this week fines grey", "this_week_fines_grey_summary_line",
     "lab.Ledger.ThisWeek.Fines = -520;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_750;", "520", "1,750"),
    ("LastWeekHardwareNetSummaryLine", "lastWeekHardwareNet", "last week hardware net", "last_week_hardware_net_summary_line",
     "lab.Ledger.LastWeek.Hardware = -5_600;\n            lab.Ledger.LastWeek.Net = 2_800;", "5,600", "2,800"),
    ("ThisWeekGrantTrainingSummaryLine", "thisWeekGrantTraining2", "this week grant training", "this_week_grant_training_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_200;\n            lab.Ledger.ThisWeek.Training = -6_300;", "5,200", "6,300"),
]

PA25 = '            string lastWeekTrainingGreyPart = string.IsNullOrEmpty(lastWeekTrainingGrey) ? "" : " · " + lastWeekTrainingGrey;'
PA26 = '            string lastWeekGrantSitePart = string.IsNullOrEmpty(lastWeekGrantSite) ? "" : " · " + lastWeekGrantSite;'
PA27 = '            string thisWeekTrainingFinesPart = string.IsNullOrEmpty(thisWeekTrainingFines) ? "" : " · " + thisWeekTrainingFines;'
PA28 = '            string lastWeekGreyLandPart = string.IsNullOrEmpty(lastWeekGreyLand) ? "" : " · " + lastWeekGreyLand;'


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


insert_formatters(F25_26)
git_commit("feat(hud): sprint25-26 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT25, PA25, "lastWeekTrainingGreyPart")
run_sprint(SPRINT26, PA26, "lastWeekGrantSitePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F27_28)
git_commit("feat(hud): sprint27-28 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT27, PA27, "thisWeekTrainingFinesPart")
run_sprint(SPRINT28, PA28, "lastWeekGreyLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)