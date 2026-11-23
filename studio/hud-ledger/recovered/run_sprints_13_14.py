#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

SPRINT13 = [
    ("LastWeekNetPowerSummaryLine", "lastWeekNetPower", "last week net power", "last_week_net_power_summary_line",
     "lab.Ledger.LastWeek.Net = 5_200;\n            lab.Ledger.LastWeek.Power = -4_100;", "5,200", "4,100"),
    ("ThisWeekFinesChipSummaryLine", "thisWeekFinesChip", "this week fines chip", "this_week_fines_chip_summary_line",
     "lab.Ledger.ThisWeek.Fines = -650;\n            lab.Ledger.ThisWeek.ChipSales = 5_100;", "650", "5,100"),
    ("LifetimeChipPowerSummaryLine", "lifetimeChipPower", "lifetime chip power", "lifetime_chip_power_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 65_000;\n            lab.Ledger.Lifetime.Power = -210_000;", "65,000", "210,000"),
    ("LastWeekGreyGrantsSummaryLine", "lastWeekGreyGrants", "last week grey grants", "last_week_grey_grants_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_900;\n            lab.Ledger.LastWeek.Grants = 8_000;", "2,900", "8,000"),
    ("ThisWeekGpuConstructionSummaryLine", "thisWeekGpuConstruction", "this week gpu construction", "this_week_gpu_construction_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_900;\n            lab.Ledger.ThisWeek.Construction = -5_800;", "1,900", "5,800"),
    ("LastWeekSiteHardwareSummaryLine", "lastWeekSiteHardware", "last week site hardware", "last_week_site_hardware_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -9_000;\n            lab.Ledger.LastWeek.Hardware = -12_500;", "9,000", "12,500"),
]

SPRINT14 = [
    ("LastWeekLaunderFinesSummaryLine", "lastWeekLaunderFines", "last week launder fines", "last_week_launder_fines_summary_line",
     "lab.Ledger.LastWeek.Laundered = 1_400;\n            lab.Ledger.LastWeek.Fines = -850;", "1,400", "850"),
    ("LifetimeTrainingChipSummaryLine", "lifetimeTrainingChip", "lifetime training chip", "lifetime_training_chip_summary_line",
     "lab.Ledger.Lifetime.Training = -350_000;\n            lab.Ledger.Lifetime.ChipSales = 42_000;", "350,000", "42,000"),
    ("ThisWeekRevenueTrainingSummaryLine", "thisWeekRevenueTraining", "this week revenue training", "this_week_revenue_training_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 16_000;\n            lab.Ledger.ThisWeek.Training = -7_200;", "16,000", "7,200"),
    ("LastWeekPowerChipDevSummaryLine", "lastWeekPowerChipDev", "last week power chip dev", "last_week_power_chip_dev_summary_line",
     "lab.Ledger.LastWeek.Power = -5_500;\n            lab.Ledger.LastWeek.ChipDevelopment = -4_000;", "5,500", "4,000"),
    ("ThisWeekSalariesGrantsSummaryLine", "thisWeekSalariesGrants", "this week salaries grants", "this_week_salaries_grants_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -9_300;\n            lab.Ledger.ThisWeek.Grants = 3_600;", "9,300", "3,600"),
    ("LastWeekNetGreySummaryLine", "lastWeekNetGrey", "last week net grey", "last_week_net_grey_summary_line",
     "lab.Ledger.LastWeek.Net = 6_700;\n            lab.Ledger.LastWeek.GreyRevenue = 2_100;", "6,700", "2,100"),
]

PA13 = '            string thisWeekGrantsChipPart = string.IsNullOrEmpty(thisWeekGrantsChip) ? "" : " · " + thisWeekGrantsChip;'
PA14 = '            string lastWeekSiteHardwarePart = string.IsNullOrEmpty(lastWeekSiteHardware) ? "" : " · " + lastWeekSiteHardware;'


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
subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint13-14 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT13, PA13, "thisWeekGrantsChipPart")
run_sprint(SPRINT14, PA14, "lastWeekSiteHardwarePart")
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)