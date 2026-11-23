from pathlib import Path
import re, subprocess
ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
SPRINT9 = [
    ("LastWeekGrantsRevenueSummaryLine", "lastWeekGrantsRevenue", "last week grants revenue",
     "last_week_grants_revenue_summary_line", "lab.Ledger.LastWeek.Grants = 7_000;\n            lab.Ledger.LastWeek.Revenue = 19_000;", "7,000", "19,000"),
    ("LifetimeFinesPowerSummaryLine", "lifetimeFinesPower", "lifetime fines power",
     "lifetime_fines_power_summary_line", "lab.Ledger.Lifetime.Fines = -12_000;\n            lab.Ledger.Lifetime.Power = -95_000;", "12,000", "95,000"),
    ("ThisWeekGpuNetSummaryLine", "thisWeekGpuNet", "this week gpu net",
     "this_week_gpu_net_summary_line", "lab.Ledger.ThisWeek.GpuRent = -2_100;\n            lab.Ledger.ThisWeek.Net = 1_400;", "2,100", "1,400"),
    ("LastWeekPatentHiringSummaryLine", "lastWeekPatentHiring", "last week patent hiring",
     "last_week_patent_hiring_summary_line", "lab.Ledger.LastWeek.Hiring = -3_800;\n            lab.Ledger.LastWeek.RoomUpkeep = -1_100;", "3,800", "1,100"),
    ("ThisWeekChipConstructionSummaryLine", "thisWeekChipConstruction", "this week chip construction",
     "this_week_chip_construction_summary_line", "lab.Ledger.ThisWeek.ChipSales = 5_600;\n            lab.Ledger.ThisWeek.Construction = -4_400;", "5,600", "4,400"),
    ("ThisWeekSalariesFinesSummaryLine", "thisWeekSalariesFines", "this week salaries fines",
     "this_week_salaries_fines_summary_line", "lab.Ledger.ThisWeek.Salaries = -7_700;\n            lab.Ledger.ThisWeek.Fines = -500;", "7,700", "500"),
]

SPRINT10 = [
    ("LastWeekNetFinesSummaryLine", "lastWeekNetFines", "last week net fines",
     "last_week_net_fines_summary_line", "lab.Ledger.LastWeek.Net = 2_800;\n            lab.Ledger.LastWeek.Fines = -900;", "2,800", "900"),
    ("ThisWeekGreyPowerSummaryLine", "thisWeekGreyPower", "this week grey power",
     "this_week_grey_power_summary_line", "lab.Ledger.ThisWeek.GreyRevenue = 1_600;\n            lab.Ledger.ThisWeek.Power = -3_300;", "1,600", "3,300"),
    ("LifetimeGrantsLaunderSummaryLine", "lifetimeGrantsLaunder", "lifetime grants launder",
     "lifetime_grants_launder_summary_line", "lab.Ledger.Lifetime.Grants = 80_000;\n            lab.Ledger.Lifetime.Laundered = 11_000;", "80,000", "11,000"),
    ("LastWeekSalariesPowerSummaryLine", "lastWeekSalariesPower", "last week salaries power",
     "last_week_salaries_power_summary_line", "lab.Ledger.LastWeek.Salaries = -9_500;\n            lab.Ledger.LastWeek.Power = -4_000;", "9,500", "4,000"),
    ("ThisWeekTrainingPowerSummaryLine", "thisWeekTrainingPower", "this week training power",
     "this_week_training_power_summary_line", "lab.Ledger.ThisWeek.Training = -6_200;\n            lab.Ledger.ThisWeek.Power = -2_800;", "6,200", "2,800"),
    ("LastWeekChipGreySummaryLine", "lastWeekChipGrey", "last week chip grey",
     "last_week_chip_grey_summary_line", "lab.Ledger.LastWeek.ChipSales = 8_900;\n            lab.Ledger.LastWeek.GreyRevenue = 1_900;", "8,900", "1,900"),
]


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


PA9 = '            string lastWeekConstructionLandPart = string.IsNullOrEmpty(lastWeekConstructionLand) ? "" : " · " + lastWeekConstructionLand;'
PA10 = '            string thisWeekSalariesFinesPart = string.IsNullOrEmpty(thisWeekSalariesFines) ? "" : " · " + thisWeekSalariesFines;'

git_commit = git_commit  # noqa
subprocess.check_call(["git", "add", str(UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs")], cwd=ROOT)
if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode != 0:
    subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint9-10 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT9, PA9, "lastWeekConstructionLandPart")
subprocess.check_call(["git", "commit", "--allow-empty", "-m", "docs: HUD HOLD sprint9 wire complete"], cwd=ROOT)
run_sprint(SPRINT10, PA10, "thisWeekSalariesFinesPart")
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)