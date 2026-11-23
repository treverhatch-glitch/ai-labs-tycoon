#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
CYCLES = [
    {"method": "LastWeekCapitalGreySummaryLine", "var": "lastWeekCapitalGrey", "slug": "last week capital grey",
     "test_name": "last_week_capital_grey_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.CapitalIn = 20_000;
            lab.Ledger.LastWeek.GreyRevenue = 1_200;
            string line = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
            Assert.That(line, Does.Contain("20,000").Or.Contain("1,200"));"""},
    {"method": "LifetimeChipGreySummaryLine", "var": "lifetimeChipGrey", "slug": "lifetime chip grey",
     "test_name": "lifetime_chip_grey_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.ChipSales = 60_000;
            lab.Ledger.Lifetime.GreyRevenue = 9_000;
            string line = HqDisplayQueriesHud.LifetimeChipGreySummaryLine(lab);
            Assert.That(line, Does.Contain("60,000").Or.Contain("9,000"));"""},
    {"method": "LastWeekTrainingGpuUpkeepSummaryLine", "var": "lastWeekTrainingGpuUpkeep", "slug": "last week training gpu upkeep",
     "test_name": "last_week_training_gpu_upkeep_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -11_000;
            lab.Ledger.LastWeek.GpuUpkeep = -900;
            string line = HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("900"));"""},
    {"method": "LastWeekChipDevSalesSummaryLine", "var": "lastWeekChipDevSales", "slug": "last week chip dev sales",
     "test_name": "last_week_chip_dev_sales_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipDevelopment = -5_500;
            lab.Ledger.LastWeek.ChipSales = 7_800;
            string line = HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("7,800"));"""},
    {"method": "ThisWeekRevenueGrantsSummaryLine", "var": "thisWeekRevenueGrants", "slug": "this week revenue grants",
     "test_name": "this_week_revenue_grants_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Revenue = 13_000;
            lab.Ledger.ThisWeek.Grants = 4_500;
            string line = HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("13,000").Or.Contain("4,500"));"""},
    {"method": "LastWeekConstructionLandSummaryLine", "var": "lastWeekConstructionLand", "slug": "last week construction land",
     "test_name": "last_week_construction_land_summary_line",
     "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -8_500;
            lab.Ledger.LastWeek.Land = -2_200;
            string line = HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(lab);
            Assert.That(line, Does.Contain("8,500").Or.Contain("2,200"));"""},
]
HUD = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
PART = '            string lastWeekRoomHiringPart = string.IsNullOrEmpty(lastWeekRoomHiring) ? "" : " · " + lastWeekRoomHiring;'

def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)

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

def wire_staff(c):
    t = STAFF.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t: return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(PART, PART + "\n" + ins.rstrip("\n"), 1)
    tail = re.search(r"(\+ lastWeekRoomHiringPart(?: \+ \w+Part)*)", t)
    t = t.replace(tail.group(1), tail.group(1) + f" + {c['var']}Part", 1)
    STAFF.write_text(t); return True

def wire_mascot(c):
    t = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(sim.Player)" in t: return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(sim.Player);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(PART, PART + "\n" + ins.rstrip("\n"), 1)
    tail = re.search(r"(\+ lastWeekRoomHiringPart(?: \+ \w+Part)*);", t)
    t = t.replace(tail.group(1) + ";", tail.group(1) + f" + {c['var']}Part;", 1)
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

def run_cycle(c):
    if wire_hud(c): git_commit(f"feat(hud): wire hud view {c['slug']}", [HUD])
    if wire_train(c): git_commit(f"feat(hud): wire training {c['slug']}", [TRAIN])
    if wire_staff(c): git_commit(f"feat(hud): wire staff gpu {c['slug']}", [STAFF])
    if wire_mascot(c): git_commit(f"feat(hud): wire build mascot {c['slug']}", [MASCOT])
    if wire_build(c): git_commit(f"feat(hud): wire build panel {c['slug']}", [BUILD])
    if add_test(c): git_commit(f"test(hud): {c['slug']} summary line", [TESTS])

for i, c in enumerate(CYCLES, 1):
    run_cycle(c)
    if i in {2, 4}:
        subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)
subprocess.check_call(["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"], cwd=ROOT)