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

F37_38 = [
    ("LastWeekLandGrantSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("ThisWeekFinesGrantSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeSiteGreySummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGpuHardwareSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
    ]),
    ("ThisWeekChipGreyGrantSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekTrainingHiringSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekRevenueConstructionSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeGrantFinesSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekSalariesGreySummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekRevenueSiteSummaryLine", [
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekRentFinesSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekConstructionGrantSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
]

F39_40 = [
    ("ThisWeekNetSalariesSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LifetimeLaunderPowerSummaryLine", [
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekPatentLandSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
    ]),
    ("ThisWeekPowerConstructionSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekLaunderCapitalSummaryLine", [
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekGreySalariesSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeNetGrantSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekHardwareTrainingSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekGrantNetChipSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
    ("LastWeekSitePowerSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekGrantFinesSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekRoomNetSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
]

SPRINT37 = [
    ("LastWeekLandGrantSummaryLine", "lastWeekLandGrant", "last week land grant", "last_week_land_grant_summary_line",
     "lab.Ledger.LastWeek.Land = -13_000;\n            lab.Ledger.LastWeek.Grants = 5_200;", "13,000", "5,200"),
    ("ThisWeekFinesGrantSummaryLine", "thisWeekFinesGrant", "this week fines grant", "this_week_fines_grant_summary_line",
     "lab.Ledger.ThisWeek.Fines = -410;\n            lab.Ledger.ThisWeek.Grants = 3_900;", "410", "3,900"),
    ("LifetimeSiteGreySummaryLine", "lifetimeSiteGrey", "lifetime site grey", "lifetime_site_grey_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 43_500;\n            lab.Ledger.Lifetime.Laundered = 24_000;", "43,500", "24,000"),
    ("LastWeekGpuHardwareSummaryLine", "lastWeekGpuHardware", "last week gpu hardware", "last_week_gpu_hardware_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_200;\n            lab.Ledger.LastWeek.Hardware = -4_500;", "2,200", "4,500"),
    ("ThisWeekChipGreyGrantSummaryLine", "thisWeekChipGreyGrant", "this week chip grey grant", "this_week_chip_grey_grant_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 4_600;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_720;", "4,600", "1,720"),
    ("LastWeekTrainingHiringSummaryLine", "lastWeekTrainingHiring", "last week training hiring", "last_week_training_hiring_summary_line",
     "lab.Ledger.LastWeek.Training = -9_500;\n            lab.Ledger.LastWeek.Hiring = -3_100;", "9,500", "3,100"),
]

SPRINT38 = [
    ("ThisWeekRevenueConstructionSummaryLine", "thisWeekRevenueConstruction", "this week revenue construction", "this_week_revenue_construction_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 14_400;\n            lab.Ledger.ThisWeek.Construction = -4_100;", "14,400", "4,100"),
    ("LifetimeGrantFinesSummaryLine", "lifetimeGrantFines", "lifetime grant fines", "lifetime_grant_fines_summary_line",
     "lab.Ledger.Lifetime.Grants = 100_000;\n            lab.Ledger.Lifetime.Fines = -20_000;", "100,000", "20,000"),
    ("ThisWeekSalariesGreySummaryLine", "thisWeekSalariesGrey", "this week salaries grey", "this_week_salaries_grey_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_700;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_800;", "7,700", "1,800"),
    ("LastWeekRevenueSiteSummaryLine", "lastWeekRevenueSite", "last week revenue site", "last_week_revenue_site_summary_line",
     "lab.Ledger.LastWeek.Revenue = 19_800;\n            lab.Ledger.LastWeek.SiteBuild = -8_100;", "19,800", "8,100"),
    ("ThisWeekRentFinesSummaryLine", "thisWeekRentFines", "this week rent fines", "this_week_rent_fines_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_980;\n            lab.Ledger.ThisWeek.Fines = -360;", "1,980", "360"),
    ("LastWeekConstructionGrantSummaryLine", "lastWeekConstructionGrant", "last week construction grant", "last_week_construction_grant_summary_line",
     "lab.Ledger.LastWeek.Construction = -6_200;\n            lab.Ledger.LastWeek.Grants = 4_700;", "6,200", "4,700"),
]

SPRINT39 = [
    ("ThisWeekNetSalariesSummaryLine", "thisWeekNetSalaries", "this week net salaries", "this_week_net_salaries_summary_line",
     "lab.Ledger.ThisWeek.Net = 2_100;\n            lab.Ledger.ThisWeek.Salaries = -7_800;", "2,100", "7,800"),
    ("LifetimeLaunderPowerSummaryLine", "lifetimeLaunderPower", "lifetime launder power", "lifetime_launder_power_summary_line",
     "lab.Ledger.Lifetime.Laundered = 25_500;\n            lab.Ledger.Lifetime.Power = -125_000;", "25,500", "125,000"),
    ("LastWeekPatentLandSummaryLine", "lastWeekPatentLand", "last week patent land", "last_week_patent_land_summary_line",
     "lab.Ledger.LastWeek.Land = -11_000;\n            lab.Ledger.LastWeek.ChipSales = 6_900;", "11,000", "6,900"),
    ("ThisWeekPowerConstructionSummaryLine", "thisWeekPowerConstruction", "this week power construction", "this_week_power_construction_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_600;\n            lab.Ledger.ThisWeek.Construction = -3_500;", "2,600", "3,500"),
    ("LastWeekLaunderCapitalSummaryLine", "lastWeekLaunderCapital", "last week launder capital", "last_week_launder_capital_summary_line",
     "lab.Ledger.LastWeek.Laundered = 2_800;\n            lab.Ledger.LastWeek.CapitalIn = 40_000;", "2,800", "40,000"),
    ("ThisWeekGreySalariesSummaryLine", "thisWeekGreySalaries", "this week grey salaries", "this_week_grey_salaries_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_920;\n            lab.Ledger.ThisWeek.Salaries = -7_400;", "1,920", "7,400"),
]

SPRINT40 = [
    ("LifetimeNetGrantSummaryLine", "lifetimeNetGrant", "lifetime net grant", "lifetime_net_grant_summary_line",
     "lab.Ledger.Lifetime.Grants = 102_000;\n            lab.Ledger.Lifetime.ChipSales = 200_000;", "102,000", "200,000"),
    ("LastWeekHardwareTrainingSummaryLine", "lastWeekHardwareTraining", "last week hardware training", "last_week_hardware_training_summary_line",
     "lab.Ledger.LastWeek.Hardware = -4_700;\n            lab.Ledger.LastWeek.Training = -8_900;", "4,700", "8,900"),
    ("ThisWeekGrantNetChipSummaryLine", "thisWeekGrantNetChip", "this week grant net chip", "this_week_grant_net_chip_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_100;\n            lab.Ledger.ThisWeek.ChipSales = 5_000;", "4,100", "5,000"),
    ("LastWeekSitePowerSummaryLine", "lastWeekSitePower", "last week site power", "last_week_site_power_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -7_900;\n            lab.Ledger.LastWeek.Power = -3_300;", "7,900", "3,300"),
    ("ThisWeekGrantFinesSummaryLine", "thisWeekGrantFines", "this week grant fines", "this_week_grant_fines_summary_line",
     "lab.Ledger.ThisWeek.Grants = 3_800;\n            lab.Ledger.ThisWeek.Fines = -440;", "3,800", "440"),
    ("LastWeekRoomNetSummaryLine", "lastWeekRoomNet", "last week room net", "last_week_room_net_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_050;\n            lab.Ledger.LastWeek.Net = 2_600;", "2,050", "2,600"),
]

PA37 = '            string lastWeekGreyPatentPart = string.IsNullOrEmpty(lastWeekGreyPatent) ? "" : " · " + lastWeekGreyPatent;'
PA38 = '            string lastWeekTrainingHiringPart = string.IsNullOrEmpty(lastWeekTrainingHiring) ? "" : " · " + lastWeekTrainingHiring;'
PA39 = '            string lastWeekConstructionGrantPart = string.IsNullOrEmpty(lastWeekConstructionGrant) ? "" : " · " + lastWeekConstructionGrant;'
PA40 = '            string thisWeekGreySalariesPart = string.IsNullOrEmpty(thisWeekGreySalaries) ? "" : " · " + thisWeekGreySalaries;'


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


insert_formatters(F37_38)
git_commit("feat(hud): sprint37-38 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT37, PA37, "lastWeekGreyPatentPart")
run_sprint(SPRINT38, PA38, "lastWeekTrainingHiringPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F39_40)
git_commit("feat(hud): sprint39-40 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT39, PA39, "lastWeekConstructionGrantPart")
run_sprint(SPRINT40, PA40, "thisWeekGreySalariesPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)