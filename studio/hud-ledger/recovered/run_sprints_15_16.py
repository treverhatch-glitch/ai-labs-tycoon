#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

SPRINT15 = [
    ("ThisWeekGrantNetSummaryLine", "thisWeekGrantNet", "this week grant net", "this_week_grant_net_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_200;\n            lab.Ledger.ThisWeek.Net = 1_900;", "4,200", "1,900"),
    ("LastWeekTrainingConstructionSummaryLine", "lastWeekTrainingConstruction", "last week training construction", "last_week_training_construction_summary_line",
     "lab.Ledger.LastWeek.Training = -13_000;\n            lab.Ledger.LastWeek.Construction = -7_200;", "13,000", "7,200"),
    ("LifetimeFinesGreySummaryLine", "lifetimeFinesGrey", "lifetime fines grey", "lifetime_fines_grey_summary_line",
     "lab.Ledger.Lifetime.Fines = -15_000;\n            lab.Ledger.Lifetime.GreyRevenue = 18_000;", "15,000", "18,000"),
    ("ThisWeekChipGpuSummaryLine", "thisWeekChipGpu", "this week chip gpu", "this_week_chip_gpu_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 6_800;\n            lab.Ledger.ThisWeek.GpuRent = -2_300;", "6,800", "2,300"),
    ("LastWeekHiringCapitalSummaryLine", "lastWeekHiringCapital", "last week hiring capital", "last_week_hiring_capital_summary_line",
     "lab.Ledger.LastWeek.Hiring = -4_800;\n            lab.Ledger.LastWeek.CapitalIn = 35_000;", "4,800", "35,000"),
    ("LastWeekRoomPowerSummaryLine", "lastWeekRoomPower", "last week room power", "last_week_room_power_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_400;\n            lab.Ledger.LastWeek.Power = -5_100;", "2,400", "5,100"),
]

SPRINT16 = [
    ("ThisWeekGreyConstructionSummaryLine", "thisWeekGreyConstruction", "this week grey construction", "this_week_grey_construction_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_800;\n            lab.Ledger.ThisWeek.Construction = -4_900;", "1,800", "4,900"),
    ("LastWeekChipPatentSummaryLine", "lastWeekChipPatent", "last week chip patent", "last_week_chip_patent_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 9_200;\n            lab.Ledger.LastWeek.CapitalIn = 28_000;", "9,200", "28,000"),
    ("LifetimeRevenueGrantsSummaryLine", "lifetimeRevenueGrants", "lifetime revenue grants", "lifetime_revenue_grants_summary_line",
     "lab.Ledger.Lifetime.Revenue = 600_000;\n            lab.Ledger.Lifetime.Grants = 90_000;", "600,000", "90,000"),
    ("ThisWeekFinesTrainingSummaryLine", "thisWeekFinesTraining", "this week fines training", "this_week_fines_training_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_050;\n            lab.Ledger.ThisWeek.Training = -5_400;", "1,050", "5,400"),
    ("LastWeekGpuGreySummaryLine", "lastWeekGpuGrey", "last week gpu grey", "last_week_gpu_grey_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_700;\n            lab.Ledger.LastWeek.GreyRevenue = 1_500;", "2,700", "1,500"),
    ("ThisWeekNetChipSummaryLine", "thisWeekNetChip", "this week net chip", "this_week_net_chip_summary_line",
     "lab.Ledger.ThisWeek.Net = 2_600;\n            lab.Ledger.ThisWeek.ChipSales = 4_100;", "2,600", "4,100"),
]

PA15 = '            string lastWeekNetGreyPart = string.IsNullOrEmpty(lastWeekNetGrey) ? "" : " · " + lastWeekNetGrey;'
PA16 = '            string lastWeekRoomPowerPart = string.IsNullOrEmpty(lastWeekRoomPower) ? "" : " · " + lastWeekRoomPower;'


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
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t: return False
    ins = f"""                string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                if (!string.IsNullOrEmpty({c['var']}))
                    status = string.IsNullOrEmpty(status) ? {c['var']} : status + " · " + {c['var']};
"""
    b = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    HUD.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1)); return True


def wire_train(c):
    t = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t: return False
    ins = f"""                    string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                    if (!string.IsNullOrEmpty({c['var']}))
                        _summary.text += " · " + {c['var']};
"""
    b = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    TRAIN.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1)); return True


def wire_staff(c, part_anchor, tail_token):
    t = STAFF.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t: return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*)", t)
    t = t.replace(m.group(1), m.group(1) + f" + {c['var']}Part", 1)
    STAFF.write_text(t); return True


def wire_mascot(c, part_anchor, tail_token):
    t = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(sim.Player)" in t: return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(sim.Player);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*);", t)
    t = t.replace(m.group(1) + ";", m.group(1) + f" + {c['var']}Part;", 1)
    MASCOT.write_text(t); return True


def wire_build(c):
    t = BUILD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(_host.Simulation.Player)" in t: return False
    b = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{c["method"]}(_host.Simulation.Player)\n'
    BUILD.write_text(t.replace(b, line + b, 1)); return True


def add_test(c):
    t = TESTS.read_text()
    if c["test_name"] in t: return False
    block = f"\n        [Test]\n        public void {c['test_name']}()\n        {{\n{c['test_body']}\n        }}\n"
    at = t.rfind("\n    }\n}")
    TESTS.write_text(t[:at] + block + t[at:]); return True


def run_sprint(cycles, part_anchor, tail_token):
    for i, raw in enumerate(cycles, 1):
        c = cycle_def(raw)
        if wire_hud(c): git_commit(f"feat(hud): wire hud view {c['slug']}", [HUD])
        if wire_train(c): git_commit(f"feat(hud): wire training {c['slug']}", [TRAIN])
        if wire_staff(c, part_anchor, tail_token): git_commit(f"feat(hud): wire staff gpu {c['slug']}", [STAFF])
        if wire_mascot(c, part_anchor, tail_token): git_commit(f"feat(hud): wire build mascot {c['slug']}", [MASCOT])
        if wire_build(c): git_commit(f"feat(hud): wire build panel {c['slug']}", [BUILD])
        if add_test(c): git_commit(f"test(hud): {c['slug']} summary line", [TESTS])
        if i in {2, 4}:
            subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)


subprocess.check_call(["git", "add", str(UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs")], cwd=ROOT)
subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint15-16 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT15, PA15, "lastWeekNetGreyPart")
run_sprint(SPRINT16, PA16, "lastWeekRoomPowerPart")
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)