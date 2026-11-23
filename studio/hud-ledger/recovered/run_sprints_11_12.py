#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

SPRINT11 = [
    ("ThisWeekGpuFinesGreySummaryLine", "thisWeekGpuFinesGrey", "this week gpu fines grey",
     "this_week_gpu_fines_grey_summary_line", "lab.Ledger.ThisWeek.GpuRent = -2_400;\n            lab.Ledger.ThisWeek.Fines = -350;", "2,400", "350"),
    ("LastWeekConstructionGreySummaryLine", "lastWeekConstructionGrey", "last week construction grey",
     "last_week_construction_grey_summary_line", "lab.Ledger.LastWeek.Construction = -5_500;\n            lab.Ledger.LastWeek.GreyRevenue = 1_700;", "5,500", "1,700"),
    ("LifetimeRevenuePowerSummaryLine", "lifetimeRevenuePower", "lifetime revenue power",
     "lifetime_revenue_power_summary_line", "lab.Ledger.Lifetime.Revenue = 500_000;\n            lab.Ledger.Lifetime.Power = -180_000;", "500,000", "180,000"),
    ("LastWeekGpuFinesSummaryLine", "lastWeekGpuFines", "last week gpu fines",
     "last_week_gpu_fines_summary_line", "lab.Ledger.LastWeek.GpuRent = -3_100;\n            lab.Ledger.LastWeek.Fines = -750;", "3,100", "750"),
    ("ThisWeekSalariesNetSummaryLine", "thisWeekSalariesNet", "this week salaries net",
     "this_week_salaries_net_summary_line", "lab.Ledger.ThisWeek.Salaries = -8_200;\n            lab.Ledger.ThisWeek.Net = 2_100;", "8,200", "2,100"),
    ("LastWeekCapitalPatentSummaryLine", "lastWeekCapitalPatent", "last week capital patent",
     "last_week_capital_patent_summary_line", "lab.Ledger.LastWeek.CapitalIn = 30_000;\n            lab.Ledger.LastWeek.Grants = 5_000;", "30,000", "5,000"),
]

SPRINT12 = [
    ("ThisWeekChipTrainingSummaryLine", "thisWeekChipTraining", "this week chip training",
     "this_week_chip_training_summary_line", "lab.Ledger.ThisWeek.ChipSales = 7_500;\n            lab.Ledger.ThisWeek.Training = -9_500;", "7,500", "9,500"),
    ("LastWeekRevenueChipSummaryLine", "lastWeekRevenueChip", "last week revenue chip",
     "last_week_revenue_chip_summary_line", "lab.Ledger.LastWeek.Revenue = 24_000;\n            lab.Ledger.LastWeek.ChipSales = 6_400;", "24,000", "6,400"),
    ("LifetimeGreyNetSummaryLine", "lifetimeGreyNet", "lifetime grey net",
     "lifetime_grey_net_summary_line", "lab.Ledger.Lifetime.GreyRevenue = 22_000;\n            lab.Ledger.Lifetime.Net = 110_000;", "22,000", "110,000"),
    ("ThisWeekConstructionPowerSummaryLine", "thisWeekConstructionPower", "this week construction power",
     "this_week_construction_power_summary_line", "lab.Ledger.ThisWeek.Construction = -6_600;\n            lab.Ledger.ThisWeek.Power = -3_900;", "6,600", "3,900"),
    ("LastWeekSalariesHiringSummaryLine", "lastWeekSalariesHiring", "last week salaries hiring",
     "last_week_salaries_hiring_summary_line", "lab.Ledger.LastWeek.Salaries = -10_500;\n            lab.Ledger.LastWeek.Hiring = -4_000;", "10,500", "4,000"),
    ("ThisWeekGrantsChipSummaryLine", "thisWeekGrantsChip", "this week grants chip",
     "this_week_grants_chip_summary_line", "lab.Ledger.ThisWeek.Grants = 6_800;\n            lab.Ledger.ThisWeek.ChipSales = 4_200;", "6,800", "4,200"),
]

PA11 = '            string lastWeekChipGreyPart = string.IsNullOrEmpty(lastWeekChipGrey) ? "" : " · " + lastWeekChipGrey;'
PA12 = '            string lastWeekCapitalPatentPart = string.IsNullOrEmpty(lastWeekCapitalPatent) ? "" : " · " + lastWeekCapitalPatent;'


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
subprocess.check_call(["git", "add", str(UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs")], cwd=ROOT)
if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode != 0:
    subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint11-12 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT11, PA11, "lastWeekChipGreyPart")
run_sprint(SPRINT12, PA12, "lastWeekCapitalPatentPart")
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)