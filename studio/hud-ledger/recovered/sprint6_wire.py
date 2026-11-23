#!/usr/bin/env python3
"""Wire sprint 6 HUD summary lines."""
import re
import subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"

CYCLES = [
    {
        "method": "ThisWeekSalariesGpuSummaryLine",
        "var": "thisWeekSalariesGpu",
        "slug": "this week salaries gpu",
        "test_name": "this_week_salaries_gpu_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Salaries = -6_000;
            lab.Ledger.ThisWeek.GpuRent = -1_800;
            string line = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
            Assert.That(line, Does.Contain("6,000").Or.Contain("1,800"));""",
    },
    {
        "method": "LastWeekHardwarePowerSummaryLine",
        "var": "lastWeekHardwarePower",
        "slug": "last week hardware power",
        "test_name": "last_week_hardware_power_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hardware = -14_000;
            lab.Ledger.LastWeek.Power = -3_500;
            string line = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(lab);
            Assert.That(line, Does.Contain("14,000").Or.Contain("3,500"));""",
    },
    {
        "method": "LifetimeTrainingLaunderSummaryLine",
        "var": "lifetimeTrainingLaunder",
        "slug": "lifetime training launder",
        "test_name": "lifetime_training_launder_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Training = -300_000;
            lab.Ledger.Lifetime.Laundered = 15_000;
            string line = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("300,000").Or.Contain("15,000"));""",
    },
    {
        "method": "LastWeekChipSalesGrantsSummaryLine",
        "var": "lastWeekChipSalesGrants",
        "slug": "last week chip sales grants",
        "test_name": "last_week_chip_sales_grants_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipSales = 11_000;
            lab.Ledger.LastWeek.Grants = 6_000;
            string line = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("6,000"));""",
    },
    {
        "method": "LastWeekHiringGreySummaryLine",
        "var": "lastWeekHiringGrey",
        "slug": "last week hiring grey",
        "test_name": "last_week_hiring_grey_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -4_200;
            lab.Ledger.LastWeek.GreyRevenue = 2_800;
            string line = HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(lab);
            Assert.That(line, Does.Contain("4,200").Or.Contain("2,800"));""",
    },
    {
        "method": "ThisWeekPowerChipSummaryLine",
        "var": "thisWeekPowerChip",
        "slug": "this week power chip",
        "test_name": "this_week_power_chip_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Power = -5_000;
            lab.Ledger.ThisWeek.ChipSales = 8_500;
            string line = HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(lab);
            Assert.That(line, Does.Contain("5,000").Or.Contain("8,500"));""",
    },
]

HUD = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
PART_ANCHOR = '            string lastWeekTrainingRevenuePart = string.IsNullOrEmpty(lastWeekTrainingRevenue) ? "" : " · " + lastWeekTrainingRevenue;'


def git_commit(msg: str, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def wire_hud(cycle):
    text = HUD.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""                string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                if (!string.IsNullOrEmpty({cycle['var']}))
                    status = string.IsNullOrEmpty(status) ? {cycle['var']} : status + " · " + {cycle['var']};
"""
    before = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    text = text.replace(before, insert.rstrip("\n") + "\n" + before, 1)
    HUD.write_text(text)
    return True


def wire_train(cycle):
    text = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""                    string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                    if (!string.IsNullOrEmpty({cycle['var']}))
                        _summary.text += " · " + {cycle['var']};
"""
    before = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    text = text.replace(before, insert.rstrip("\n") + "\n" + before, 1)
    TRAIN.write_text(text)
    return True


def wire_staff(cycle):
    text = STAFF.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    text = text.replace(PART_ANCHOR, PART_ANCHOR + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ lastWeekTrainingRevenuePart(?: \+ \w+Part)*)", text)
    if not old_tail:
        raise SystemExit("StaffGpuPanel tail missing")
    text = text.replace(old_tail.group(1), old_tail.group(1) + f" + {cycle['var']}Part", 1)
    STAFF.write_text(text)
    return True


def wire_mascot(cycle):
    text = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(sim.Player)" in text:
        return False
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(sim.Player);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    text = text.replace(PART_ANCHOR, PART_ANCHOR + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ lastWeekTrainingRevenuePart(?: \+ \w+Part)*);", text)
    if not old_tail:
        raise SystemExit("Mascot tail missing")
    text = text.replace(old_tail.group(1) + ";", old_tail.group(1) + f" + {cycle['var']}Part;", 1)
    MASCOT.write_text(text)
    return True


def wire_build(cycle):
    text = BUILD.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(_host.Simulation.Player)" in text:
        return False
    before = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{cycle["method"]}(_host.Simulation.Player)\n'
    text = text.replace(before, line + before, 1)
    BUILD.write_text(text)
    return True


def add_test(cycle):
    text = TESTS.read_text()
    if cycle["test_name"] in text:
        return False
    block = f"""
        [Test]
        public void {cycle['test_name']}()
        {{
{cycle['test_body']}
        }}
"""
    insert_at = text.rfind("\n    }\n}")
    text = text[:insert_at] + block + text[insert_at:]
    TESTS.write_text(text)
    return True


def run_cycle(cycle):
    print(f"=== {cycle['slug']} ===")
    if wire_hud(cycle):
        git_commit(f"feat(hud): wire hud view {cycle['slug']}", [HUD])
    if wire_train(cycle):
        git_commit(f"feat(hud): wire training {cycle['slug']}", [TRAIN])
    if wire_staff(cycle):
        git_commit(f"feat(hud): wire staff gpu {cycle['slug']}", [STAFF])
    if wire_mascot(cycle):
        git_commit(f"feat(hud): wire build mascot {cycle['slug']}", [MASCOT])
    if wire_build(cycle):
        git_commit(f"feat(hud): wire build panel {cycle['slug']}", [BUILD])
    if add_test(cycle):
        git_commit(f"test(hud): {cycle['slug']} summary line", [TESTS])


def main():
    golden_after = {2, 4}
    for i, cycle in enumerate(CYCLES, 1):
        run_cycle(cycle)
        if i in golden_after:
            subprocess.check_call(
                [
                    "dotnet",
                    "test",
                    "tools/SimTests/SimTests.csproj",
                    "--filter",
                    "FullyQualifiedName~golden",
                    "-v",
                    "q",
                ],
                cwd=ROOT,
            )


if __name__ == "__main__":
    main()