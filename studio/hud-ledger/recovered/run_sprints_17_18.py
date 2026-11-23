#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

SPRINT17 = [
    ("LastWeekGrantTrainingSummaryLine", "lastWeekGrantTraining", "last week grant training", "last_week_grant_training_summary_line",
     "lab.Ledger.LastWeek.Grants = 9_000;\n            lab.Ledger.LastWeek.Training = -14_000;", "9,000", "14,000"),
    ("ThisWeekRevenuePowerSummaryLine", "thisWeekRevenuePower", "this week revenue power", "this_week_revenue_power_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 17_500;\n            lab.Ledger.ThisWeek.Power = -4_600;", "17,500", "4,600"),
    ("LifetimeChipDevSummaryLine", "lifetimeChipDev", "lifetime chip dev", "lifetime_chip_dev_summary_line",
     "lab.Ledger.Lifetime.ChipDevelopment = -55_000;\n            lab.Ledger.Lifetime.ChipSales = 48_000;", "55,000", "48,000"),
    ("LastWeekConstructionChipSummaryLine", "lastWeekConstructionChip", "last week construction chip", "last_week_construction_chip_summary_line",
     "lab.Ledger.LastWeek.Construction = -8_800;\n            lab.Ledger.LastWeek.ChipSales = 7_100;", "8,800", "7,100"),
    ("ThisWeekGreyTrainingSummaryLine", "thisWeekGreyTraining", "this week grey training", "this_week_grey_training_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_300;\n            lab.Ledger.ThisWeek.Training = -6_100;", "2,300", "6,100"),
    ("LastWeekFinesCapitalSummaryLine", "lastWeekFinesCapital", "last week fines capital", "last_week_fines_capital_summary_line",
     "lab.Ledger.LastWeek.Fines = -1_200;\n            lab.Ledger.LastWeek.CapitalIn = 40_000;", "1,200", "40,000"),
]

SPRINT18 = [
    ("ThisWeekGpuSalariesSummaryLine", "thisWeekGpuSalaries", "this week gpu salaries", "this_week_gpu_salaries_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_050;\n            lab.Ledger.ThisWeek.Salaries = -8_800;", "2,050", "8,800"),
    ("LastWeekLandPatentSummaryLine", "lastWeekLandPatent", "last week land patent", "last_week_land_patent_summary_line",
     "lab.Ledger.LastWeek.Land = -4_500;\n            lab.Ledger.LastWeek.SiteBuild = -11_000;", "4,500", "11,000"),
    ("LifetimeLaunderGrantsSummaryLine", "lifetimeLaunderGrants", "lifetime launder grants", "lifetime_launder_grants_summary_line",
     "lab.Ledger.Lifetime.Laundered = 25_000;\n            lab.Ledger.Lifetime.Grants = 95_000;", "25,000", "95,000"),
    ("ThisWeekConstructionNetSummaryLine", "thisWeekConstructionNet", "this week construction net", "this_week_construction_net_summary_line",
     "lab.Ledger.ThisWeek.Construction = -5_700;\n            lab.Ledger.ThisWeek.Net = 3_400;", "5,700", "3,400"),
    ("LastWeekRoomGreySummaryLine", "lastWeekRoomGrey", "last week room grey", "last_week_room_grey_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -1_800;\n            lab.Ledger.LastWeek.GreyRevenue = 2_600;", "1,800", "2,600"),
    ("ThisWeekChipFinesSummaryLine", "thisWeekChipFines", "this week chip fines", "this_week_chip_fines_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_900;\n            lab.Ledger.ThisWeek.Fines = -480;", "5,900", "480"),
]

PA17 = '            string thisWeekNetChipPart = string.IsNullOrEmpty(thisWeekNetChip) ? "" : " · " + thisWeekNetChip;'
PA18 = '            string lastWeekFinesCapitalPart = string.IsNullOrEmpty(lastWeekFinesCapital) ? "" : " · " + lastWeekFinesCapital;'


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
subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint17-18 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT17, PA17, "thisWeekNetChipPart")
run_sprint(SPRINT18, PA18, "lastWeekFinesCapitalPart")
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)